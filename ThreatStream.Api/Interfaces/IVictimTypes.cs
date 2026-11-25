using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>The victim types that actors, campaigns and incidents refer to by ID (<c>api/v1/victimtype/</c>).</summary>
public interface IVictimTypes
{
	/// <summary>Gets a page of victim types, such as <c>Health Care</c> (13) or <c>Insurance</c> (15) (<c>GET api/v1/victimtype/</c>).</summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of victim types.</returns>
	/// <remarks>
	/// Inferred: the reference names the path in the actor, campaign and incident attribute tables but does not document it,
	/// so the method and response shape follow ThreatStream's other lists. Unit-verified only.
	/// </remarks>
	[Get("api/v1/victimtype/")]
	Task<ThreatStreamPage<ThreatModelReference>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
