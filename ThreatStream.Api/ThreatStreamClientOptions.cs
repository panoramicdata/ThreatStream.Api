using Microsoft.Extensions.Logging;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ThreatStream.Api;

/// <summary>
/// Configuration for <see cref="ThreatStreamClient"/>. The values are read once when the client is constructed; changing
/// this object afterwards does not affect an existing client.
/// </summary>
/// <remarks>
/// <para>
/// Choose the deployment with <see cref="Region"/>: the US cloud (the default), the EU cloud, or
/// <see cref="ThreatStreamRegion.Custom"/> with a <see cref="BaseUrl"/> for an on-premises (OnPrem) deployment.
/// </para>
/// <para>
/// Supply exactly one way to authenticate: a <see cref="Username"/> and <see cref="ApiKey"/>, sent as
/// <c>Authorization: apikey &lt;username&gt;:&lt;api_key&gt;</c> (ThreatStream's recommended method), or an OAuth 2.0
/// <see cref="ClientId"/> and <see cref="ClientSecret"/>, exchanged at <c>oauth/token</c> for a bearer token that is
/// renewed before it expires. Credentials never travel in the URL.
/// </para>
/// </remarks>
public sealed class ThreatStreamClientOptions
{
	/// <summary>The base URL of the US cloud.</summary>
	public const string UsBaseUrl = "https://api.threatstream.com/";

	/// <summary>The base URL of the EU cloud.</summary>
	public const string EuBaseUrl = "https://api-eu.threatstream.com/";

	private static readonly TimeSpan MaxTimerDuration = TimeSpan.FromMilliseconds(int.MaxValue);

	/// <summary>The deployment to use. Defaults to <see cref="ThreatStreamRegion.Us"/>.</summary>
	public ThreatStreamRegion Region { get; set; } = ThreatStreamRegion.Us;

	/// <summary>
	/// The absolute base URL of a <see cref="ThreatStreamRegion.Custom"/> deployment, such as
	/// <c>https://threatstream.example.com/</c>; endpoint paths (<c>api/v1/...</c>, <c>api/v2/...</c>) are appended to it, so
	/// a path prefix is kept. Leave it empty for <see cref="ThreatStreamRegion.Us"/> and <see cref="ThreatStreamRegion.Eu"/>.
	/// It must not contain credentials, a query string or a fragment.
	/// </summary>
	public string? BaseUrl { get; set; }

	/// <summary>The ThreatStream user name (the account's email address) for API key authentication. Requires <see cref="ApiKey"/>.</summary>
	public string? Username { get; set; }

	/// <summary>The API key of <see cref="Username"/> (from the My Account tab).</summary>
	public string? ApiKey { get; set; }

	/// <summary>The OAuth 2.0 client ID for client-credentials authentication. Requires <see cref="ClientSecret"/>.</summary>
	public string? ClientId { get; set; }

	/// <summary>The OAuth 2.0 client secret of <see cref="ClientId"/>.</summary>
	public string? ClientSecret { get; set; }

	/// <summary>
	/// When <see langword="true"/>, the client refuses (with <see cref="ThreatStreamReadOnlyException"/>, before anything
	/// is sent) every request that could change ThreatStream: any method other than GET and HEAD, except POSTs to this
	/// allow-list of read-only operations:
	/// <list type="bullet">
	/// <item><c>oauth/token</c> (obtaining an OAuth 2.0 token);</item>
	/// <item><c>api/v2/intelligence/</c> (an observable query too long for a URL, over 4000 characters, is POSTed);</item>
	/// <item><c>api/v1/threat_model_search/</c> (likewise, for a long threat model search);</item>
	/// <item><c>api/v1/integration_package/direct/</c> (a Recorded Future enrichment lookup, which ThreatStream documents
	/// as read-only).</item>
	/// </list>
	/// Each is matched exactly (trailing slash optional), and only for POST: <c>PATCH api/v2/intelligence/</c> (import
	/// without approval) and every sub-path (<c>import/</c>, <c>bulk_tagging/</c>, <c>{id}/clone/</c>...) are refused.
	/// </summary>
	/// <remarks>
	/// GET operations that start a server-side job are refused too: generating a PDF report
	/// (<c>api/v1/alfred/report/</c>) and any request with <c>async_result=true</c> (user audit reports). Use a read-only
	/// ThreatStream user for a truly read-only identity.
	/// </remarks>
	public bool IsReadOnly { get; set; }

