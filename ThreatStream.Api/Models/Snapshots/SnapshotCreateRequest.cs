using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Snapshots;

/// <summary>A request to start a snapshot (<c>POST api/v1/snapshot/</c>).</summary>
public sealed class SnapshotCreateRequest
{
	/// <summary>The intelligence version (<c>intel_version</c>); only 1 is supported.</summary>
	[JsonPropertyName("intel_version")]
	public int? IntelVersion { get; init; }

	/// <summary>What the snapshot holds and how it is split (<c>custom_config</c>).</summary>
	[JsonPropertyName("custom_config")]
	public required SnapshotConfig CustomConfig { get; init; }
}
