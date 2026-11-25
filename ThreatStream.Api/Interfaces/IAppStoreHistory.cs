using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.AppStoreHistory;

namespace ThreatStream.Api.Interfaces;

/// <summary>Your organisation's APP Store activity log (<c>api/v1/appstorehistory/</c>).</summary>
public interface IAppStoreHistory
{
	/// <summary>
	/// Lists APP Store activity: requests, failed requests, approvals, rejections and other changes
	/// (<c>GET api/v1/appstorehistory/</c>).
	/// </summary>
	/// <param name="options">Paging and ordering; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of activity.</returns>
	/// <remarks>Not one of the endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user.</remarks>
	[Get("api/v1/appstorehistory/")]
	Task<ThreatStreamPage<AppStoreActivity>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
