using Refit;
using ThreatStream.Api.Models.ObservableImports;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Imports of observables into ThreatStream, with approval (<c>api/v2/intelligence/import/</c>) or without
/// (<c>PATCH api/v2/intelligence/</c>).
/// </summary>
/// <remarks>
/// Every operation changes ThreatStream, so a read-only client refuses them. Imports are limited to 90 an hour per
/// organisation, with and without approval.
/// </remarks>
public interface IObservableImports
{
	/// <summary>
	/// Imports observables from a file, text or a URL into an import session that waits for approval in the ThreatStream user
	/// interface, unless <see cref="ObservableImportRequest.DefaultState"/> is <c>active</c>
	/// (<c>POST api/v2/intelligence/import/</c>, <c>multipart/form-data</c>).
	/// </summary>
	/// <param name="request">The data and the settings for its observables.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The import session created (<c>202 Accepted</c>); poll it with <see cref="IImportSessions.GetAsync"/>.</returns>
	/// <remarks>Changes ThreatStream. Rate limit: 90 imports an hour per organisation.</remarks>
	[Post("api/v2/intelligence/import/")]
	Task<ImportJob> ImportAsync([Body] ObservableImportRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Imports observables directly, without an import session to approve (<c>PATCH api/v2/intelligence/</c>, JSON).
	/// </summary>
	/// <param name="request">The settings for every observable and the observables themselves; at most 10 MB.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream accepts the request (<c>202 Accepted</c>).</returns>
	/// <remarks>
	/// Changes ThreatStream, and needs the Approve Intelligence permission. Acceptance only means the JSON was valid:
	/// observables with missing or wrong fields can still be ignored or imported as false positives. Rate limit: 90 imports
	/// an hour per organisation.
	/// </remarks>
	[Patch("api/v2/intelligence/")]
	Task ImportWithoutApprovalAsync([Body] ObservableBulkImportRequest request, CancellationToken cancellationToken);
}
