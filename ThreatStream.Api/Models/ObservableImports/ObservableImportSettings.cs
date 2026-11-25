using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// The settings an import without approval accepts both for every observable (<see cref="ObservableImportMeta"/>) and per
/// observable (<see cref="ObservableImportObject"/>, which takes precedence).
/// </summary>
public abstract class ObservableImportSettings
{
	/// <summary>The visibility: <c>public</c> (the default) or <c>private</c> (<c>classification</c>).</summary>
	[JsonPropertyName("classification")]
	public string? Classification { get; init; }

	/// <summary>
	/// The confidence you assign, 0 to 100 (<c>confidence</c>). Email and hash observables without one are imported as false
	/// positives.
	/// </summary>
	[JsonPropertyName("confidence")]
	public int? Confidence { get; init; }

	/// <summary>
	/// How much of the final confidence comes from <see cref="Confidence"/> rather than ThreatStream's own score, in percent
	/// (<c>source_confidence_weight</c>).
	/// </summary>
	[JsonPropertyName("source_confidence_weight")]
	public int? SourceConfidenceWeight { get; init; }

	/// <summary>When the observables expire; 90 days from now by default (<c>expiration_ts</c>).</summary>
	[JsonPropertyName("expiration_ts")]
	public DateTimeOffset? ExpirationTs { get; init; }

	/// <summary>The severity: <c>low</c>, <c>medium</c>, <c>high</c> or <c>very-high</c> (<c>severity</c>).</summary>
	[JsonPropertyName("severity")]
	public string? Severity { get; init; }

	/// <summary>The ID of the source feed (<c>source_feed_id</c>).</summary>
	[JsonPropertyName("source_feed_id")]
	public long? SourceFeedId { get; init; }

	/// <summary>The tags to apply (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }

	/// <summary>The trusted circles to share the observables with (<c>trustedcircles</c>).</summary>
	[JsonPropertyName("trustedcircles")]
	public IReadOnlyList<long>? TrustedCircles { get; init; }

	/// <summary>Source regions, countries or US states, such as <c>europe</c> or <c>US-FL</c> (<c>source_locations</c>).</summary>
	[JsonPropertyName("source_locations")]
	public IReadOnlyList<string>? SourceLocations { get; init; }

	/// <summary>Targeted regions, countries or US states (<c>target_locations</c>).</summary>
	[JsonPropertyName("target_locations")]
	public IReadOnlyList<string>? TargetLocations { get; init; }

	/// <summary>Targeted industries from the STIX 2.1 vocabulary (<c>target_industry</c>).</summary>
	[JsonPropertyName("target_industry")]
	public IReadOnlyList<string>? TargetIndustry { get; init; }

	/// <summary>
	/// ThreatStream OnPrem only: <see langword="true"/> to import into ThreatStream Cloud (<c>remote_api</c>). Never set it to
	/// <see langword="false"/>, which ThreatStream rejects.
	/// </summary>
	[JsonPropertyName("remote_api")]
	public bool? RemoteApi { get; init; }
}
