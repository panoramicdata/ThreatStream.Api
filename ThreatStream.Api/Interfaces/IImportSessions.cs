using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableImports;
using ThreatStream.Api.Models.Intelligence;

namespace ThreatStream.Api.Interfaces;

/// <summary>Import jobs started by an import with approval or a clone (<c>api/v1/importsession/</c>).</summary>
/// <remarks>
/// The reference shows <c>intelligence</c> and <c>approve_all</c> without a trailing slash; the paths here keep the
/// trailing slash Django's other paths have.
/// </remarks>
public interface IImportSessions
{
	/// <summary>Gets an import session, to poll its progress (<c>GET api/v1/importsession/{importSessionId}/</c>).</summary>
	/// <param name="importSessionId">The import session's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The import session.</returns>
	/// <remarks>Not one of the endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user.</remarks>
	[Get("api/v1/importsession/{importSessionId}/")]
	Task<ImportSession> GetAsync(long importSessionId, CancellationToken cancellationToken);

	/// <summary>
	/// Lists the observables in an import session (<c>GET api/v1/importsession/{importSessionId}/intelligence/</c>).
	/// </summary>
	/// <param name="importSessionId">The import session's ID; it must belong to your organisation.</param>
	/// <param name="options">Paging; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of the session's observables.</returns>
	/// <remarks>
	/// The reference says only that the observables' IDs are in their <c>id</c> attributes; the rest of the shape follows the
	/// other observable lists. Unit-verified only.
	/// </remarks>
	[Get("api/v1/importsession/{importSessionId}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long importSessionId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Approves every observable in an import session (<c>PATCH api/v1/importsession/{importSessionId}/approve_all/</c>).</summary>
	/// <param name="importSessionId">The import session's ID; it must belong to your organisation.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream accepts the approval.</returns>
	/// <remarks>Changes ThreatStream, so a read-only client refuses it. Needs the Approve Intelligence permission.</remarks>
	[Patch("api/v1/importsession/{importSessionId}/approve_all/")]
	Task ApproveAllAsync(long importSessionId, CancellationToken cancellationToken);

	/// <summary>
	/// Approves some of the observables in an import session
	/// (<c>PATCH api/v1/importsession/{importSessionId}/approve_selected/?ids=...</c>).
	/// </summary>
	/// <param name="importSessionId">The import session's ID; it must belong to your organisation.</param>
	/// <param name="ids">The IDs of the observables to approve, sent comma-separated in the query string.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream accepts the approval.</returns>
	/// <remarks>Changes ThreatStream, so a read-only client refuses it. Needs the Approve Intelligence permission.</remarks>
	[Patch("api/v1/importsession/{importSessionId}/approve_selected/")]
	Task ApproveSelectedAsync(long importSessionId, [Query][AliasAs("ids")] IReadOnlyList<long> ids, CancellationToken cancellationToken);
}
