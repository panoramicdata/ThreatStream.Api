using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>The kind of a role on the Anomali platform (<c>type</c>).</summary>
public enum UserRoleType
{
	/// <summary>A kind this library does not know.</summary>
	Unknown = 0,

	/// <summary>A custom role an Org Admin created (<c>user</c>).</summary>
	[JsonStringEnumMemberName("user")]
	User = 1,

	/// <summary>A system (out-of-the-box) role, which cannot be deleted (<c>legacy</c>).</summary>
	[JsonStringEnumMemberName("legacy")]
	Legacy = 2
}
