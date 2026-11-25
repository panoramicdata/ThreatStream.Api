using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UserAdministration;

public sealed partial class OrganizationUser
{
	/// <summary>Whether the user is an Org Admin (<c>is_org_admin</c>).</summary>
	[JsonPropertyName("is_org_admin")]
	public bool? IsOrgAdmin { get; init; }

	/// <summary>Whether the user is Read Only (<c>is_readonly</c>).</summary>
	[JsonPropertyName("is_readonly")]
	public bool? IsReadOnly { get; init; }

	/// <summary>Whether the user can approve imported intelligence (<c>can_approve_intel</c>).</summary>
	[JsonPropertyName("can_approve_intel")]
	public bool? CanApproveIntel { get; init; }

	/// <summary>Whether the user can create intelligence shared with the Anomali Community (<c>can_share_intelligence</c>).</summary>
	[JsonPropertyName("can_share_intelligence")]
	public bool? CanShareIntelligence { get; init; }

	/// <summary>Whether the user can see their own API key on the My Profile tab (<c>can_see_api_key</c>); a permission, not the key.</summary>
	[JsonPropertyName("can_see_api_key")]
	public bool? CanSeeApiKey { get; init; }

	/// <summary>Whether the user can push data from TAXII clients to the organisation's TAXII server (<c>can_import_to_taxii_inbox</c>).</summary>
	[JsonPropertyName("can_import_to_taxii_inbox")]
	public bool? CanImportToTaxiiInbox { get; init; }

	/// <summary>Whether the user can use ThreatStream Chat (<c>can_use_chat</c>).</summary>
	[JsonPropertyName("can_use_chat")]
	public bool? CanUseChat { get; init; }

	/// <summary>Whether the user is exempt from multi-factor authentication (<c>is_tfa_exempt</c>).</summary>
	[JsonPropertyName("is_tfa_exempt")]
	public bool? IsTfaExempt { get; init; }
}
