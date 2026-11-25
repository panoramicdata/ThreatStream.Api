using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>An SSO security group mapping (<c>api/v1/ssopermission/</c>): the roles granted to members of an identity provider's group.</summary>
public sealed class SsoGroupMapping : ThreatStreamResource
{
	/// <summary>The security group's name or UUID in the identity provider (<c>group</c>).</summary>
	[JsonPropertyName("group")]
	public string? Group { get; init; }

	/// <summary>The identity provider's API path (<c>identity_provider</c>), such as <c>/api/v1/identity-provider/112/</c>.</summary>
	[JsonPropertyName("identity_provider")]
	public string? IdentityProvider { get; init; }

	/// <summary>The identity provider's ID (<c>idp_id</c>).</summary>
	[JsonPropertyName("idp_id")]
	public long? IdpId { get; init; }

	/// <summary>The roles mapped to the group (<c>roles</c>).</summary>
	[JsonPropertyName("roles")]
	public IReadOnlyList<UserRoleReference> Roles { get; init; } = [];
}
