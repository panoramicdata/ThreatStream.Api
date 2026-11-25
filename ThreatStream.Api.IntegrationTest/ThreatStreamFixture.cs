using Microsoft.Extensions.Configuration;

namespace ThreatStream.Api.IntegrationTest;

/// <summary>
/// A read-only <see cref="ThreatStreamClient"/> for a live ThreatStream deployment, configured from user secrets or
/// environment variables.
/// </summary>
/// <remarks>
/// Requires <c>ThreatStream:BaseUrl</c> (<c>https://api.threatstream.com/</c> for the US cloud,
/// <c>https://api-eu.threatstream.com/</c> for the EU cloud, or an on-premises URL), <c>ThreatStream:Username</c> and
/// <c>ThreatStream:ApiKey</c>; <c>ThreatStream:TrustedServerCertificateThumbprint</c> (SHA-256) trusts an on-premises
/// self-signed certificate. Set them with, for example,
/// <c>dotnet user-secrets set ThreatStream:Username analyst@example.com --project ThreatStream.Api.IntegrationTest</c>, or as
/// <c>ThreatStream__Username</c> etc. in the environment. A missing setting fails the tests with a message naming it;
/// nothing is skipped.
/// </remarks>
public sealed class ThreatStreamFixture : IDisposable
{
	/// <summary>The prefix of every object the integration tests create, so leftovers are recognisable.</summary>
	internal const string Prefix = "threatstream_api_it_";

	private readonly Lazy<ThreatStreamClient> _client;

	public ThreatStreamFixture()
	{
		Configuration = new ConfigurationBuilder()
			.AddUserSecrets<ThreatStreamFixture>()
			.AddEnvironmentVariables()
			.Build();
		_client = new Lazy<ThreatStreamClient>(() => new ThreatStreamClient(CreateOptions()));
	}

	public IConfiguration Configuration { get; }

	/// <summary>A client authenticated as configured, with <see cref="ThreatStreamClientOptions.IsReadOnly"/> on.</summary>
	public ThreatStreamClient Client => _client.Value;

	/// <summary>Builds read-only options from configuration, failing loudly when a required setting is missing.</summary>
	public ThreatStreamClientOptions CreateOptions() => CreateOptions(static _ => { });

	/// <summary>
	/// Builds read-only options from configuration, then lets <paramref name="tweak"/> change them. Turning
	/// <see cref="ThreatStreamClientOptions.IsReadOnly"/> off is reserved for tests that create objects named with
	/// <see cref="UniqueName(string)"/> and delete them again.
	/// </summary>
	public ThreatStreamClientOptions CreateOptions(Action<ThreatStreamClientOptions> tweak)
	{
		var baseUrl = Required("ThreatStream:BaseUrl");
		var region = RegionOf(baseUrl);
		var options = new ThreatStreamClientOptions
		{
			Region = region,
			BaseUrl = region == ThreatStreamRegion.Custom ? baseUrl : null,
			Username = Required("ThreatStream:Username"),
			ApiKey = Required("ThreatStream:ApiKey"),
			TrustedServerCertificateThumbprint = Configuration["ThreatStream:TrustedServerCertificateThumbprint"],
			IsReadOnly = true,
			// Endpoints without a documented rate limit allow one request per minute per user: wait for the window.
			MaxRetries = 2,
			MaxRetryDelay = TimeSpan.FromSeconds(75)
		};
		tweak(options);
		return options;
	}

	/// <summary>A unique, recognisable name for an object a test creates.</summary>
	public static string UniqueName(string what) => $"{Prefix}{what}_{Guid.NewGuid():N}"[..Math.Min(60, Prefix.Length + what.Length + 33)];

	private static ThreatStreamRegion RegionOf(string baseUrl)
	{
		var normalized = baseUrl.TrimEnd('/') + "/";
		if (normalized.Equals(ThreatStreamClientOptions.UsBaseUrl, StringComparison.OrdinalIgnoreCase))
		{
			return ThreatStreamRegion.Us;
		}

		return normalized.Equals(ThreatStreamClientOptions.EuBaseUrl, StringComparison.OrdinalIgnoreCase)
			? ThreatStreamRegion.Eu
			: ThreatStreamRegion.Custom;
	}

	private string Required(string key)
		=> Configuration[key] is { Length: > 0 } value
			? value
			: throw new InvalidOperationException(
				$"Integration tests need '{key}'. Set it with 'dotnet user-secrets set {key} <value> --project ThreatStream.Api.IntegrationTest', "
				+ $"or as the environment variable {key.Replace(":", "__", StringComparison.Ordinal)}. See docs/TESTING.md.");

	public void Dispose()
	{
		if (_client.IsValueCreated)
		{
			_client.Value.Dispose();
		}
	}
}
