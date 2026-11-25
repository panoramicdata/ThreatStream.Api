using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UserAdministration;

/// <summary>Changes to a user (<c>PATCH api/v1/orgadmin/{user_id}/</c>); properties left <see langword="null"/> are not sent.</summary>
public sealed class OrganizationUserUpdateRequest : OrganizationUserPrivileges
{
	/// <summary>Whether the user must change their password at their next sign-in (<c>must_change_password</c>).</summary>
	[JsonPropertyName("must_change_password")]
	public bool? MustChangePassword { get; init; }
}
