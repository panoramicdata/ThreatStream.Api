using Refit;

namespace ThreatStream.Api.Models.Snapshots;

/// <summary>Filters for <c>GET api/v1/snapshot/</c>.</summary>
public sealed class SnapshotListOptions : ListOptions
{
	/// <summary>Snapshots whose ID is greater than this (<c>id__gt</c>).</summary>
	[AliasAs("id__gt")]
	public long? IdAfter { get; set; }

	/// <summary>Snapshots started at or after this moment (<c>snapshot_ts__gte</c>).</summary>
	[AliasAs("snapshot_ts__gte")]
	public DateTimeOffset? StartedSince { get; set; }

	/// <summary>Snapshots started before this moment (<c>snapshot_ts__lt</c>).</summary>
	[AliasAs("snapshot_ts__lt")]
	public DateTimeOffset? StartedBefore { get; set; }

	/// <summary>The snapshot with this file SHA-256 sum (<c>sha256sum</c>).</summary>
	[AliasAs("sha256sum")]
	public string? Sha256Sum { get; set; }
}
