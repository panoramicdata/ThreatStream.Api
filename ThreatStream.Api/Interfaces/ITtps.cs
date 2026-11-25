using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Tactics, techniques and procedures (<c>api/v1/ttp/</c>).</summary>
public interface ITtps
{
	/// <summary>Lists TTPs (<c>GET api/v1/ttp/</c>).</summary>
	/// <param name="options">Filters, paging and order, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of TTPs.</returns>
	/// <remarks>
	/// Anomali recommends the threat model search (<c>api/v1/threat_model_search/?model_type=ttp</c>) for listing.
	/// One request per minute per user, ThreatStream's default for endpoints without a documented limit.
	/// </remarks>
	[Get("api/v1/ttp/")]
	Task<ThreatStreamPage<Ttp>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a TTP (<c>POST api/v1/ttp/</c>).</summary>
	/// <param name="request">The new TTP, with a name unique within your organisation.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created TTP, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Post("api/v1/ttp/")]
	Task<Ttp?> CreateAsync([Body] TtpCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets a TTP with its associations (<c>GET api/v1/ttp/{id}/</c>).</summary>
	/// <param name="id">The TTP's ID.</param>
	/// <param name="options">ThreatStream OnPrem options, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The TTP.</returns>
	[Get("api/v1/ttp/{id}/")]
	Task<Ttp> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes a TTP (<c>PATCH api/v1/ttp/{id}/</c>).</summary>
	/// <param name="id">The TTP's ID.</param>
	/// <param name="request">The fields to change; those left <see langword="null"/> are kept.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The changed TTP, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Patch("api/v1/ttp/{id}/")]
	Task<Ttp?> UpdateAsync(long id, [Body] TtpUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Lists the TTPs associated with an observable value (<c>GET api/v1/ttp/associated_with_intelligence/?value=</c>).</summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of TTPs.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/ttp/associated_with_intelligence/")]
	Task<ThreatStreamPage<Ttp>> ListAssociatedWithValueAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the TTPs associated with any of the given observables (<c>GET api/v1/ttp/associated_with_intelligence/?ids=</c>).</summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of TTPs.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/ttp/associated_with_intelligence/")]
	Task<ThreatStreamPage<Ttp>> ListAssociatedWithIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Lists the entities of one type linked to a TTP (<c>GET api/v1/ttp/{id}/{entity_type}/</c>), such as its actors
	/// (<c>api/v1/ttp/234673/actor/</c>).
	/// </summary>
	/// <param name="id">The TTP's ID.</param>
	/// <param name="entityType">The type of the linked entities.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of linked entities.</returns>
	[Get("api/v1/ttp/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelAssociatedEntity>> ListLinkedAsync(long id, ThreatModelType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the observables associated with a TTP (<c>GET api/v1/ttp/{id}/intelligence/</c>).</summary>
	/// <param name="id">The TTP's ID.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	[Get("api/v1/ttp/{id}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);
}
