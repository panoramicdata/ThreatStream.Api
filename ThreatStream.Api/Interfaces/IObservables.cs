using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableImports;
using ThreatStream.Api.Models.Intelligence;

namespace ThreatStream.Api.Interfaces;

/// <summary>Observables (indicators) in ThreatStream (<c>api/v2/intelligence/</c>).</summary>
/// <remarks>
/// Importing observables is in <see cref="IObservableImports"/>, although one of its operations (import without approval,
/// <c>PATCH api/v2/intelligence/</c>) shares this path.
/// </remarks>
public interface IObservables
{
	/// <summary>
	/// Lists observables, the 20 most recently modified by default (<c>GET api/v2/intelligence/</c>).
	/// </summary>
	/// <param name="options">Filters, paging and ordering; <see langword="null"/> for the defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables. <c>meta.total_count</c> may be approximate unless
	/// <see cref="ObservableListOptions.FullCount"/> is 1.</returns>
	/// <remarks>
	/// <para>
	/// Rate limit: 150 requests a minute per user, one a minute when filtering with regular expressions.
	/// </para>
	/// <para>
	/// To read every page use <see cref="ThreatStreamClient.EnumerateObservablesAsync"/>; above 10,000 results use
	/// <see cref="ThreatStreamClient.EnumerateObservablesByUpdateIdAsync"/>, as ThreatStream recommends. For very large sets
	/// on an ad-hoc basis, a snapshot (<see cref="ISnapshots"/>) is faster.
	/// </para>
	/// </remarks>
	[Get("api/v2/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListAsync([Query] ObservableListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Lists the observables associated with an observable value or IDs
	/// (<c>GET api/v2/intelligence/associated_with_intelligence/</c>).
	/// </summary>
	/// <param name="options">The value or IDs to look up, and paging.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	/// <remarks>
	/// The ThreatStream API reference names this path only in its table of rate limits (150 requests a minute per user); its
	/// parameters and response are inferred from the threat model types' <c>associated_with_intelligence</c> lookups
	/// (<c>value</c> or <c>ids</c>). Unit-verified only.
	/// </remarks>
	[Get("api/v2/intelligence/associated_with_intelligence/")]
	Task<ThreatStreamPage<Observable>> ListAssociatedWithIntelligenceAsync([Query] ObservableAssociationOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Clones an observable (typically another organisation's) into your organisation
	/// (<c>POST api/v2/intelligence/{observableId}/clone/</c>).
	/// </summary>
	/// <param name="observableId">The observable's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The import session the clone created (<c>202 Accepted</c>), or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>
	/// Changes ThreatStream: it starts an import session, which must be approved (see
	/// <see cref="IImportSessions.ApproveAllAsync"/>) before the clone becomes part of your intelligence. A read-only client
	/// refuses it. The reference's example omits the <c>/api</c> prefix; the path here follows the rest of the API.
	/// </remarks>
	[Post("api/v2/intelligence/{observableId}/clone/")]
	Task<ImportJob?> CloneAsync(long observableId, CancellationToken cancellationToken);
}
