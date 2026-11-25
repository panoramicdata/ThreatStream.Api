using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Snapshots;

/// <summary>
/// A point-in-time copy of threat intelligence (<c>api/v1/snapshot/</c>): its progress and, as they are written, the files
/// to download.
/// </summary>
public sealed class Snapshot : ThreatStreamResource
{
	/// <summary><c>inprogress</c> while files are being written, then <c>completed</c> or <c>errors</c> (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>When the snapshot was started (<c>snapshot_ts</c>).</summary>
	[JsonPropertyName("snapshot_ts")]
	public DateTimeOffset? SnapshotTs { get; init; }

	/// <summary>The URL to check the snapshot's progress at (<c>location</c>).</summary>
	[JsonPropertyName("location")]
	public string? Location { get; init; }

	/// <summary>The SHA-256 sum of the snapshot file (<c>sha256sum</c>).</summary>
	[JsonPropertyName("sha256sum")]
	public string? Sha256Sum { get; init; }

	/// <summary>The files written so far (<c>files</c>); keep polling until <see cref="Status"/> is <c>completed</c>.</summary>
	[JsonPropertyName("files")]
	public IReadOnlyList<SnapshotFile> Files { get; init; } = [];
}
