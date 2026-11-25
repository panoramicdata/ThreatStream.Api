using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>The incident statuses that incidents refer to by ID (<c>api/v1/incidentstatustype/</c>).</summary>
public interface IIncidentStatusTypes
{
	/// <summary>Gets a page of incident statuses, such as open (2) or closed (7) (<c>GET api/v1/incidentstatustype/</c>).</summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of incident statuses.</returns>
	/// <remarks>
	/// Inferred: the reference names the path in the incident attribute table but does not document it, so the method and
	/// response shape follow ThreatStream's other lists. Unit-verified only. The status IDs above are from the reference's
	/// examples.
	/// </remarks>
	[Get("api/v1/incidentstatustype/")]
	Task<ThreatStreamPage<ThreatStreamReference>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
