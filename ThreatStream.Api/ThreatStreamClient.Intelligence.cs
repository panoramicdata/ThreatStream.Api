using ThreatStream.Api.Interfaces;
using ThreatStream.Api.Models.Intelligence;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Observables (indicators) in ThreatStream (<c>api/v2/intelligence/</c>).</summary>
	public IObservables Observables => field ??= For<IObservables>();

	/// <summary>Saved searches (<c>api/v1/search_filter/</c>).</summary>
	public ISearchFilters SearchFilters => field ??= For<ISearchFilters>();

	/// <summary>
	/// Enumerates every observable matching <paramref name="options"/> (<c>GET api/v2/intelligence/</c>), following
	/// <c>meta.next</c> page by page (see <see cref="EnumerateAsync{T}"/>).
	/// </summary>
	/// <param name="options">Filters, page size and ordering; <see langword="null"/> for every observable, most recently
	/// modified first.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>Every matching observable.</returns>
	/// <remarks>Suitable up to 10,000 results; beyond that use <see cref="EnumerateObservablesByUpdateIdAsync"/>.</remarks>
	public IAsyncEnumerable<Observable> EnumerateObservablesAsync(ObservableListOptions? options, CancellationToken cancellationToken)
		=> EnumerateAsync(ct => Observables.ListAsync(options, ct), cancellationToken);

	/// <summary>
	/// Enumerates every observable matching <paramref name="options"/> by <c>update_id</c>, the way ThreatStream recommends
	/// for more than 10,000 results: each page asks for <c>update_id__gt</c> the highest <c>update_id</c> seen so far, with
	/// <c>order_by=update_id</c> (see <see cref="EnumerateByUpdateIdAsync{T}"/>).
	/// </summary>
	/// <param name="options">Filters and page size (ThreatStream answers 1000 a page by default here); their ordering and
	/// <see cref="ObservableListOptions.UpdateIdAfter"/> are replaced. <see langword="null"/> for every observable. The
	/// caller's object is not changed.</param>
	/// <param name="afterUpdateId">Where to start: 0 for everything, or the last <c>update_id</c> of a previous sync.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>Every matching observable, in <c>update_id</c> order.</returns>
	/// <remarks>
	/// When <see cref="ObservableListOptions.Query"/> is set, the bound goes inside the advanced search instead, as
	/// <c>(query) AND update_id&gt;N</c>, because ThreatStream requires it there. For a continuous sync, also set
	/// <see cref="ObservableListOptions.ModifiedBefore"/> to a minute ago.
	/// </remarks>
	public IAsyncEnumerable<Observable> EnumerateObservablesByUpdateIdAsync(ObservableListOptions? options, long afterUpdateId, CancellationToken cancellationToken)
	{
		var template = options ?? new ObservableListOptions();
		return EnumerateByUpdateIdAsync((last, ct) => Observables.ListAsync(template.AfterUpdateId(last), ct), static o => o.UpdateId, afterUpdateId, cancellationToken);
	}
}
