using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Workgroups;

/// <summary>The body of <c>POST api/v2/intelligence/bulk_workgroups/</c>: the observables to restrict and the workgroups to restrict them to.</summary>
public sealed class BulkWorkgroupsRequest
{
	/// <summary>The IDs of the observables whose visibility is restricted (<c>ids</c>).</summary>
	[JsonPropertyName("ids")]
	public required IReadOnlyList<long> Ids { get; init; }

	/// <summary>The IDs of the workgroups that are given exclusive access to the observables (<c>workgroups</c>).</summary>
	[JsonPropertyName("workgroups")]
	public required IReadOnlyList<long> Workgroups { get; init; }
}
