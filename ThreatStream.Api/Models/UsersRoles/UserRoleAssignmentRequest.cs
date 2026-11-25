using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>Roles to assign to users (<c>POST api/v1/user_role/users_set/</c>).</summary>
public sealed class UserRoleAssignmentRequest
{
	/// <summary>The IDs of the users (<c>users</c>), as <c>user_id</c> in <c>GET api/v1/orgadmin/</c>.</summary>
	[JsonPropertyName("users")]
	public required IReadOnlyList<long> Users { get; init; }

	/// <summary>The IDs of the roles to assign (<c>roles</c>).</summary>
	[JsonPropertyName("roles")]
	public required IReadOnlyList<long> Roles { get; init; }
}
