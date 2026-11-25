using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UserAudit;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// User audit: reports of what the organisation's users did in the ThreatStream user interface
/// (<c>api/v1/useraction/</c>), including sign-ins, Threat Model, investigation and import activity. Requires Org Admin.
/// </summary>
/// <remarks>
/// No documented rate limit, so ThreatStream's default applies: one request per minute per user. For API activity see
/// <see cref="IApiUserActions"/>.
/// </remarks>
public interface IUserActions
{
	/// <summary>
	/// Starts generating a UI activity report as an asynchronous job (<c>GET api/v1/useraction/?async_result=true</c>), as
	/// Anomali recommends for more than five users or a week of data. Poll the job with
	/// <see cref="IAsyncResults.GetAsync"/> and download it with <see cref="IAsyncResults.GetResultAsync"/>, or let
	/// <see cref="ThreatStreamClient.RunAsyncJobAsync"/> do both.
	/// </summary>
	/// <param name="options">The time range, users, format and record limit, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The job, typically with status <c>inprogress</c>, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Get("api/v1/useraction/?async_result=true")]
	Task<GenericAsyncResult?> StartReportAsync([Query] UserActivityReportOptions? options, CancellationToken cancellationToken);

	/// <summary>Generates a UI activity report and returns it in the same response (<c>GET api/v1/useraction/</c>).</summary>
	/// <param name="options">The time range, users, format and record limit, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The report as text, CSV or JSON as <see cref="UserActivityReportOptions.Format"/> asks.</returns>
	[Get("api/v1/useraction/")]
	Task<string> GetReportAsync([Query] UserActivityReportOptions? options, CancellationToken cancellationToken);
}
