using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Snapshots;

/// <summary>One file of a <see cref="Snapshot"/>: CSV, JSON or STIX, as the snapshot's format says.</summary>
public sealed class SnapshotFile : ThreatStreamObject
{
	/// <summary>
	/// Where to download the file (<c>download_url</c>). Download it with a plain <see cref="HttpClient"/>, not through
	/// <see cref="ThreatStreamClient"/>: the link is a pre-signed URL on another host, which must not be sent your
	/// ThreatStream credentials.
	/// </summary>
	[JsonPropertyName("download_url")]
	public string? DownloadUrl { get; init; }

	/// <summary>The file's SHA-256 sum, to verify the download (<c>sha256</c>).</summary>
	[JsonPropertyName("sha256")]
	public string? Sha256 { get; init; }

	/// <summary>The number of observables in the file (<c>total_count</c>).</summary>
	[JsonPropertyName("total_count")]
	public long? TotalCount { get; init; }
}
