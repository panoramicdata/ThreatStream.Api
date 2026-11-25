using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>Changes to an SSO security group mapping (<c>PATCH api/v1/ssopermission/{id}/</c>); optional properties left <see langword="null"/> are not sent.</summary>
public sealed class SsoGroupMappingUpdateRequest
{
	/// <summary>The security group's new name (<c>group</c>).</summary>
	[JsonPropertyName("group")]
	public string? Group { get; init; }

	/// <summary>The identity provider's ID (<c>idp_id</c>), which ThreatStream requires on every update.</summary>
	[JsonPropertyName("idp_id")]
	public required long IdpId { get; init; }

	/// <summary>The IDs of the roles the group keeps (<c>roles</c>): the complete new set.</summary>
	[JsonPropertyName("roles")]
	public IReadOnlyList<long>? Roles { get; init; }
}
