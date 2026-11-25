using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Threat bulletins (<c>api/v1/tipreport/</c>).</summary>
public interface IThreatBulletins
{
	/// <summary>Lists threat bulletins (<c>GET api/v1/tipreport/</c>).</summary>
	/// <param name="options">Filters, paging and order, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of threat bulletins.</returns>
	/// <remarks>
	/// Anomali recommends the threat model search (<c>api/v1/threat_model_search/?model_type=tipreport</c>) for listing.
	/// One request per minute per user, ThreatStream's default for endpoints without a documented limit.
	/// </remarks>
	[Get("api/v1/tipreport/")]
	Task<ThreatStreamPage<ThreatBulletin>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a threat bulletin (<c>POST api/v1/tipreport/</c>).</summary>
	/// <param name="request">The new bulletin; its body may hold up to 100 MB of text.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created bulletin, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Post("api/v1/tipreport/")]
	Task<ThreatBulletin?> CreateAsync([Body] ThreatBulletinCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets a threat bulletin with its associations (<c>GET api/v1/tipreport/{id}/</c>).</summary>
	/// <param name="id">The bulletin's ID.</param>
	/// <param name="options">ThreatStream OnPrem options, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The bulletin.</returns>
	[Get("api/v1/tipreport/{id}/")]
	Task<ThreatBulletin> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Lists the entities of one type linked to a threat bulletin (<c>GET api/v1/tipreport/{id}/{entity_type}/</c>), such as
	/// its actors (<c>api/v1/tipreport/234673/actor/</c>).
	/// </summary>
	/// <param name="id">The bulletin's ID.</param>
	/// <param name="entityType">The type of the linked entities.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of linked entities.</returns>
	[Get("api/v1/tipreport/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelAssociatedEntity>> ListLinkedAsync(long id, ThreatModelType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes a threat bulletin (<c>PATCH api/v1/tipreport/{id}/</c>).</summary>
	/// <param name="id">The bulletin's ID.</param>
	/// <param name="request">The fields to change; those left <see langword="null"/> are kept.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The changed bulletin, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it. The body content type cannot be changed once set.</remarks>
	[Patch("api/v1/tipreport/{id}/")]
	Task<ThreatBulletin?> UpdateAsync(long id, [Body] ThreatBulletinUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Lists the observables associated with a threat bulletin (<c>GET api/v1/tipreport/{id}/intelligence/</c>).</summary>
	/// <param name="id">The bulletin's ID.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	/// <remarks>Observables associated through an import session may not be included; see <see cref="ThreatBulletin.ImportSessions"/>.</remarks>
	[Get("api/v1/tipreport/{id}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the threat bulletins associated with an observable value (<c>GET api/v1/tipreport/associated_with_intelligence/?value=</c>).</summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of threat bulletins.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/tipreport/associated_with_intelligence/")]
	Task<ThreatStreamPage<ThreatBulletin>> ListAssociatedWithValueAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the threat bulletins associated with any of the given observables (<c>GET api/v1/tipreport/associated_with_intelligence/?ids=</c>).</summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of threat bulletins.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/tipreport/associated_with_intelligence/")]
	Task<ThreatStreamPage<ThreatBulletin>> ListAssociatedWithIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Attaches a file to a threat bulletin (<c>POST api/v1/tipreport/{id}/attachment/</c>, multipart).</summary>
	/// <param name="id">The bulletin's ID.</param>
	/// <param name="request">The file and its name.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>ThreatStream's answer, which the reference does not document, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>
	/// Changes ThreatStream: a read-only client refuses it. The attachment is visible to every organisation that can see
	/// the bulletin.
	/// </remarks>
	[Post("api/v1/tipreport/{id}/attachment/")]
	Task<ThreatBulletinAttachment?> AddAttachmentAsync(long id, [Body] ThreatBulletinAttachmentRequest request, CancellationToken cancellationToken);
}
