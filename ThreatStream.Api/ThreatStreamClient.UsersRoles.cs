using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Roles and their assignment to users (<c>api/v1/user_role/</c>).</summary>
	public IUserRoles UserRoles => field ??= For<IUserRoles>();

	/// <summary>SSO security group mappings (<c>api/v1/ssopermission/</c>).</summary>
	public ISsoPermissions SsoPermissions => field ??= For<ISsoPermissions>();

	/// <summary>SSO identity providers (<c>api/v1/identity-provider/</c>).</summary>
	public IIdentityProviders IdentityProviders => field ??= For<IIdentityProviders>();
}
