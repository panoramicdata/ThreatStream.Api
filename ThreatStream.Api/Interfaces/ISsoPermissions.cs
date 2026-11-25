using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// SSO security group mappings (<c>api/v1/ssopermission/</c>): the roles granted to members of an identity provider's
/// groups. Requires the Users &amp; Roles Management feature; changes require Org Admin.
/// </summary>
/// <remarks>
/// None of these endpoints has a documented rate limit, so ThreatStream's default applies: one request per minute per
/// user. The reference's curl example for mapping a group sends a PATCH to <c>/api/v1/orgadmin/</c>; its request table,
/// which these methods follow, creates with <c>POST api/v1/ssopermission/</c> and updates with
/// <c>PATCH api/v1/ssopermission/{id}/</c>.
/// </remarks>
public interface ISsoPermissions
{
	/// <summary>Lists the SSO security group mappings of one identity provider (<c>GET api/v1/ssopermission/?identity_provider_id=</c>).</summary>
	/// <param name="identityProviderId">The identity provider's ID, from <see cref="IIdentityProviders.ListAsync"/>.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of mappings.</returns>
	[Get("api/v1/ssopermission/")]
	Task<ThreatStreamPage<SsoGroupMapping>> ListAsync([AliasAs("identity_provider_id")] long identityProviderId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Maps a security group to roles (<c>POST api/v1/ssopermission/</c>). <b>Side effect:</b> the group's members gain the roles at their next sign-in.</summary>
	/// <param name="request">The group, identity provider and roles.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The new mapping.</returns>
	[Post("api/v1/ssopermission/")]
	Task<SsoGroupMapping> CreateAsync([Body] SsoGroupMappingCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Changes a mapping (<c>PATCH api/v1/ssopermission/{id}/</c>). <b>Side effect:</b> changes what the group's members may do.</summary>
	/// <param name="id">The mapping's ID.</param>
	/// <param name="request">The changes; the identity provider's ID is always required.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The updated mapping.</returns>
	[Patch("api/v1/ssopermission/{id}/")]
	Task<SsoGroupMapping> UpdateAsync(long id, [Body] SsoGroupMappingUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Deletes a mapping (<c>DELETE api/v1/ssopermission/{id}/</c>). <b>Side effect:</b> the group's members lose the mapped roles.</summary>
	/// <param name="id">The mapping's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when the mapping is deleted.</returns>
	[Delete("api/v1/ssopermission/{id}/")]
	Task DeleteAsync(long id, CancellationToken cancellationToken);
}
