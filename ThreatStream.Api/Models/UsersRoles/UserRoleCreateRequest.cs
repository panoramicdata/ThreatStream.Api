using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>A new custom role (<c>POST api/v1/user_role/</c>).</summary>
public sealed class UserRoleCreateRequest
{
	/// <summary>The role's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	/// <summary>
	/// The permissions the role grants (<c>permissions</c>), from <c>GET api/v1/user_role/permissions/</c>: for example
	/// <c>can_approve_intel</c>, <c>can_audit</c>, <c>can_change_rules</c>, <c>can_import_to_taxii_inbox</c>,
	/// <c>can_share_intelligence</c>, <c>can_use_match</c>, <c>is_org_admin</c> or <c>is_tfa_exempt</c>.
	/// <c>is_readonly</c>, <c>can_use_chat</c> and <c>can_see_api_key</c> cannot be in a custom role; assign the matching
	/// system role instead.
	/// </summary>
	[JsonPropertyName("permissions")]
	public required IReadOnlyList<string> Permissions { get; init; }
}
