using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Feeds;

namespace ThreatStream.Api.Interfaces;

/// <summary>Intelligence feeds (streams) that bring observables into ThreatStream (<c>api/v1/feed/</c>).</summary>
public interface IFeeds
{
	/// <summary>Lists the feeds you have access to (<c>GET api/v1/feed/</c>).</summary>
	/// <param name="options">Filters, paging and ordering; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of feeds.</returns>
	/// <remarks>Not one of the endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user.</remarks>
	[Get("api/v1/feed/")]
	Task<ThreatStreamPage<Feed>> ListAsync([Query] FeedListOptions? options, CancellationToken cancellationToken);
}
