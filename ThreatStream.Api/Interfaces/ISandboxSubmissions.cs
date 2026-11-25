using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Sandbox;

namespace ThreatStream.Api.Interfaces;

/// <summary>Sandbox detonations of URLs and files (<c>api/v1/submit/</c>).</summary>
/// <remarks>
/// Not endpoints with a documented rate limit, so ThreatStream's default applies: one request per minute per user.
/// </remarks>
public interface ISandboxSubmissions
{
	/// <summary>Submits a URL or file for detonation (<c>POST api/v1/submit/new/</c>, <c>multipart/form-data</c>).</summary>
	/// <param name="request">The URL or file, and the sandbox to use.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>Where to follow each detonation started.</returns>
	/// <remarks>
	/// Changes ThreatStream, so a read-only client refuses it: it detonates the sample (using the organisation's sandbox
	/// quota) and, unless <see cref="SandboxSubmitRequest.ImportIndicators"/> is <see langword="false"/>, imports the
	/// observables found. The chosen sandbox needs an active integration, except the Anomali Sandbox.
	/// </remarks>
	[Post("api/v1/submit/new/")]
	Task<SandboxSubmission> SubmitAsync([Body] SandboxSubmitRequest request, CancellationToken cancellationToken);

	/// <summary>Searches your organisation's detonations (<c>GET api/v1/submit/search/</c>).</summary>
	/// <param name="options">The keyword, and paging; <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of detonations.</returns>
	[Get("api/v1/submit/search/")]
	Task<ThreatStreamPage<SandboxReport>> SearchAsync([Query] SandboxSearchOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a detonation's status (<c>GET api/v1/submit/{id}/</c>).</summary>
	/// <param name="id">The detonation's ID (see <see cref="SandboxReportLinks.Id"/>).</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The detonation.</returns>
	/// <remarks>
	/// The reference shows this path only as the <c>status</c> link of a submission. Unit-verified only.
	/// </remarks>
	[Get("api/v1/submit/{id}/")]
	Task<SandboxReport> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Gets a detonation's report (<c>GET api/v1/submit/{id}/report/</c>).</summary>
	/// <param name="id">The detonation's ID (see <see cref="SandboxReportLinks.Id"/>).</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The report, whose content depends on the sandbox vendor.</returns>
	/// <remarks>
	/// The reference shows this path only as the <c>detail</c> link of a submission. Unit-verified only.
	/// </remarks>
	[Get("api/v1/submit/{id}/report/")]
	Task<SandboxReportDetail> GetReportAsync(long id, CancellationToken cancellationToken);
}
