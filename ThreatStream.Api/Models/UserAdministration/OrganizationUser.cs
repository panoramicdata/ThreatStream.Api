using System.Text.Json.Serialization;
using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.Models.UserAdministration;

/// <summary>
/// A user of the organisation as an Org Admin sees it (<c>api/v1/orgadmin/</c>): profile, account state, privileges and
/// roles. The ID is <see cref="UserId"/>; this resource has no <c>id</c> field.
/// </summary>
/// <remarks>
/// A user object can carry the user's API key (<see cref="ApiKey"/>), a secret. This type is a class, not a record, so
/// <see cref="object.ToString"/> prints only the type name; never log or serialize a user object wholesale.
/// </remarks>
public sealed partial class OrganizationUser : ThreatStreamObject
{
	/// <summary>The user's ID (<c>user_id</c>; ThreatStream sends it as a string), used in <c>api/v1/orgadmin/{user_id}/</c>.</summary>
	[JsonPropertyName("user_id")]
	public long UserId { get; init; }

	/// <summary>The user's own API path (<c>resource_uri</c>), such as <c>/api/v1/orgadmin/725/</c>.</summary>
	[JsonPropertyName("resource_uri")]
	public string? ResourceUri { get; init; }

	/// <summary>The user's email address (<c>email</c>).</summary>
	[JsonPropertyName("email")]
	public string? Email { get; init; }

	/// <summary>The name the user entered on the My Profile tab (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The user's nickname (<c>nickname</c>).</summary>
	[JsonPropertyName("nickname")]
	public string? Nickname { get; init; }

	/// <summary>The URL of the user's avatar image (<c>avatar_s3_url</c>); often empty.</summary>
	[JsonPropertyName("avatar_s3_url")]
	public string? AvatarS3Url { get; init; }

	/// <summary>
	/// <b>Secret.</b> The user's ThreatStream API key (<c>api_key</c>), when ThreatStream includes it (the reference does
	/// not document it). Anyone holding it can act as the user: never log, display or store it.
	/// </summary>
	[JsonPropertyName("api_key")]
	public string? ApiKey { get; init; }

	/// <summary>The roles assigned to the user (<c>roles</c>), with the Users &amp; Roles Management feature.</summary>
	[JsonPropertyName("roles")]
	public IReadOnlyList<UserRoleReference> Roles { get; init; } = [];
}