	/// <summary>
	/// The SHA-256 thumbprint (hex, case and separators ignored) of a server certificate to trust even when it fails
	/// normal validation, such as an on-premises deployment's self-signed certificate. Other certificates are still
	/// validated normally. Ignored when <see cref="ServerCertificateValidationCallback"/> is set, and when an inner
	/// <see cref="HttpMessageHandler"/> is supplied to the client.
	/// </summary>
	public string? TrustedServerCertificateThumbprint { get; set; }

	/// <summary>
	/// Full control over server certificate validation. Takes precedence over <see cref="TrustedServerCertificateThumbprint"/>.
	/// Ignored when an inner <see cref="HttpMessageHandler"/> is supplied to the client.
	/// </summary>
	public Func<X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? ServerCertificateValidationCallback { get; set; }

	/// <summary>
	/// HTTP timeout per attempt, covering sending the request and receiving the response headers. It does not include
	/// retry back-off, nor reading a streamed body after the headers arrive. An attempt that exceeds it raises a
	/// <see cref="TimeoutException"/>; caller cancellation still raises <see cref="OperationCanceledException"/>. Must be
	/// greater than zero and at most <see cref="int.MaxValue"/> milliseconds (about 24.8 days).
	/// </summary>
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

	/// <summary>
	/// Maximum retries of a transient failure. Any verb is retried on 429 (rate limited) and 503; other 5xx responses are
	/// retried only for idempotent verbs (GET, HEAD, PUT, DELETE), never POST or PATCH. A connection that could not be
	/// established (refused, reset during the TLS handshake, or a name that did not resolve) is retried for any verb, since
	/// nothing was sent. Requests with a stream body are never retried.
	/// </summary>
	public int MaxRetries { get; set; } = 3;

	/// <summary>Initial back-off, doubled on each retry (up to <see cref="MaxRetryDelay"/>). Must not be negative.</summary>
	public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(1);

	/// <summary>
	/// The longest single wait before a retry. A 429 or 503 whose <c>Retry-After</c> asks for longer is not retried: it
	/// raises <see cref="ThreatStreamApiException"/> at once, carrying <see cref="ThreatStreamApiException.RetryAfter"/>.
	/// Defaults to 60 seconds, which covers ThreatStream's one-request-per-minute limit on less used endpoints. Must be
	/// greater than zero and at most <see cref="int.MaxValue"/> milliseconds.
	/// </summary>
	public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(60);

	/// <summary>
	/// Optional logger. Credentials, tokens and query strings (which can carry observable values) are never logged; only
	/// the method and path are.
	/// </summary>
	public ILogger? Logger { get; set; }

	/// <summary>The clock used to decide when an OAuth token must be renewed (replaced in tests).</summary>
	internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

	internal AuthenticationKind AuthenticationKind
		=> string.IsNullOrWhiteSpace(ClientId) ? AuthenticationKind.ApiKey : AuthenticationKind.OAuthClientCredentials;

	/// <summary>The base URL the client uses: the region's, or <see cref="BaseUrl"/> for a custom deployment.</summary>
	internal string EffectiveBaseUrl
		=> Region switch
		{
			ThreatStreamRegion.Us => UsBaseUrl,
			ThreatStreamRegion.Eu => EuBaseUrl,
			_ => BaseUrl ?? string.Empty
		};

	internal void Validate()
	{
		ValidateBaseUrl();
		ValidateCredentials();
		_ = ThreatStreamClient.NormalizeThumbprint(TrustedServerCertificateThumbprint);
		ArgumentOutOfRangeException.ThrowIfNegative(MaxRetries);
		// The upper bounds are those of CancellationTokenSource.CancelAfter and Task.Delay, which would otherwise throw on
		// the first request rather than here.
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(Timeout, TimeSpan.Zero);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(Timeout, MaxTimerDuration);
		ArgumentOutOfRangeException.ThrowIfLessThan(RetryBaseDelay, TimeSpan.Zero);
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(MaxRetryDelay, TimeSpan.Zero);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(MaxRetryDelay, MaxTimerDuration);
	}

