using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.Interfaces;

/// <summary>SSO identity providers configured for the organisation (<c>api/v1/identity-provider/</c>).</summary>
public interface IIdentityProviders
{
	/// <summary>Lists the identity providers (<c>GET api/v1/identity-provider/</c>), whose IDs SSO security group mappings use.</summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of identity providers.</returns>
	/// <remarks>
	/// The reference mentions this endpoint only in notes on <see cref="ISsoPermissions"/>; the usual
	/// <c>{"meta", "objects"}</c> list shape is assumed. No documented rate limit, so ThreatStream's default applies: one
	/// request per minute per user.
	/// </remarks>
	[Get("api/v1/identity-provider/")]
	Task<ThreatStreamPage<IdentityProvider>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
