using Refit;

namespace ThreatStream.Api.Models.Intelligence;

/// <summary>
/// Filters for <c>GET api/v2/intelligence/</c>. Filters on different fields are combined with AND; a comma-separated value
/// matches any of its items (OR). Pass <see langword="null"/> (or leave every property unset) for the 20 most recently
/// modified observables.
/// </summary>
/// <remarks>
/// <para>
/// For anything the typed filters cannot say, use <see cref="Query"/>, ThreatStream's advanced search expression, such as
/// <c>confidence&gt;=90 AND (itype="apt_ip" OR itype="c2_ip")</c> (filters that are only available there include
/// <c>expiration_ts</c>). <see cref="SearchFilter"/> runs a saved search and cannot be combined with <see cref="Query"/> or
/// the other filters; paging and ordering still apply.
/// </para>
/// <para>
/// The timestamp filters are sent as UTC ISO 8601. Filtering with regular expressions lowers the rate limit from 150
/// requests a minute to one.
/// </para>
/// </remarks>
public sealed partial class ObservableListOptions : ListOptions
{
	/// <summary>The advanced search expression (<c>q</c>), with symbolic operators (<c>=</c>, <c>&gt;=</c>, <c>AND</c>, <c>OR</c>).</summary>
	[AliasAs("q")]
	public string? Query { get; set; }

	/// <summary>The ID of a saved search to run (<c>search_filter</c>); see <see cref="ThreatStreamClient.SearchFilters"/>.</summary>
	[AliasAs("search_filter")]
	public long? SearchFilter { get; set; }

	/// <summary>The exact value (<c>value</c>).</summary>
	[AliasAs("value")]
	public string? Value { get; set; }

	/// <summary>Values containing this text, at least 3 characters for good results (<c>value__contains</c>).</summary>
	[AliasAs("value__contains")]
	public string? ValueContains { get; set; }

	/// <summary>Values starting with this text (<c>value__startswith</c>).</summary>
	[AliasAs("value__startswith")]
	public string? ValueStartsWith { get; set; }

	/// <summary>The observable type, such as <c>ip</c> or <c>domain</c> (<c>type</c>).</summary>
	[AliasAs("type")]
	public string? Type { get; set; }

	/// <summary>The indicator type, such as <c>mal_ip</c>; comma-separated for any of several (<c>itype</c>).</summary>
	[AliasAs("itype")]
	public string? Itype { get; set; }

	/// <summary>The threat type, such as <c>malware</c> or <c>c2</c> (<c>threat_type</c>).</summary>
	[AliasAs("threat_type")]
	public string? ThreatType { get; set; }

	/// <summary>The status, such as <c>active</c>, <c>inactive</c> or <c>falsepos</c> (<c>status</c>).</summary>
	[AliasAs("status")]
	public string? Status { get; set; }

	/// <summary>The hash type of hash observables, such as <c>SHA256</c> (<c>subtype</c>).</summary>
	[AliasAs("subtype")]
	public string? Subtype { get; set; }

	/// <summary>The Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[AliasAs("tlp")]
	public Tlp? Tlp { get; set; }

	/// <summary>Whether the observables are public (<c>is_public</c>).</summary>
	[AliasAs("is_public")]
	public bool? IsPublic { get; set; }

	/// <summary>The name of a tag on the observables (<c>tags.name</c>).</summary>
	[AliasAs("tags.name")]
	public string? TagName { get; set; }

	/// <summary>The severity: <c>low</c>, <c>medium</c>, <c>high</c> or <c>very-high</c> (<c>meta.severity</c>).</summary>
	[AliasAs("meta.severity")]
	public string? Severity { get; set; }

	/// <summary>The associated malware (<c>meta.maltype</c>).</summary>
	[AliasAs("meta.maltype")]
	public string? Maltype { get; set; }

	/// <summary>The tag ThreatStream assigned to related observables (<c>meta.detail</c>).</summary>
	[AliasAs("meta.detail")]
	public string? Detail { get; set; }

	/// <summary>The details of the observables' state (<c>meta.detail2</c>).</summary>
	[AliasAs("meta.detail2")]
	public string? Detail2 { get; set; }

	/// <summary>The feed that brought the observables in; 0 for user-created ones (<c>feed_id</c>).</summary>
	[AliasAs("feed_id")]
	public long? FeedId { get; set; }

	/// <summary>The import session that brought the observables in (<c>import_session_id</c>).</summary>
	[AliasAs("import_session_id")]
	public long? ImportSessionId { get; set; }

	/// <summary>The trusted circles the observables are shared with (<c>trusted_circle_ids</c>).</summary>
	[AliasAs("trusted_circle_ids")]
	public IReadOnlyList<long>? TrustedCircleIds { get; set; }

	/// <summary>A target industry from the STIX 2.1 vocabulary, such as <c>financial-services</c> (<c>target_industry</c>).</summary>
	[AliasAs("target_industry")]
	public string? TargetIndustry { get; set; }

	/// <summary>A targeted region, country or US state, such as <c>europe</c> or <c>US-FL</c> (<c>target_locations</c>).</summary>
	[AliasAs("target_locations")]
	public string? TargetLocations { get; set; }

	/// <summary>A source region, country or US state (<c>source_locations</c>).</summary>
	[AliasAs("source_locations")]
	public string? SourceLocations { get; set; }

	/// <summary>The STIX identifier (<c>uuid</c>).</summary>
	[AliasAs("uuid")]
	public string? Uuid { get; set; }

	/// <summary>Set to 1 for an exact <c>total_count</c> on large result sets, at some cost in speed (<c>full_count</c>).</summary>
	[AliasAs("full_count")]
	public int? FullCount { get; set; }

	/// <summary>ThreatStream OnPrem only: <see langword="true"/> for local observables only (<c>local_only</c>).</summary>
	[AliasAs("local_only")]
	public bool? LocalOnly { get; set; }

	/// <summary>
	/// ThreatStream OnPrem only: <see langword="true"/> for remote observables (stored on ThreatStream Cloud) only
	/// (<c>remote_api</c>). Never set it to <see langword="false"/>, which ThreatStream rejects.
	/// </summary>
	[AliasAs("remote_api")]
	public bool? RemoteApi { get; set; }
}