	private void ValidateBaseUrl()
	{
		if (!Enum.IsDefined(Region))
		{
			throw new ArgumentOutOfRangeException(nameof(Region), Region, "Region must be Us, Eu or Custom.");
		}

		if (Region != ThreatStreamRegion.Custom)
		{
			if (!string.IsNullOrWhiteSpace(BaseUrl))
			{
				throw new ArgumentException("BaseUrl is only used with Region = Custom; leave it empty for the Us and Eu clouds.", nameof(BaseUrl));
			}

			return;
		}

		// Absolute alone is not enough: on Linux a rooted path such as "/relative/path" parses as a file:// URI.
		if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var baseUri)
			|| (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
		{
			throw new ArgumentException("BaseUrl must be an absolute http or https URL when Region is Custom.", nameof(BaseUrl));
		}

		// HttpClient never sends user info, and it would appear in logs and exception messages that show the URL.
		if (baseUri.UserInfo.Length > 0)
		{
			throw new ArgumentException("BaseUrl must not contain credentials: set Username and ApiKey, or ClientId and ClientSecret.", nameof(BaseUrl));
		}

		// Endpoint paths are appended to BaseUrl, which a query string or fragment would swallow.
		if (baseUri.Query.Length > 0 || baseUri.Fragment.Length > 0)
		{
			throw new ArgumentException("BaseUrl must not have a query string or fragment.", nameof(BaseUrl));
		}
	}

	private void ValidateCredentials()
	{
		var hasApiKey = !string.IsNullOrWhiteSpace(Username) || !string.IsNullOrWhiteSpace(ApiKey);
		var hasOAuth = !string.IsNullOrWhiteSpace(ClientId) || !string.IsNullOrWhiteSpace(ClientSecret);
		if (hasApiKey && hasOAuth)
		{
			throw new ArgumentException("Set either Username and ApiKey, or ClientId and ClientSecret, not both.", nameof(ClientId));
		}

		if (hasOAuth)
		{
			RequireBoth(ClientId, nameof(ClientId), ClientSecret, nameof(ClientSecret));
			return;
		}

		if (!hasApiKey)
		{
			throw new ArgumentException("Set Username and ApiKey, or ClientId and ClientSecret.", nameof(Username));
		}

		RequireBoth(Username, nameof(Username), ApiKey, nameof(ApiKey));
	}

	private static void RequireBoth(string? first, string firstName, string? second, string secondName)
	{
		if (string.IsNullOrWhiteSpace(first))
		{
			throw new ArgumentException($"{firstName} is required with {secondName}.", firstName);
		}

		if (string.IsNullOrWhiteSpace(second))
		{
			throw new ArgumentException($"{secondName} is required with {firstName}.", secondName);
		}
	}

	/// <inheritdoc />
	public override string ToString()
		=> $"ThreatStreamClientOptions {{ Region = {Region}, BaseUrl = {MaskUserInfo(BaseUrl)}, Username = {Username}, ApiKey = {Mask(ApiKey)}, ClientId = {ClientId}, ClientSecret = {Mask(ClientSecret)}, IsReadOnly = {IsReadOnly} }}";

	private static string Mask(string? secret) => string.IsNullOrEmpty(secret) ? "(none)" : "***";

	/// <summary>
	/// Masks any <c>user:password@</c> in a URL. Done on the text rather than a parsed <see cref="Uri"/>, since this must
	/// also redact a URL that fails to parse (such as one whose password contains an unescaped <c>@</c>).
	/// </summary>
	private static string MaskUserInfo(string? url)
	{
		if (url is null)
		{
			return string.Empty;
		}

		var start = url.IndexOf("://", StringComparison.Ordinal);
		if (start < 0)
		{
			return url;
		}

		start += 3;
		var end = url.IndexOfAny(['/', '?', '#'], start);
		var authority = end < 0 ? url[start..] : url[start..end];
		var at = authority.LastIndexOf('@');
		return at < 0 ? url : $"{url[..start]}***{url[(start + at)..]}";
	}
}
