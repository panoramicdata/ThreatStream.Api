using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UserAdministration;

/// <summary>
/// The privileges a request can grant or remove from a user (<c>api/v1/orgadmin/</c>); properties left
/// <see langword="null"/> are not sent.
/// </summary>
public abstract class OrganizationUserPrivileges
{
	/// <summary>Whether the user is an Org Admin (<c>is_org_admin</c>).</summary>
	[JsonPropertyName("is_org_admin")]
	public bool? IsOrgAdmin { get; init; }

	/// <summary>
	/// Whether the user is Read Only (<c>is_readonly</c>). Cannot be <see langword="true"/> together with
	/// <see cref="CanShareIntelligence"/>, <see cref="CanImportToTaxiiInbox"/>, <see cref="CanApproveIntel"/> or
	/// <see cref="IsOrgAdmin"/>; setting it on an existing user removes those privileges.
	/// </summary>
	[JsonPropertyName("is_readonly")]
	public bool? IsReadOnly { get; init; }

	/// <summary>Whether the user can approve imported intelligence (<c>can_approve_intel</c>).</summary>
	[JsonPropertyName("can_approve_intel")]
	public bool? CanApproveIntel { get; init; }

	/// <summary>Whether the user can create intelligence shared with the Anomali Community (<c>can_share_intelligence</c>).</summary>
	[JsonPropertyName("can_share_intelligence")]
	public bool? CanShareIntelligence { get; init; }

	/// <summary>Whether the user can see their own API key on the My Profile tab (<c>can_see_api_key</c>).</summary>
	[JsonPropertyName("can_see_api_key")]
	public bool? CanSeeApiKey { get; init; }

	/// <summary>Whether the user can push data from TAXII clients to the organisation's TAXII server (<c>can_import_to_taxii_inbox</c>).</summary>
	[JsonPropertyName("can_import_to_taxii_inbox")]
	public bool? CanImportToTaxiiInbox { get; init; }

	/// <summary>Whether the user can use ThreatStream Chat, which must be enabled for the organisation (<c>can_use_chat</c>).</summary>
	[JsonPropertyName("can_use_chat")]
	public bool? CanUseChat { get; init; }

	/// <summary>Whether the user is exempt from multi-factor authentication (<c>is_tfa_exempt</c>).</summary>
	[JsonPropertyName("is_tfa_exempt")]
	public bool? IsTfaExempt { get; init; }

	/// <summary>
	/// The IDs of the roles to assign (<c>roles</c>): the complete new set. Requires the Users &amp; Roles Management
	/// feature (see <see cref="Interfaces.IUserRoles"/>).
	/// </summary>
	[JsonPropertyName("roles")]
	public IReadOnlyList<long>? Roles { get; init; }
}
