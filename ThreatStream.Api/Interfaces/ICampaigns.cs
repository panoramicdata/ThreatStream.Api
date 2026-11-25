using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Campaigns (<c>api/v1/campaign/</c>).</summary>
/// <remarks>
/// Endpoints without a documented rate limit allow one request per minute per user; <c>associated_with_intelligence</c>
/// allows 150. To search across threat model types, Anomali recommends <c>api/v1/threat_model_search/</c>.
/// </remarks>
public interface ICampaigns
{
	/// <summary>Gets a page of campaigns (<c>GET api/v1/campaign/</c>).</summary>
	/// <param name="options">Paging, ordering and filters, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of campaigns.</returns>
	/// <remarks>Anomali recommends <c>api/v1/threat_model_search/?model_type=campaign</c> for listing campaigns.</remarks>
	[Get("api/v1/campaign/")]
	Task<ThreatStreamPage<Campaign>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a campaign (<c>POST api/v1/campaign/</c>). Changes ThreatStream: refused by a read-only client.</summary>
	/// <param name="request">The new campaign; <see cref="CampaignCreateRequest.Name"/> is required.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created campaign, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Post("api/v1/campaign/")]
	Task<Campaign?> CreateAsync([Body] CampaignCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets a campaign's details, with up to 1000 associations (<c>GET api/v1/campaign/{id}/</c>).</summary>
	/// <param name="id">The campaign's ID.</param>
	/// <param name="options">The OnPrem-only detail parameters, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The campaign.</returns>
	[Get("api/v1/campaign/{id}/")]
	Task<Campaign> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes a campaign (<c>PATCH api/v1/campaign/{id}/</c>). Changes ThreatStream: refused by a read-only client.</summary>
	/// <param name="id">The campaign's ID.</param>
	/// <param name="request">The attributes to change; those left <see langword="null"/> are not sent.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The updated campaign, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Patch("api/v1/campaign/{id}/")]
	Task<Campaign?> UpdateAsync(long id, [Body] CampaignUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the campaigns associated with an observable value (<c>GET api/v1/campaign/associated_with_intelligence/?value=</c>).
	/// Rate limit: 150 requests per minute.
	/// </summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of campaigns.</returns>
	[Get("api/v1/campaign/associated_with_intelligence/")]
	Task<ThreatStreamPage<Campaign>> ListAssociatedWithValueAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the campaigns associated with any of the given observables (<c>GET api/v1/campaign/associated_with_intelligence/?ids=</c>).
	/// Rate limit: 150 requests per minute.
	/// </summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of campaigns.</returns>
	[Get("api/v1/campaign/associated_with_intelligence/")]
	Task<ThreatStreamPage<Campaign>> ListAssociatedWithIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the entities of one type associated with a campaign (<c>GET api/v1/campaign/{id}/{entity_type}/</c>), such as its
	/// malware (<c>api/v1/campaign/234673/malware/</c>).
	/// </summary>
	/// <param name="id">The campaign's ID.</param>
	/// <param name="entityType">The type of the associated entities, sent as its path name (<c>malware</c>, <c>campaign</c>...).</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of entities; each type's own attributes are in <see cref="ThreatStreamObject.AdditionalProperties"/>.</returns>
	/// <remarks>
	/// The reference shows only the <c>malware</c> example; the other path names are those of the type endpoints. For the
	/// associated observables use <see cref="ListIntelligenceAsync"/>.
	/// </remarks>
	[Get("api/v1/campaign/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelAssociatedEntity>> ListLinkedAsync(long id, ThreatModelType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets the observables associated with a campaign (<c>GET api/v1/campaign/{id}/intelligence/</c>).</summary>
	/// <param name="id">The campaign's ID.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	[Get("api/v1/campaign/{id}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);
}
