using Refit;
using System.Text.Json;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Role-based access control on the Anomali platform (<c>api/v1/user_role/</c>). Requires the Users &amp; Roles
/// Management feature to be enabled for the organisation; changes require Org Admin.
/// </summary>
/// <remarks>
/// None of these endpoints has a documented rate limit, so ThreatStream's default applies: one request per minute per
/// user. Users and their roles are listed and created through <c>api/v1/orgadmin/</c>
/// (<c>ThreatStreamClient.OrgAdminUsers</c>).
/// </remarks>
public interface IUserRoles
{
	/// <summary>Lists system and custom roles (<c>GET api/v1/user_role/</c>), optionally only one kind (<c>type</c>).</summary>
	/// <param name="options">The kind of role, paging and ordering, or <see langword="null"/> for every role.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of roles.</returns>
	[Get("api/v1/user_role/")]
	Task<ThreatStreamPage<UserRole>> ListAsync([Query] UserRoleListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Creates a custom role (<c>POST api/v1/user_role/</c>). Org Admin only. <b>Side effect:</b> the role is added to the
	/// organisation.
	/// </summary>
	/// <param name="request">The role's name and permissions.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The new role.</returns>
	/// <remarks>
	/// The reference's curl example for this operation posts to <c>/api/v1/orgadmin/</c>; its request table and the
	/// rest of the page use <c>/api/v1/user_role/</c>, which is what this method sends.
	/// </remarks>
	[Post("api/v1/user_role/")]
	Task<UserRole> CreateAsync([Body] UserRoleCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Lists the permissions that can be put in a custom role (<c>GET api/v1/user_role/permissions/</c>).</summary>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The response as ThreatStream sends it.</returns>
	/// <remarks>
	/// The reference names the permissions (<c>can_approve_intel</c>, <c>can_audit</c>, <c>can_change_rules</c>,
	/// <c>can_import_to_taxii_inbox</c>, <c>can_share_intelligence</c>, <c>can_submit_sandbox</c>, <c>can_use_match</c>,
	/// <c>is_org_admin</c>, <c>is_tfa_exempt</c>; the reference spells the sandbox one <c>can_submit sandbox</c>) but not
	/// the shape of this response, so it is returned unparsed. The
	/// reference writes the path without a trailing slash; this method adds one, as Django expects.
	/// </remarks>
	[Get("api/v1/user_role/permissions/")]
	Task<JsonElement> GetPermissionsAsync(CancellationToken cancellationToken);

	/// <summary>Changes a custom role's name or permissions (<c>PATCH api/v1/user_role/{id}/</c>). <b>Side effect:</b> every user with the role is affected.</summary>
	/// <param name="id">The role's ID.</param>
	/// <param name="request">The changes.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The updated role.</returns>
	[Patch("api/v1/user_role/{id}/")]
	Task<UserRole> UpdateAsync(long id, [Body] UserRoleUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Deletes a custom role (<c>DELETE api/v1/user_role/{id}/</c>). Org Admin only; system roles cannot be deleted.</summary>
	/// <param name="id">The role's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when the role is deleted.</returns>
	/// <remarks>The reference's request table omits the trailing slash; its curl example, which this follows, has it.</remarks>
	[Delete("api/v1/user_role/{id}/")]
	Task DeleteAsync(long id, CancellationToken cancellationToken);

	/// <summary>
	/// Assigns roles to users (<c>POST api/v1/user_role/users_set/</c>). Org Admin only. <b>Side effect:</b> changes what
	/// the users may do.
	/// </summary>
	/// <param name="request">The users and the roles to assign them.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when the roles are assigned.</returns>
	/// <remarks>The reference does not document the response, so it is not read.</remarks>
	[Post("api/v1/user_role/users_set/")]
	Task AssignAsync([Body] UserRoleAssignmentRequest request, CancellationToken cancellationToken);
}
