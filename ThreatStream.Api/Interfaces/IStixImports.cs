using Refit;
using ThreatStream.Api.Models.ObservableImports;

namespace ThreatStream.Api.Interfaces;

/// <summary>Imports of observables from STIX files (<c>api/v1/stix_import/</c>).</summary>
public interface IStixImports
{
	/// <summary>
	/// Imports the observables in a STIX file, XML or JSON (<c>POST api/v1/stix_import/</c>, <c>multipart/form-data</c>).
	/// </summary>
	/// <param name="request">The file and the settings for its observables.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream accepts the file.</returns>
	/// <remarks>
	/// Changes ThreatStream, so a read-only client refuses it. The reference documents no response body, so none is read.
	/// </remarks>
	[Post("api/v1/stix_import/")]
	Task ImportAsync([Body] StixImportRequest request, CancellationToken cancellationToken);
}
