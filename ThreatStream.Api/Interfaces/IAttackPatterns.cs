using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Attack patterns (<c>api/v1/attackpattern/</c>).</summary>
/// <remarks>
/// Endpoints without a documented rate limit allow one request per minute per user; <c>associated_with_intelligence</c>
/// allows 150. To search across threat model types, Anomali recommends <c>api/v1/threat_model_search/</c>.
/// </remarks>
public interface IAttackPatterns
{
	/// <summary>Gets a page of attack patterns (<c>GET api/v1/attackpattern/</c>).</summary>
	/// <param name="options">Paging, ordering and filters, or <see langword="null"/> for the first 20.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of attack patterns.</returns>
	/// <remarks>Anomali recommends <c>api/v1/threat_model_search/?model_type=attackpattern</c> for listing attack patterns.</remarks>
	[Get("api/v1/attackpattern/")]
	Task<ThreatStreamPage<AttackPattern>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates an attack pattern (<c>POST api/v1/attackpattern/</c>). Changes ThreatStream: refused by a read-only client.</summary>
	/// <param name="request">The new attack pattern; <see cref="AttackPatternCreateRequest.Name"/> is required.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created attack pattern, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Post("api/v1/attackpattern/")]
	Task<AttackPattern?> CreateAsync([Body] AttackPatternCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets an attack pattern's details, with up to 1000 associations (<c>GET api/v1/attackpattern/{id}/</c>).</summary>
	/// <param name="id">The attack pattern's ID.</param>
	/// <param name="options">The OnPrem-only detail parameters, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The attack pattern.</returns>
	[Get("api/v1/attackpattern/{id}/")]
	Task<AttackPattern> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the entities of one type associated with an attack pattern (<c>GET api/v1/attackpattern/{id}/{entity_type}/</c>), such as its
	/// malware (<c>api/v1/attackpattern/234673/malware/</c>).
	/// </summary>
	/// <param name="id">The attack pattern's ID.</param>
	/// <param name="entityType">The type of the associated entities, sent as its path name (<c>malware</c>, <c>campaign</c>...).</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of entities; each type's own attributes are in <see cref="ThreatStreamObject.AdditionalProperties"/>.</returns>
	/// <remarks>
	/// The reference shows only the <c>malware</c> example; the other path names are those of the type endpoints. For the
	/// associated observables use <see cref="ListIntelligenceAsync"/>.
	/// </remarks>
	[Get("api/v1/attackpattern/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelAssociatedEntity>> ListLinkedAsync(long id, ThreatModelType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes an attack pattern (<c>PATCH api/v1/attackpattern/{id}/</c>). Changes ThreatStream: refused by a read-only client.</summary>
	/// <param name="id">The attack pattern's ID.</param>
	/// <param name="request">The attributes to change; those left <see langword="null"/> are not sent.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The updated attack pattern, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	[Patch("api/v1/attackpattern/{id}/")]
	Task<AttackPattern?> UpdateAsync(long id, [Body] AttackPatternUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets the observables associated with an attack pattern (<c>GET api/v1/attackpattern/{id}/intelligence/</c>).</summary>
	/// <param name="id">The attack pattern's ID.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	[Get("api/v1/attackpattern/{id}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the attack patterns associated with an observable value (<c>GET api/v1/attackpattern/associated_with_intelligence/?value=</c>).
	/// Rate limit: 150 requests per minute.
	/// </summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of attack patterns.</returns>
	[Get("api/v1/attackpattern/associated_with_intelligence/")]
	Task<ThreatStreamPage<AttackPattern>> ListAssociatedWithValueAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the attack patterns associated with any of the given observables (<c>GET api/v1/attackpattern/associated_with_intelligence/?ids=</c>).
	/// Rate limit: 150 requests per minute.
	/// </summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and ordering, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of attack patterns.</returns>
	[Get("api/v1/attackpattern/associated_with_intelligence/")]
	Task<ThreatStreamPage<AttackPattern>> ListAssociatedWithIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);
}
