using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// An import with approval (<c>POST api/v2/intelligence/import/</c>, sent as <c>multipart/form-data</c>): observables in a
/// file (<see cref="File"/>), in text (<see cref="DataText"/>) or at a plain-text URL (<see cref="Url"/>), with settings
/// applied to every observable that does not set its own.
/// </summary>
/// <remarks>
/// <para>
/// Unstructured data (any text, CSV, JSON or PDF) is parsed for observables, which all get the same settings. Structured data
/// is a CSV file whose header is <c>value,itype,tags</c> (optionally followed by <c>private_tags</c>, <c>is_anonymous</c> and
/// <c>tlp</c>); its columns override these settings per observable.
/// </para>
/// <para>
/// Give <see cref="ThreatType"/>, or else the <c>*Mapping</c> indicator types for every observable type the data holds.
/// Fields this model does not declare (the reference's examples also send <c>notes</c>) go in
/// <see cref="ThreatStreamMultipartRequest.AdditionalFields"/>.
/// </para>
/// </remarks>
public sealed partial class ObservableImportRequest : ThreatStreamMultipartRequest
{
	/// <summary>The file holding the observables (<c>file</c>).</summary>
	[JsonPropertyName("file")]
	public ThreatStreamFile? File { get; init; }

	/// <summary>Text holding the observables (<c>datatext</c>).</summary>
	[JsonPropertyName("datatext")]
	public string? DataText { get; init; }

	/// <summary>The URL of a plain-text intelligence stream to import (<c>url</c>).</summary>
	[JsonPropertyName("url")]
	public string? Url { get; init; }

	/// <summary><c>structured</c> or <c>unstructured</c> (<c>data_type</c>), as in the reference's examples.</summary>
	[JsonPropertyName("data_type")]
	public string? DataType { get; init; }

	/// <summary>The threat type, which decides the indicator types assigned, such as <c>malware</c>, <c>apt</c> or <c>c2</c> (<c>threat_type</c>).</summary>
	[JsonPropertyName("threat_type")]
	public string? ThreatType { get; init; }

	/// <summary>The visibility: <c>public</c> or <c>private</c> (<c>classification</c>).</summary>
	[JsonPropertyName("classification")]
	public string? Classification { get; init; }

	/// <summary>The confidence you assign, 0 to 100 (<c>confidence</c>).</summary>
	[JsonPropertyName("confidence")]
	public int? Confidence { get; init; }

	/// <summary>
	/// How much of the final confidence comes from <see cref="Confidence"/> rather than ThreatStream's own score, in percent;
	/// 100 to use yours entirely (<c>source_confidence_weight</c>).
	/// </summary>
	[JsonPropertyName("source_confidence_weight")]
	public int? SourceConfidenceWeight { get; init; }

	/// <summary>
	/// <c>active</c> to approve the import at once, which needs the Approve Intelligence permission; otherwise it waits for
	/// approval (<c>default_state</c>).
	/// </summary>
	[JsonPropertyName("default_state")]
	public string? DefaultState { get; init; }

	/// <summary>When the observables expire; 90 days from now by default (<c>expiration_ts</c>).</summary>
	[JsonPropertyName("expiration_ts")]
	public DateTimeOffset? ExpirationTs { get; init; }

	/// <summary>The severity: <c>low</c>, <c>medium</c>, <c>high</c> or <c>very-high</c> (<c>severity</c>).</summary>
	[JsonPropertyName("severity")]
	public string? Severity { get; init; }

	/// <summary>The tags to apply, sent as JSON (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }

	/// <summary>The Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>The trusted circles to import into, sent comma-separated (<c>trustedcircles</c>).</summary>
	[JsonPropertyName("trustedcircles")]
	public IReadOnlyList<long>? TrustedCircles { get; init; }

	/// <summary>Whether to drop benign observables (confidence 15 or less); <see langword="true"/> by default (<c>reject_benign</c>).</summary>
	[JsonPropertyName("reject_benign")]
	public bool? RejectBenign { get; init; }

	/// <summary>Whether to hide your organisation from other organisations (<c>is_anonymous</c>).</summary>
	[JsonPropertyName("is_anonymous")]
	public bool? IsAnonymous { get; init; }

	/// <summary>Where the intelligence came from (<c>intelligence_source</c>).</summary>
	[JsonPropertyName("intelligence_source")]
	public string? IntelligenceSource { get; init; }

	/// <summary>
	/// ThreatStream OnPrem only: <see langword="true"/> to import into ThreatStream Cloud (<c>remote_api</c>). Never set it to
	/// <see langword="false"/>, which ThreatStream rejects.
	/// </summary>
	[JsonPropertyName("remote_api")]
	public bool? RemoteApi { get; init; }
}
