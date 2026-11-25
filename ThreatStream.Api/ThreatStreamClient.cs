using Refit;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ThreatStream.Api.Handlers;
using ThreatStream.Api.Serialization;

namespace ThreatStream.Api;

/// <summary>
/// Client for the Anomali ThreatStream REST API (v1 and v2). Each group of endpoints is a property (for example
/// <see cref="Confidence"/>); the groups are created on first use.
/// </summary>
/// <remarks>
/// <para>
/// Every request authenticates as <see cref="ThreatStreamClientOptions"/> describes and retries transient failures,
/// including rate limiting (429, honouring <c>Retry-After</c>). Non-success responses raise
/// <see cref="ThreatStreamApiException"/>; a failure to send raises the transport's own exception (such as
/// <see cref="HttpRequestException"/>, or <see cref="TimeoutException"/> when an attempt exceeds
/// <see cref="ThreatStreamClientOptions.Timeout"/>).
/// </para>
/// <para>
/// List operations return one page (<see cref="Models.ThreatStreamPage{T}"/>); <see cref="EnumerateAsync{T}"/> follows
/// <c>meta.next</c> through every page, and <see cref="EnumerateByUpdateIdAsync{T}"/> walks large intelligence result sets
/// by <c>update_id</c>.
/// </para>
/// <para>
/// A client is thread-safe and intended to be long-lived: create one per ThreatStream deployment and identity, and
/// dispose it when done.
/// </para>
/// </remarks>
public sealed partial class ThreatStreamClient : IDisposable
{
	private readonly HttpMessageHandler _pipeline;
	private readonly HttpClient _httpClient;

	/// <summary>Creates a client.</summary>
	/// <param name="options">Connection options.</param>
	public ThreatStreamClient(ThreatStreamClientOptions options) : this(options, CreateTransport(Validated(options)))
	{
	}

	/// <summary>
	/// Creates a client that sends requests through <paramref name="innerHandler"/> (for example a proxy-aware or
	/// instrumented handler). The client takes ownership of the handler and disposes it.
	/// <see cref="ThreatStreamClientOptions.TrustedServerCertificateThumbprint"/> and
	/// <see cref="ThreatStreamClientOptions.ServerCertificateValidationCallback"/> are ignored: configure certificate
	/// validation on the handler instead.
	/// </summary>
	/// <param name="options">Connection options.</param>
	/// <param name="innerHandler">The handler that sends requests to the network.</param>
	public ThreatStreamClient(ThreatStreamClientOptions options, HttpMessageHandler innerHandler)
	{
		ArgumentNullException.ThrowIfNull(innerHandler);
		Validated(options);
		BaseAddress = CreateBaseAddress(options.EffectiveBaseUrl);
		_pipeline = CreatePipeline(options, BaseAddress, innerHandler);
		_httpClient = new HttpClient(_pipeline, disposeHandler: false)
		{
			BaseAddress = BaseAddress,
			// The per-attempt timeout is applied inside RetryHandler so retries and Retry-After waits are not cut short.
			Timeout = System.Threading.Timeout.InfiniteTimeSpan
		};
	}

	/// <summary>The base address every endpoint path is appended to, always ending in <c>/</c>.</summary>
	public Uri BaseAddress { get; }

	/// <summary>The Refit settings shared by every endpoint group.</summary>
	internal static RefitSettings Settings { get; } = new()
	{
		ContentSerializer = new ThreatStreamContentSerializer(),
		// Interface paths are relative (no leading slash) so they append to a path-prefixed BaseUrl.
		UrlResolution = UrlResolutionMode.Rfc3986,
		UrlParameterFormatter = new ThreatStreamUrlParameterFormatter(),
		// ThreatStream reads a list filter as one comma-separated value (id__in=1,2,3), not a repeated parameter.
		CollectionFormat = CollectionFormat.Csv,
		// Unbuffered, Refit sends a serialized body as a read-once push stream, which can be neither retried nor resent
		// with a renewed OAuth token.
		Buffered = true,
		ExceptionFactory = response => new ValueTask<Exception?>(ThreatStreamErrorMapper.CreateAsync(response)),
		// Refit would wrap every exception thrown while sending in its ApiRequestException; surface them as themselves
		// (TimeoutException, HttpRequestException, ObjectDisposedException...), as documented.
		TransportExceptionFactory = static (_, exception, _) => exception
	};

