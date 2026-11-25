using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UserAudit;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// User audit: reports of the API requests the organisation's users made (<c>api/v1/apiuseraction/</c>). Requires Org
/// Admin.
/// </summary>
/// <remarks>
/// No documented rate limit, so ThreatStream's default applies: one request per minute per user. For user interface
/// activity see <see cref="IUserActions"/>.
/// </remarks>
public interface IApiUserActions
{
	/// <summary>
	/// Starts generating an API activity report as an asynchronous job (<c>GET api/v1/apiuseraction/?async_result=true</c>).
	/// Poll and download it with <see cref="IAsyncResults"/>, or let <see cref="ThreatStreamClient.RunAsyncJobAsync"/> do
	/// both.
	/// </summary>
	/// <param name="options">The time range, users, format and record limit, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The job, typically with status <c>inprogress</c>, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Get("api/v1/apiuseraction/?async_result=true")]
	Task<GenericAsyncResult?> StartReportAsync([Query] UserActivityReportOptions? options, CancellationToken cancellationToken);

	/// <summary>Generates an API activity report and returns it in the same response (<c>GET api/v1/apiuseraction/</c>).</summary>
	/// <param name="options">The time range, users, format and record limit, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The report as text, CSV or JSON as <see cref="UserActivityReportOptions.Format"/> asks.</returns>
	[Get("api/v1/apiuseraction/")]
	Task<string> GetReportAsync([Query] UserActivityReportOptions? options, CancellationToken cancellationToken);
}
