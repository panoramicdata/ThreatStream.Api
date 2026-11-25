using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Feeds;

/// <summary>
/// An intelligence feed (stream) that brings observables into ThreatStream (<c>api/v1/feed/</c>).
/// </summary>
/// <remarks>
/// The documented attributes whose shape the reference gives are modelled; <c>circles</c>, <c>config</c>,
/// <c>fields</c>, <c>fields_mapping</c>, <c>tags</c> and the attributes it calls internal stay in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class Feed : ThreatStreamResource
{
	/// <summary>The feed's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>A short description (<c>description</c>).</summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }

	/// <summary>Whether ThreatStream is currently receiving data from it (<c>active</c>).</summary>
	[JsonPropertyName("active")]
	public bool? Active { get; init; }

	/// <summary>The feed's health: <c>success</c>, <c>no results</c>, <c>errors</c> or <c>no run</c> (<c>state</c>).</summary>
	[JsonPropertyName("state")]
	public string? State { get; init; }

	/// <summary>The visibility of its data: <c>public</c> or <c>private</c> (<c>classification</c>).</summary>
	[JsonPropertyName("classification")]
	public string? Classification { get; init; }

	/// <summary>The default indicator type of its observables (<c>itype</c>).</summary>
	[JsonPropertyName("itype")]
	public string? Itype { get; init; }

	/// <summary>The confidence score of its observables, 0 to 100 (<c>confidence</c>).</summary>
	[JsonPropertyName("confidence")]
	public int? Confidence { get; init; }

	/// <summary>How much of the final confidence comes from the source's confidence, in percent (<c>source_confidence_weight</c>).</summary>
	[JsonPropertyName("source_confidence_weight")]
	public double? SourceConfidenceWeight { get; init; }

	/// <summary>How many days its observables stay active after import (<c>expiry_time_in_days</c>).</summary>
	[JsonPropertyName("expiry_time_in_days")]
	public int? ExpiryTimeInDays { get; init; }

	/// <summary>How often ThreatStream scrapes it (<c>interval</c>).</summary>
	[JsonPropertyName("interval")]
	public string? Interval { get; init; }

	/// <summary>Where the feed is hosted (<c>url</c>).</summary>
	[JsonPropertyName("url")]
	public string? Url { get; init; }

	/// <summary>The organisation that owns it (<c>organization</c>).</summary>
	[JsonPropertyName("organization")]
	public FeedOrganization? Organization { get; init; }

	/// <summary>When it was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When new data was last imported (<c>last_imported</c>).</summary>
	[JsonPropertyName("last_imported")]
	public DateTimeOffset? LastImported { get; init; }

	/// <summary>When ThreatStream last scraped it (<c>last_scraped</c>).</summary>
	[JsonPropertyName("last_scraped")]
	public DateTimeOffset? LastScraped { get; init; }

	/// <summary>When its settings last changed (<c>last_updated</c>).</summary>
	[JsonPropertyName("last_updated")]
	public DateTimeOffset? LastUpdated { get; init; }
}
