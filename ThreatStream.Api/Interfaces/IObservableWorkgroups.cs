using Refit;
using ThreatStream.Api.Models.Workgroups;

namespace ThreatStream.Api.Interfaces;

/// <summary>Workgroup restrictions on observables in bulk (<c>api/v2/intelligence/bulk_workgroups/</c>).</summary>
public interface IObservableWorkgroups
{
	/// <summary>
	/// Restricts the visibility of observables to the given workgroups of your organization
	/// (<c>POST api/v2/intelligence/bulk_workgroups/</c>).
	/// </summary>
	/// <param name="request">The observables and the workgroups to restrict them to.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The workgroups given and whether the restriction was applied.</returns>
	/// <remarks>
	/// <para>
	/// <b>Side effect:</b> users outside the workgroups stop seeing the observables. Refused by a read-only client.
	/// </para>
	/// <para>
	/// The reference's attribute table describes <c>ids</c> and <c>workgroups</c> as comma-separated strings, but its
	/// example sends JSON arrays of IDs; this method follows the example. Restrictions cannot be lifted in bulk: the
	/// reference lifts them one observable at a time, by patching the observable's <c>workgroups</c> to an empty list.
	/// </para>
	/// </remarks>
	[Post("api/v2/intelligence/bulk_workgroups/")]
	Task<BulkWorkgroupsResult> RestrictAsync([Body] BulkWorkgroupsRequest request, CancellationToken cancellationToken);
}
