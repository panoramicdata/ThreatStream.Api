using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Incidents (<c>api/v1/incident/</c>).</summary>
/// <remarks>
/// Endpoints without a documented rate limit allow one request per minute per user; <c>associated_with_intelligence</c>
/// allows 150. To search across threat model types, Anomali recommends <c>api/v1/threat_model_search/</c>.
/// </remarks>
public interface IIncidents
{
	/// <summary>Gets a page of incidents (<c>GET api/v1/incident/</c>).</summary>
	/// <param name="options">Paging, ordering and filters, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of incidents.</returns>
	/// <remarks>Anomali recommends <c>api/v1/threat_model_search/?model_type=incident</c> for listing incidents.</remarks>
	[Get("api/v1/incident/")]
	Task<ThreatStreamPage<Incident>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates an incident (<c>POST api/v1/incident/</c>). Changes ThreatStream: refused by a read-only client.</summary>
	/// <param name="request">The new incident; <see cref="IncidentCreateRequest.Name"/> is required.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created incident.</returns>
	[Post("api/v1/incident/")]
	Task<Incident> CreateAsync([Body] IncidentCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets an incident's details, with up to 1000 associations (<c>GET api/v1/incident/{id}/</c>).</summary>
	/// <param name="id">The incident's ID.</param>
	/// <param name="options">The OnPrem-only detail parameters, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The incident.</returns>
	[Get("api/v1/incident/{id}/")]
	Task<Incident> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes an incident (<c>PATCH api/v1/incident/{id}/</c>). Changes ThreatStream: refused by a read-only client.</summary>
	/// <param name="id">The incident's ID.</param>
	/// <param name="request">The attributes to change; those left <see langword="null"/> are not sent.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The updated incident.</returns>
	[Patch("api/v1/incident/{id}/")]
	Task<Incident> UpdateAsync(long id, [Body] IncidentUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the incidents associated with an observable value (<c>GET api/v1/incident/associated_with_intelligence/?value=</c>).
	/// Rate limit: 150 requests per minute.
	/// </summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of incidents.</returns>
	[Get("api/v1/incident/associated_with_intelligence/")]
	Task<ThreatStreamPage<Incident>> ListAssociatedWithValueAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the incidents associated with any of the given observables (<c>GET api/v1/incident/associated_with_intelligence/?ids=</c>).
	/// Rate limit: 150 requests per minute.
	/// </summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of incidents.</returns>
	[Get("api/v1/incident/associated_with_intelligence/")]
	Task<ThreatStreamPage<Incident>> ListAssociatedWithIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the entities of one type associated with an incident (<c>GET api/v1/incident/{id}/{entity_type}/</c>), such as its
	/// malware (<c>api/v1/incident/234673/malware/</c>).
	/// </summary>
	/// <param name="id">The incident's ID.</param>
	/// <param name="entityType">The type of the associated entities, sent as its path name (<c>malware</c>, <c>campaign</c>...).</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of entities; each type's own attributes are in <see cref="ThreatStreamObject.AdditionalProperties"/>.</returns>
	/// <remarks>
	/// The reference shows only the <c>malware</c> example; the other path names are those of the type endpoints. For the
	/// associated observables use <see cref="ListIntelligenceAsync"/>.
	/// </remarks>
	[Get("api/v1/incident/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelAssociatedEntity>> ListLinkedAsync(long id, ThreatModelType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets the observables associated with an incident (<c>GET api/v1/incident/{id}/intelligence/</c>).</summary>
	/// <param name="id">The incident's ID.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	/// <remarks>The reference spells the path <c>/ api/v1/incident/{id}/intelligence/</c>, with a stray space; this follows the other types.</remarks>
	[Get("api/v1/incident/{id}/intelligence/")]
	Task<ThreatStreamPage<ThreatModelObservable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);
}
