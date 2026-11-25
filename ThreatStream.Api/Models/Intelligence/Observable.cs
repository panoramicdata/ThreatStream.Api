using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Intelligence;

/// <summary>
/// An observable (indicator) in ThreatStream, as returned by <c>api/v2/intelligence/</c>: an IP address, domain, URL,
/// email address, hash or string, with its indicator type, scores and provenance.
/// </summary>
/// <remarks>
/// The commonly used attributes are modelled; the rest (<c>locations</c>, <c>source_locations</c>, <c>target_industry</c>,
/// <c>workgroups</c>, <c>sort</c> and the deprecated <c>threatscore</c>) stay in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>. Other categories that return observables reuse this model.
/// </remarks>
public sealed class Observable : ThreatStreamResource
{
	/// <summary>The observable's value, such as <c>1.2.3.4</c> or <c>bad.example</c> (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	public string? Value { get; init; }

	/// <summary>The observable type: <c>domain</c>, <c>email</c>, <c>ip</c>, <c>md5</c>, <c>string</c> or <c>url</c> (<c>type</c>).</summary>
	[JsonPropertyName("type")]
	public string? Type { get; init; }

	/// <summary>The indicator type, such as <c>mal_ip</c> or <c>apt_domain</c> (<c>itype</c>).</summary>
	[JsonPropertyName("itype")]
	public string? Itype { get; init; }

	/// <summary>The summarised threat type, such as <c>malware</c>, <c>apt</c> or <c>c2</c> (<c>threat_type</c>).</summary>
	[JsonPropertyName("threat_type")]
	public string? ThreatType { get; init; }

	/// <summary>The status, such as <c>active</c>, <c>inactive</c> or <c>falsepos</c> (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>The visibility: <c>public</c> or <c>private</c> (<c>classification</c>).</summary>
	[JsonPropertyName("classification")]
	public string? Classification { get; init; }

	/// <summary>The confidence score, 0 to 100 (<c>confidence</c>).</summary>
	[JsonPropertyName("confidence")]
	public int? Confidence { get; init; }

	/// <summary>The score of ThreatStream's machine learning engine, 0 to 100, or -1 when none was calculated (<c>retina_confidence</c>).</summary>
	[JsonPropertyName("retina_confidence")]
	public int? RetinaConfidence { get; init; }

	/// <summary>The risk score the source reported, 0 to 100 (<c>source_reported_confidence</c>).</summary>
	[JsonPropertyName("source_reported_confidence")]
	public int? SourceReportedConfidence { get; init; }

	/// <summary>The severity: <c>low</c>, <c>medium</c>, <c>high</c> or <c>very-high</c> (<c>severity</c>).</summary>
	[JsonPropertyName("severity")]
	public string? Severity { get; init; }

	/// <summary>The Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>Extra detail: severity, malware type and the tags ThreatStream assigned (<c>meta</c>).</summary>
	[JsonPropertyName("meta")]
	public ObservableMeta? Meta { get; init; }

	/// <summary>The tags on the observable (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag> Tags { get; init; } = [];

	/// <summary>For hashes, the hash type: <c>MD5</c>, <c>SHA1</c>, <c>SHA256</c> or <c>SHA512</c> (<c>subtype</c>).</summary>
	[JsonPropertyName("subtype")]
	public string? Subtype { get; init; }

	/// <summary>The autonomous system number of the IP address (<c>asn</c>).</summary>
	[JsonPropertyName("asn")]
	public string? Asn { get; init; }

	/// <summary>The country code of the IP address (<c>country</c>).</summary>
	[JsonPropertyName("country")]
	public string? Country { get; init; }

	/// <summary>For a domain or URL, the IP address it resolved to (<c>ip</c>).</summary>
	[JsonPropertyName("ip")]
	public string? Ip { get; init; }

	/// <summary>For an IP observable, the address (<c>srcip</c>).</summary>
	[JsonPropertyName("srcip")]
	public string? SourceIp { get; init; }

	/// <summary>The registered owner of the IP address (<c>org</c>).</summary>
	[JsonPropertyName("org")]
	public string? Org { get; init; }

	/// <summary>The reverse DNS name of the IP address (<c>rdns</c>).</summary>
	[JsonPropertyName("rdns")]
	public string? Rdns { get; init; }

	/// <summary>The latitude of the IP address's location (<c>latitude</c>).</summary>
	[JsonPropertyName("latitude")]
	public double? Latitude { get; init; }

	/// <summary>The longitude of the IP address's location (<c>longitude</c>).</summary>
	[JsonPropertyName("longitude")]
	public double? Longitude { get; init; }

	/// <summary>When the observable was first seen on ThreatStream (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the observable last changed on ThreatStream (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }

	/// <summary>When the observable expires (<c>expiration_ts</c>).</summary>
	[JsonPropertyName("expiration_ts")]
	public DateTimeOffset? ExpirationTs { get; init; }

	/// <summary>When the original source created it (<c>source_created</c>).</summary>
	[JsonPropertyName("source_created")]
	public DateTimeOffset? SourceCreated { get; init; }

	/// <summary>When the original source last changed it (<c>source_modified</c>).</summary>
	[JsonPropertyName("source_modified")]
	public DateTimeOffset? SourceModified { get; init; }

	/// <summary>The (usually obfuscated) source, such as a feed name or <c>Analyst</c> (<c>source</c>).</summary>
	[JsonPropertyName("source")]
	public string? Source { get; init; }

	/// <summary>The email address of the user who imported it (<c>created_by</c>).</summary>
	[JsonPropertyName("created_by")]
	public string? CreatedBy { get; init; }

	/// <summary>The feed that brought it in; 0 for user-created observables (<c>feed_id</c>).</summary>
	[JsonPropertyName("feed_id")]
	public long? FeedId { get; init; }

	/// <summary>The import session that brought it in; 0 for feed observables (<c>import_session_id</c>).</summary>
	[JsonPropertyName("import_session_id")]
	public long? ImportSessionId { get; init; }

	/// <summary>The organisation that brought it in (<c>owner_organization_id</c>).</summary>
	[JsonPropertyName("owner_organization_id")]
	public long? OwnerOrganizationId { get; init; }

	/// <summary>The trusted circles it is shared with (<c>trusted_circle_ids</c>).</summary>
	[JsonPropertyName("trusted_circle_ids")]
	public IReadOnlyList<long> TrustedCircleIds { get; init; } = [];

	/// <summary>Whether it is public (<c>is_public</c>).</summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; init; }

	/// <summary>Whether the owner's organisation and user are hidden from other organisations (<c>is_anonymous</c>).</summary>
	[JsonPropertyName("is_anonymous")]
	public bool? IsAnonymous { get; init; }

	/// <summary>Whether an intelligence source may update it (<c>is_editable</c>).</summary>
	[JsonPropertyName("is_editable")]
	public bool? IsEditable { get; init; }

	/// <summary>Whether the user may add public tags (<c>can_add_public_tags</c>).</summary>
	[JsonPropertyName("can_add_public_tags")]
	public bool? CanAddPublicTags { get; init; }

	/// <summary>
	/// An identifier that increases every time the observable changes (<c>update_id</c>); used to page through large result
	/// sets (see <see cref="ThreatStreamClient.EnumerateObservablesByUpdateIdAsync"/>).
	/// </summary>
	[JsonPropertyName("update_id")]
	public long UpdateId { get; init; }

	/// <summary>The STIX identifier (<c>uuid</c>).</summary>
	[JsonPropertyName("uuid")]
	public string? Uuid { get; init; }
}
