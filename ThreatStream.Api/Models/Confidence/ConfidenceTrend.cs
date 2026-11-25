using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Confidence;

/// <summary>The confidence scores ThreatStream has calculated for one observable (<c>inteldetails/confidence_trend</c>).</summary>
public sealed class ConfidenceTrend : ThreatStreamObject
{
	/// <summary>The lowest calculated confidence score, 0 to 100 (<c>min_confidence</c>).</summary>
	[JsonPropertyName("min_confidence")]
	public double? MinConfidence { get; init; }

	/// <summary>The average calculated confidence score, 0 to 100 (<c>average_confidence</c>).</summary>
	[JsonPropertyName("average_confidence")]
	public double? AverageConfidence { get; init; }

	/// <summary>The highest calculated confidence score, 0 to 100 (<c>max_confidence</c>).</summary>
	[JsonPropertyName("max_confidence")]
	public double? MaxConfidence { get; init; }
}
