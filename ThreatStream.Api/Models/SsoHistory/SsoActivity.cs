using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.SsoHistory;

/// <summary>One entry of your organisation's single sign-on activity log (<c>api/v1/ssohistory/</c>).</summary>
public sealed class SsoActivity : ThreatStreamResource
{
	/// <summary>
	/// What happened, such as <c>Login Attempt</c>, <c>Login Success</c>, <c>Login Fail</c>, <c>SSO Configuration Changed</c>,
	/// <c>Permission Map Updated</c> or <c>Break-glass Account</c> (<c>action</c>).
	/// </summary>
	[JsonPropertyName("action")]
	public string? Action { get; init; }

	/// <summary>The deployment using single sign-on: <c>Cloud</c> or <c>OnPrem</c> (<c>deployment_type</c>).</summary>
	[JsonPropertyName("deployment_type")]
	public string? DeploymentType { get; init; }

	/// <summary>The name configured for the identity provider (<c>idp</c>).</summary>
	[JsonPropertyName("idp")]
	public string? Idp { get; init; }

	/// <summary>The email address of the ThreatStream user concerned (<c>user_email</c>).</summary>
	[JsonPropertyName("user_email")]
	public string? UserEmail { get; init; }

	/// <summary>When it happened (<c>ts</c>).</summary>
	[JsonPropertyName("ts")]
	public DateTimeOffset? Ts { get; init; }
}
