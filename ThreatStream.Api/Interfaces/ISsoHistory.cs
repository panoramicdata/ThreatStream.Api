using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.SsoHistory;

namespace ThreatStream.Api.Interfaces;

/// <summary>Your organisation's single sign-on activity log (<c>api/v1/ssohistory/</c>).</summary>
public interface ISsoHistory
{
	/// <summary>
	/// Lists single sign-on activity: logins, configuration and permission mapping changes (<c>GET api/v1/ssohistory/</c>).
	/// </summary>
	/// <param name="options">Paging and ordering; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of activity.</returns>
	/// <remarks>Not one of the endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user.</remarks>
	[Get("api/v1/ssohistory/")]
	Task<ThreatStreamPage<SsoActivity>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
