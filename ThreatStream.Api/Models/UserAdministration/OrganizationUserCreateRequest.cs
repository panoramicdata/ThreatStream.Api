using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UserAdministration;

/// <summary>A new user in the organisation (<c>POST api/v1/orgadmin/</c>), with the privileges or roles to grant.</summary>
public sealed class OrganizationUserCreateRequest : OrganizationUserPrivileges
{
	/// <summary>The new user's email address (<c>email</c>), which is also their username.</summary>
	[JsonPropertyName("email")]
	public required string Email { get; init; }
}
