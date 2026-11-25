using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>A new SSO security group mapping (<c>POST api/v1/ssopermission/</c>).</summary>
public sealed class SsoGroupMappingCreateRequest
{
	/// <summary>The security group's name in the identity provider (<c>group</c>), such as an Active Directory or Entra ID group.</summary>
	[JsonPropertyName("group")]
	public required string Group { get; init; }

	/// <summary>The identity provider's ID (<c>idp_id</c>), from <c>GET api/v1/identity-provider/</c>.</summary>
	[JsonPropertyName("idp_id")]
	public required long IdpId { get; init; }

	/// <summary>The IDs of the roles to map to the group (<c>roles</c>).</summary>
	[JsonPropertyName("roles")]
	public required IReadOnlyList<long> Roles { get; init; }
}
