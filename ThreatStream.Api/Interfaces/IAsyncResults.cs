using Refit;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Asynchronous jobs (<c>api/v1/generic_async_result/</c>): user activity reports and query-based bulk tagging. See
/// <see cref="ThreatStreamClient.RunAsyncJobAsync"/> to start, poll and download in one call.
/// </summary>
/// <remarks>No documented rate limit, so ThreatStream's default applies: one request per minute per user.</remarks>
public interface IAsyncResults
{
	/// <summary>Gets a job's state (<c>GET api/v1/generic_async_result/{id}/</c>).</summary>
	/// <param name="id">The job's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The job; it is finished when <see cref="GenericAsyncResult.Status"/> is <c>done</c>.</returns>
	[Get("api/v1/generic_async_result/{id}/")]
	Task<GenericAsyncResult> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Downloads a finished job's result (<c>GET api/v1/generic_async_result/{id}/get_result/</c>).</summary>
	/// <param name="id">The job's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The result as text: CSV or JSON for a user activity report.</returns>
	/// <remarks>
	/// The user audit reference writes this path without its <c>/api/v1</c> prefix; its examples include it. For a bulk
	/// tagging job, fetching the result is what finishes the job.
	/// </remarks>
	[Get("api/v1/generic_async_result/{id}/get_result/")]
	Task<string> GetResultAsync(long id, CancellationToken cancellationToken);
}
