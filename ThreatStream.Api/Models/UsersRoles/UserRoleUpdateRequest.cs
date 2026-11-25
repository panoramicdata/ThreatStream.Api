using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>Changes to a custom role (<c>PATCH api/v1/user_role/{id}/</c>); properties left <see langword="null"/> are not sent.</summary>
public sealed class UserRoleUpdateRequest
{
	/// <summary>The role's new name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The complete new set of permissions (<c>permissions</c>); see <see cref="UserRoleCreateRequest.Permissions"/>.</summary>
	[JsonPropertyName("permissions")]
	public IReadOnlyList<string>? Permissions { get; init; }
}
