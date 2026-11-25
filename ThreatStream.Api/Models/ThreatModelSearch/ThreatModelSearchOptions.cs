using Refit;

namespace ThreatStream.Api.Models.ThreatModelSearch;

/// <summary>
/// Filters for <c>GET api/v1/threat_model_search/</c>. Filters on different fields are combined with AND; a
/// comma-separated value matches any of its items (OR). Pass <see langword="null"/> for the 20 most recently modified
/// entities of every type except vulnerabilities.
/// </summary>
/// <remarks>
/// <see cref="SearchFilter"/> runs a saved search and cannot be combined with <see cref="Query"/> or the other filters;
/// paging and ordering still apply.
/// </remarks>
public sealed partial class ThreatModelSearchOptions : ListOptions
{
	/// <summary>The advanced search expression (<c>q</c>), with symbolic operators.</summary>
	[AliasAs("q")]
	public string? Query { get; set; }

	/// <summary>The ID of a saved search to run (<c>search_filter</c>).</summary>
	[AliasAs("search_filter")]
	public long? SearchFilter { get; set; }

	/// <summary>
	/// The entity type, comma-separated for several (<c>model_type</c>): <c>actor</c>, <c>attackpattern</c>,
	/// <c>campaign</c>, <c>incident</c>, <c>malware</c>, <c>signature</c>, <c>tipreport</c>, <c>ttp</c> or
	/// <c>vulnerability</c>. Vulnerabilities are only found when asked for here.
	/// </summary>
	[AliasAs("model_type")]
	public string? ModelType { get; set; }

	/// <summary>
	/// A keyword search, as in the user interface, over names, aliases, descriptions and tags (<c>value</c>).
	/// </summary>
	[AliasAs("value")]
	public string? Value { get; set; }

	/// <summary>The exact name (<c>name</c>).</summary>
	[AliasAs("name")]
	public string? Name { get; set; }

	/// <summary>Names containing this text (<c>name__contains</c>).</summary>
	[AliasAs("name__contains")]
	public string? NameContains { get; set; }

	/// <summary>An alias of actors (<c>alias</c>).</summary>
	[AliasAs("alias")]
	public string? Alias { get; set; }

	/// <summary>A tag name, comma-separated for any of several (<c>tags.name</c>).</summary>
	[AliasAs("tags.name")]
	public string? TagName { get; set; }

	/// <summary>The Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[AliasAs("tlp")]
	public Tlp? Tlp { get; set; }

	/// <summary>Whether the entities are public (<c>is_public</c>).</summary>
	[AliasAs("is_public")]
	public bool? IsPublic { get; set; }

	/// <summary>
	/// The publication status: <c>new</c>, <c>pending_review</c>, <c>review_requested</c>, <c>reviewed</c> or
	/// <c>published</c> (<c>publication_status</c>).
	/// </summary>
	[AliasAs("publication_status")]
	public string? PublicationStatus { get; set; }

	/// <summary>
	/// The signature type, such as <c>Snort</c>, <c>YARA</c> or <c>Splunk Query</c> (<c>signature$type</c>); needs
	/// <see cref="ModelType"/> to include <c>signature</c>.
	/// </summary>
	[AliasAs("signature$type")]
	public string? SignatureType { get; set; }

	/// <summary>The feed that provided the entities (<c>feed_id</c>).</summary>
	[AliasAs("feed_id")]
	public long? FeedId { get; set; }

	/// <summary>The organisation that owns the entities (<c>organization_id</c>).</summary>
	[AliasAs("organization_id")]
	public long? OrganizationId { get; set; }

	/// <summary>The user who created the entities (<c>owner_user_id</c>).</summary>
	[AliasAs("owner_user_id")]
	public long? OwnerUserId { get; set; }

	/// <summary>The trusted circles the entities are shared with (<c>trusted_circle_ids</c>).</summary>
	[AliasAs("trusted_circle_ids")]
	public IReadOnlyList<long>? TrustedCircleIds { get; set; }

	/// <summary>Whether threat bulletins came from an email import (<c>is_email</c>).</summary>
	[AliasAs("is_email")]
	public bool? IsEmail { get; set; }
}
