using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>A role as a user or an SSO security group mapping lists it: <c>{"id", "name", "type"}</c>.</summary>
public sealed class UserRoleReference : ThreatStreamObject
{
	/// <summary>The role's ID (<c>id</c>).</summary>
	[JsonPropertyName("id")]
	public long Id { get; init; }

	/// <summary>The role's name (<c>name</c>), such as <c>Organization Administrator</c>.</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>Whether the role is custom or a system role (<c>type</c>).</summary>
	[JsonPropertyName("type")]
	public UserRoleType? Type { get; init; }
}
