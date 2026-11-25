using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>The <c>meta</c> object of a ThreatStream list response.</summary>
public sealed class ThreatStreamPageMeta : ThreatStreamObject
{
	/// <summary>The page size used (<c>limit</c>; 20 by default, and 0 asks for up to 1000).</summary>
	[JsonPropertyName("limit")]
	public int? Limit { get; init; }

	/// <summary>The index of the first object on this page (<c>offset</c>).</summary>
	[JsonPropertyName("offset")]
	public int? Offset { get; init; }

	/// <summary>The link to the next page, relative to the host (<c>/api/...</c>), or <see langword="null"/> on the last page.</summary>
	[JsonPropertyName("next")]
	public string? Next { get; init; }

	/// <summary>The link to the previous page, or <see langword="null"/> on the first page.</summary>
	[JsonPropertyName("previous")]
	public string? Previous { get; init; }

	/// <summary>
	/// The number of matching objects (<c>total_count</c>). For observables and threat model searches it may be approximate
	/// (see <see cref="ApproximateCount"/>) unless the query asked for <c>full_count=1</c>.
	/// </summary>
	[JsonPropertyName("total_count")]
	public long? TotalCount { get; init; }

	/// <summary>How long ThreatStream took to answer, in milliseconds (<c>took</c>).</summary>
	[JsonPropertyName("took")]
	public double? Took { get; init; }

	/// <summary>Whether <see cref="TotalCount"/> is an approximation (<c>approximate_count</c>).</summary>
	[JsonPropertyName("approximate_count")]
	public bool? ApproximateCount { get; init; }
}
