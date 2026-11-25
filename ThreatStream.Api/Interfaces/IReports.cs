using Refit;
using ThreatStream.Api.Models.Reports;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// PDF reports on sandbox submissions, rule matches, threat model entities and other resources (<c>api/v1/alfred/</c>).
/// </summary>
public interface IReports
{
	/// <summary>
	/// Requests a report on a resource (<c>GET api/v1/alfred/report/</c>), viewable at once in a browser and exported as PDF
	/// by an asynchronous job.
	/// </summary>
	/// <param name="options">The resource to report on.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The request, with the view URL and the export job's ID (<see cref="ReportGeneration.AsyncReport"/>).</returns>
	/// <remarks>
	/// <para>
	/// Although it is a GET, it starts a report generation job on ThreatStream, and the read-only client does not refuse it
	/// (its guard passes every GET).
	/// </para>
	/// <para>Rate limit: one request per minute per user.</para>
	/// </remarks>
	[Get("api/v1/alfred/report/")]
	Task<ReportGeneration> GenerateAsync([Query] ReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the PDF export job of a report (<c>GET api/v1/alfred/async/{id}/</c>): whether it is done, and where to download
	/// the PDF.
	/// </summary>
	/// <param name="id">The export job's ID (<see cref="ReportGeneration.AsyncReport"/>).</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The export job.</returns>
	/// <remarks>
	/// The reference shows this path only as the report request's <c>callbackUrl</c>, on the host
	/// <c>optic.threatstream.com</c> rather than the API host; this method sends it to the client's base address, which
	/// has not been verified against a live deployment. Unit-verified only. Not one of the endpoints with a documented rate
	/// limit, so ThreatStream's default applies: one request per minute per user.
	/// </remarks>
	[Get("api/v1/alfred/async/{id}/")]
	Task<AsyncReport> GetAsyncReportAsync(long id, CancellationToken cancellationToken);
}
