using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Workgroups;

/// <summary>The response of <c>POST api/v2/intelligence/bulk_workgroups/</c>.</summary>
public sealed class BulkWorkgroupsResult : ThreatStreamObject
{
	/// <summary>The IDs of the workgroups given in the request (<c>workgroups</c>).</summary>
	[JsonPropertyName("workgroups")]
	public IReadOnlyList<long> Workgroups { get; init; } = [];

	/// <summary>Whether the restriction was applied (<c>success</c>).</summary>
	[JsonPropertyName("success")]
	public bool? Success { get; init; }
}
