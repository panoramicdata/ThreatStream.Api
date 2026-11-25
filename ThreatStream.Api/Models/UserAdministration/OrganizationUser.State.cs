using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UserAdministration;

public sealed partial class OrganizationUser
{
	/// <summary>Whether the account is active (<c>is_active</c>).</summary>
	[JsonPropertyName("is_active")]
	public bool? IsActive { get; init; }

	/// <summary>Whether the account is locked, for example after successive failed sign-ins (<c>is_locked</c>).</summary>
	[JsonPropertyName("is_locked")]
	public bool? IsLocked { get; init; }

	/// <summary>Whether the user must change their password at their next sign-in (<c>must_change_password</c>).</summary>
	[JsonPropertyName("must_change_password")]
	public bool? MustChangePassword { get; init; }

	/// <summary>When the user was added to ThreatStream (<c>date_joined</c>).</summary>
	[JsonPropertyName("date_joined")]
	public DateTimeOffset? DateJoined { get; init; }

	/// <summary>When the user last changed their password (<c>date_password_changed</c>).</summary>
	[JsonPropertyName("date_password_changed")]
	public DateTimeOffset? DatePasswordChanged { get; init; }

	/// <summary>When the user will next be forced to change their password (<c>next_password_change_ts</c>).</summary>
	[JsonPropertyName("next_password_change_ts")]
	public DateTimeOffset? NextPasswordChangeTs { get; init; }

	/// <summary>When the user last accessed ThreatStream (<c>last_access_ts</c>).</summary>
	[JsonPropertyName("last_access_ts")]
	public DateTimeOffset? LastAccessTs { get; init; }

	/// <summary>When the user last signed in (<c>last_login</c>).</summary>
	[JsonPropertyName("last_login")]
	public DateTimeOffset? LastLogin { get; init; }
}
