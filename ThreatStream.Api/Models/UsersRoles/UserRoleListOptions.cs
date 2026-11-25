using Refit;

namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>Filters for listing roles (<c>GET api/v1/user_role/</c>).</summary>
public sealed class UserRoleListOptions : ListOptions
{
	/// <summary>Only custom (<see cref="UserRoleType.User"/>) or only system (<see cref="UserRoleType.Legacy"/>) roles (<c>type</c>); all roles when <see langword="null"/>.</summary>
	[AliasAs("type")]
	public UserRoleType? Type { get; set; }
}
