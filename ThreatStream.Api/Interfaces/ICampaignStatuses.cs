using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>The campaign statuses that campaigns refer to by ID (<c>api/v1/campaignstatus/</c>).</summary>
public interface ICampaignStatuses
{
	/// <summary>Gets a page of campaign statuses (<c>GET api/v1/campaignstatus/</c>).</summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of campaign statuses.</returns>
	/// <remarks>
	/// Inferred: the reference names the path in the campaign attribute table but does not document it, so the method and
	/// response shape follow ThreatStream's other lists. Unit-verified only.
	/// </remarks>
	[Get("api/v1/campaignstatus/")]
	Task<ThreatStreamPage<ThreatModelReference>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
