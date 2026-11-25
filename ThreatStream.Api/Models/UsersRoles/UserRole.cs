using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>
/// A role on the Anomali platform (<c>api/v1/user_role/</c>): a named set of permissions assigned to users. Its
/// Security Analytics settings (<c>match_filter</c>, <c>max_search_back</c>; always <see langword="null"/> without that
/// subscription) are kept in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </summary>
public sealed class UserRole : ThreatStreamResource
{
	/// <summary>The role's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>Whether the role is custom or a system role (<c>type</c>).</summary>
	[JsonPropertyName("type")]
	public UserRoleType? Type { get; init; }

	/// <summary>The permissions the role grants (<c>permissions</c>), such as <c>can_approve_intel</c>.</summary>
	[JsonPropertyName("permissions")]
	public IReadOnlyList<string> Permissions { get; init; } = [];

	/// <summary>When the role was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the role was last modified (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
