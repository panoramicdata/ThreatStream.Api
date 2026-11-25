using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;

namespace ThreatStream.Api.Interfaces;

/// <summary>Searches saved from the ThreatStream user interface (<c>api/v1/search_filter/</c>).</summary>
public interface ISearchFilters
{
	/// <summary>Lists your organisation's saved searches (<c>GET api/v1/search_filter/</c>).</summary>
	/// <param name="options">Paging and ordering; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of saved searches. Run one with <see cref="ObservableListOptions.SearchFilter"/>.</returns>
	/// <remarks>Not one of the endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user.</remarks>
	[Get("api/v1/search_filter/")]
	Task<ThreatStreamPage<SearchFilter>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