	internal T For<T>() => RestService.For<T>(_httpClient, Settings);

	/// <inheritdoc />
	public void Dispose()
	{
		_httpClient.Dispose();
		_pipeline.Dispose();
	}

	// Validation comes before the transport is created, so invalid options do not leave an undisposed network handler.
	private static ThreatStreamClientOptions Validated(ThreatStreamClientOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		options.Validate();
		return options;
	}

	private static Uri CreateBaseAddress(string baseUrl) => new(baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/");

	private static HttpMessageHandler CreatePipeline(ThreatStreamClientOptions options, Uri baseAddress, HttpMessageHandler innerHandler)
	{
		HttpMessageHandler handler = new RetryHandler(options) { InnerHandler = innerHandler };
		handler = new AuthenticationHandler(options, baseAddress) { InnerHandler = handler };
		if (options.IsReadOnly)
		{
			handler = new ReadOnlyHandler(baseAddress) { InnerHandler = handler };
		}

		return handler;
	}

	/// <summary>
	/// How long an idle pooled connection is kept. Servers and load balancers close idle keep-alive connections after a
	/// few seconds; reusing one already closed fails with "connection forcibly closed" mid-request, so connections are
	/// dropped first.
	/// </summary>
	internal static readonly TimeSpan PooledConnectionIdleTimeout = TimeSpan.FromSeconds(5);

	/// <summary>How long any pooled connection is kept, so DNS changes (for example a load balancer move) are picked up.</summary>
	internal static readonly TimeSpan PooledConnectionLifetime = TimeSpan.FromMinutes(5);

	internal static SocketsHttpHandler CreateTransport(ThreatStreamClientOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		return CreateTransport(options.ServerCertificateValidationCallback, options.TrustedServerCertificateThumbprint);
	}

	/// <summary>
	/// Creates the network handler: <paramref name="validationCallback"/> when set, else trust for the certificate with
	/// <paramref name="trustedThumbprint"/> (SHA-256) on top of normal validation. Validation is never disabled.
	/// </summary>
	internal static SocketsHttpHandler CreateTransport(
		Func<X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? validationCallback,
		string? trustedThumbprint)
	{
		var handler = new SocketsHttpHandler
		{
			PooledConnectionIdleTimeout = PooledConnectionIdleTimeout,
			PooledConnectionLifetime = PooledConnectionLifetime
		};
		var callback = validationCallback ?? PinnedCallback(NormalizeThumbprint(trustedThumbprint));
		if (callback is not null)
		{
			// The sender is the SslStream, not the request, so the callback is not given one.
			handler.SslOptions.RemoteCertificateValidationCallback = (_, certificate, chain, errors)
				=> callback(AsCertificate2(certificate), chain, errors);
		}

		return handler;
	}

	/// <summary>TLS hands over an <see cref="X509Certificate2"/>; anything else is loaded as one.</summary>
	internal static X509Certificate2? AsCertificate2(X509Certificate? certificate)
		=> certificate switch
		{
			null => null,
			X509Certificate2 certificate2 => certificate2,
			_ => X509CertificateLoader.LoadCertificate(certificate.GetRawCertData())
		};

	private static Func<X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? PinnedCallback(string? pinned)
		=> pinned is null
			? null
			: (certificate, _, errors) => errors == SslPolicyErrors.None
				|| (certificate is not null
					&& string.Equals(certificate.GetCertHashString(HashAlgorithmName.SHA256), pinned, StringComparison.OrdinalIgnoreCase));

	internal static string? NormalizeThumbprint(string? thumbprint)
	{
		if (string.IsNullOrWhiteSpace(thumbprint))
		{
			return null;
		}

		var hex = new string([.. thumbprint.Where(Uri.IsHexDigit)]);
		return hex.Length == 64
			? hex
			: throw new ArgumentException("TrustedServerCertificateThumbprint must be a SHA-256 thumbprint (64 hex digits).", nameof(thumbprint));
	}
}
