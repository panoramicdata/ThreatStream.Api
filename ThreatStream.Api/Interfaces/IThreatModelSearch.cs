using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModelSearch;

namespace ThreatStream.Api.Interfaces;

/// <summary>Search across every threat model type (<c>api/v1/threat_model_search/</c>).</summary>
public interface IThreatModelSearch
{
	/// <summary>
	/// Searches threat model entities of every type, the most recently modified first
	/// (<c>GET api/v1/threat_model_search/</c>).
	/// </summary>
	/// <param name="options">Filters, paging and ordering; <see langword="null"/> for the defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of entities of mixed types. <c>meta.total_count</c> may be approximate unless
	/// <see cref="ThreatModelSearchOptions.FullCount"/> is 1.</returns>
	/// <remarks>
	/// Vulnerabilities are only found when <see cref="ThreatModelSearchOptions.ModelType"/> asks for them. Rate limit: 750
	/// requests a minute per user. Page through with <see cref="ThreatStreamClient.EnumerateAsync{T}"/>.
	/// </remarks>
	[Get("api/v1/threat_model_search/")]
	Task<ThreatStreamPage<ThreatModelSearchResult>> SearchAsync([Query] ThreatModelSearchOptions? options, CancellationToken cancellationToken);
}
