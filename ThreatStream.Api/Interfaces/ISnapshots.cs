using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Snapshots;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Snapshots: point-in-time copies of threat intelligence, for downloading large sets ad hoc (<c>api/v1/snapshot/</c>).
/// </summary>
/// <remarks>
/// Not endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user. An
/// organisation can have at most three snapshots in progress.
/// </remarks>
public interface ISnapshots
{
	/// <summary>Lists snapshots (<c>GET api/v1/snapshot/</c>).</summary>
	/// <param name="options">Filters, paging and ordering; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of snapshots.</returns>
	[Get("api/v1/snapshot/")]
	Task<ThreatStreamPage<Snapshot>> ListAsync([Query] SnapshotListOptions? options, CancellationToken cancellationToken);

	/// <summary>Starts a snapshot (<c>POST api/v1/snapshot/</c>).</summary>
	/// <param name="request">What the snapshot holds and how it is split.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The snapshot as ThreatStream answers it, or <see langword="null"/> when it answers with no body.</returns>
	/// <remarks>
	/// <para>
	/// Changes ThreatStream (it starts an asynchronous job), so a read-only client refuses it.
	/// </para>
	/// <para>
	/// The reference documents the new snapshot's address only as the response's <c>Location</c> header
	/// (<c>.../api/v1/snapshot/{id}/</c>), which this method cannot return; the body is read as a snapshot, whose
	/// <see cref="ThreatStreamResource.Id"/> is 0 if ThreatStream leaves it out. When there is no body or no ID, find the
	/// new snapshot with <see cref="ListAsync"/> ordered by <c>-id</c>. Unit-verified only.
	/// </para>
	/// </remarks>
	[Post("api/v1/snapshot/")]
	Task<Snapshot?> CreateAsync([Body] SnapshotCreateRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Gets a snapshot's progress and the files written so far (<c>GET api/v1/snapshot/{id}/</c>). Poll until its status is
	/// <c>completed</c>.
	/// </summary>
	/// <param name="id">The snapshot's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The snapshot. Download its files from their <see cref="SnapshotFile.DownloadUrl"/> without ThreatStream
	/// credentials.</returns>
	[Get("api/v1/snapshot/{id}/")]
	Task<Snapshot> GetAsync(long id, CancellationToken cancellationToken);
}
