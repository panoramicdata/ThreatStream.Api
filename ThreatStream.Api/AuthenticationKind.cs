namespace ThreatStream.Api;

/// <summary>How the client authenticates, derived from <see cref="ThreatStreamClientOptions"/>.</summary>
internal enum AuthenticationKind
{
	/// <summary><c>Authorization: apikey &lt;username&gt;:&lt;api_key&gt;</c> on every request.</summary>
	ApiKey,

	/// <summary>An OAuth 2.0 client-credentials token from <c>oauth/token</c>, sent as <c>Authorization: Bearer</c>.</summary>
	OAuthClientCredentials
}
