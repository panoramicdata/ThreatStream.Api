using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>The actor sophistication types that actors refer to by ID (<c>api/v1/actorsophisticationtype/</c>).</summary>
public interface IActorSophisticationTypes
{
	/// <summary>
	/// Gets a page of actor sophistication types, from the STIX threat actor sophistication vocabulary
	/// (<c>GET api/v1/actorsophisticationtype/</c>).
	/// </summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of sophistication types.</returns>
	/// <remarks>
	/// Inferred: the reference names the path in the actor attribute table but does not document it, so the method and
	/// response shape follow ThreatStream's other lists. Unit-verified only.
	/// </remarks>
	[Get("api/v1/actorsophisticationtype/")]
	Task<ThreatStreamPage<ThreatModelReference>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
