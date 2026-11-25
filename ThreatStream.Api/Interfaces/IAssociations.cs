using Refit;
using ThreatStream.Api.Models.Associations;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Bidirectional associations between threat model entities, and between threat model entities and observables
/// (<c>api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_add/</c> and <c>bulk_remove/</c>).
/// </summary>
/// <remarks>
/// Associations appear on the Associations tab of both entities' details pages. Neither endpoint has a documented rate
/// limit, so ThreatStream's default applies: one request per minute per user.
/// </remarks>
public interface IAssociations
{
	/// <summary>
	/// Associates an entity with entities or observables of another type
	/// (<c>POST api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_add/</c>).
	/// </summary>
	/// <param name="entityType">The type of the entity to add associations to (not <see cref="AssociationEntityType.Intelligence"/>).</param>
	/// <param name="entityId">The ID of that entity.</param>
	/// <param name="associatedEntityType">The type of the entities to associate it with; <see cref="AssociationEntityType.Intelligence"/> for observables.</param>
	/// <param name="request">The IDs of the entities to associate it with.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has accepted the associations.</returns>
	/// <remarks>
	/// <b>Side effect:</b> adds associations. Refused by a read-only client. The reference documents no response body.
	/// </remarks>
	[Post("api/v1/{entityType}/{entityId}/{associatedEntityType}/bulk_add/")]
	Task BulkAddAsync(AssociationEntityType entityType, long entityId, AssociationEntityType associatedEntityType, [Body] AssociationRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Removes associations between an entity and entities or observables of another type
	/// (<c>POST api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_remove/</c>).
	/// </summary>
	/// <param name="entityType">The type of the entity to remove associations from (not <see cref="AssociationEntityType.Intelligence"/>).</param>
	/// <param name="entityId">The ID of that entity.</param>
	/// <param name="associatedEntityType">The type of the associated entities; <see cref="AssociationEntityType.Intelligence"/> for observables.</param>
	/// <param name="request">The IDs of the associated entities to dissociate.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has removed the associations.</returns>
	/// <remarks>
	/// <b>Side effect:</b> removes associations. Refused by a read-only client. The reference documents no response body,
	/// and its text for this endpoint repeats the wording of <c>bulk_add/</c> ("adding").
	/// </remarks>
	[Post("api/v1/{entityType}/{entityId}/{associatedEntityType}/bulk_remove/")]
	Task BulkRemoveAsync(AssociationEntityType entityType, long entityId, AssociationEntityType associatedEntityType, [Body] AssociationRequest request, CancellationToken cancellationToken);
}
