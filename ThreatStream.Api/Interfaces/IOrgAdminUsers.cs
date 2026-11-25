using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UserAdministration;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// User administration (<c>api/v1/orgadmin/</c>): list, add, change, unlock and delete the organisation's users. Every
/// operation requires Org Admin.
/// </summary>
/// <remarks>
/// None of these endpoints has a documented rate limit, so ThreatStream's default applies: one request per minute per
/// user. The reference writes several paths without a trailing slash; its curl examples, which these methods follow,
/// end them in one.
/// </remarks>
public interface IOrgAdminUsers
{
	/// <summary>Lists the organisation's users and their roles (<c>GET api/v1/orgadmin/</c>).</summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of users.</returns>
	[Get("api/v1/orgadmin/")]
	Task<ThreatStreamPage<OrganizationUser>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Adds a user to the organisation (<c>POST api/v1/orgadmin/</c>), with privileges or, with the Users &amp; Roles
	/// Management feature, roles. <b>Side effect:</b> creates an account and invites the user.
	/// </summary>
	/// <param name="request">The user's email address and what to grant.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The new user, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Post("api/v1/orgadmin/")]
	Task<OrganizationUser?> CreateAsync([Body] OrganizationUserCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets one user (<c>GET api/v1/orgadmin/{user_id}/</c>).</summary>
	/// <param name="userId">The user's ID (<see cref="OrganizationUser.UserId"/>).</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The user.</returns>
	[Get("api/v1/orgadmin/{userId}/")]
	Task<OrganizationUser> GetAsync(long userId, CancellationToken cancellationToken);

	/// <summary>
	/// Changes a user's privileges or roles, or forces a password change (<c>PATCH api/v1/orgadmin/{user_id}/</c>).
	/// <b>Side effect:</b> changes what the user may do; making a user Read Only removes their sharing, TAXII import,
	/// approval and Org Admin privileges.
	/// </summary>
	/// <param name="userId">The user's ID.</param>
	/// <param name="request">The changes.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The updated user, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Patch("api/v1/orgadmin/{userId}/")]
	Task<OrganizationUser?> UpdateAsync(long userId, [Body] OrganizationUserUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Deletes a user from the organisation (<c>DELETE api/v1/orgadmin/{user_id}/</c>). <b>Side effect:</b> the account is removed.</summary>
	/// <param name="userId">The user's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when the user is deleted.</returns>
	[Delete("api/v1/orgadmin/{userId}/")]
	Task DeleteAsync(long userId, CancellationToken cancellationToken);

	/// <summary>
	/// Unlocks an account locked after successive failed sign-ins (<c>PATCH api/v1/orgadmin/{user_id}/unlock/</c>, with no
	/// body). <b>Side effect:</b> the user can sign in again.
	/// </summary>
	/// <param name="userId">The user's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when the account is unlocked.</returns>
	/// <remarks>The reference does not document the response, so it is not read.</remarks>
	[Patch("api/v1/orgadmin/{userId}/unlock/")]
	Task UnlockAsync(long userId, CancellationToken cancellationToken);
}
