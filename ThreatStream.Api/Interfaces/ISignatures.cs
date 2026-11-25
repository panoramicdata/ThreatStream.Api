using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Signature threat model entities (<c>api/v1/signature/</c>) and signature types (<c>api/v1/signature_type/</c>).</summary>
public interface ISignatures
{
	/// <summary>Lists signatures (<c>GET api/v1/signature/</c>).</summary>
	/// <param name="options">Filters, paging and order, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of signatures.</returns>
	/// <remarks>
	/// Anomali recommends the threat model search (<c>api/v1/threat_model_search/?model_type=signature</c>) for listing,
	/// which can also filter by signature type. One request per minute per user, ThreatStream's default for endpoints
	/// without a documented limit.
	/// </remarks>
	[Get("api/v1/signature/")]
	Task<ThreatStreamPage<Signature>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a signature (<c>POST api/v1/signature/</c>).</summary>
	/// <param name="request">The new signature, with a name unique within your organisation and its type's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created signature, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Post("api/v1/signature/")]
	Task<Signature?> CreateAsync([Body] SignatureCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets a signature with its associations (<c>GET api/v1/signature/{id}/</c>).</summary>
	/// <param name="id">The signature's ID.</param>
	/// <param name="options">ThreatStream OnPrem options, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The signature.</returns>
	[Get("api/v1/signature/{id}/")]
	Task<Signature> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes a signature (<c>PATCH api/v1/signature/{id}/</c>).</summary>
	/// <param name="id">The signature's ID.</param>
	/// <param name="request">The fields to change; those left <see langword="null"/> are kept.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The changed signature, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Patch("api/v1/signature/{id}/")]
	Task<Signature?> UpdateAsync(long id, [Body] SignatureUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Lists the signatures associated with an observable value (<c>GET api/v1/signature/associated_with_intelligence/?value=</c>).</summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of signatures.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/signature/associated_with_intelligence/")]
	Task<ThreatStreamPage<Signature>> ListAssociatedWithValueAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the signatures associated with any of the given observables (<c>GET api/v1/signature/associated_with_intelligence/?ids=</c>).</summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of signatures.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/signature/associated_with_intelligence/")]
	Task<ThreatStreamPage<Signature>> ListAssociatedWithIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Lists the entities of one type linked to a signature (<c>GET api/v1/signature/{id}/{entity_type}/</c>), such as its
	/// actors (<c>api/v1/signature/234673/actor/</c>).
	/// </summary>
	/// <param name="id">The signature's ID.</param>
	/// <param name="entityType">The type of the linked entities.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of linked entities.</returns>
	[Get("api/v1/signature/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelAssociatedEntity>> ListLinkedAsync(long id, ThreatModelType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the observables associated with a signature (<c>GET api/v1/signature/{id}/intelligence/</c>).</summary>
	/// <param name="id">The signature's ID.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	[Get("api/v1/signature/{id}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the signature types and the IDs a new signature names its type by (<c>GET api/v1/signature_type/</c>).</summary>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of signature types: Bro, Carbon Black Query, ClamAV, Custom, CybOX, OpenIOC, RSA NetWitness, Snort, Splunk Query, Suricata and YARA.</returns>
	/// <remarks>
	/// The reference names this endpoint (in the <c>s_type</c> attribute) but does not document it; the method and response
	/// shape are inferred and unit-verified only.
	/// </remarks>
	[Get("api/v1/signature_type/")]
	Task<ThreatStreamPage<ThreatStreamReference>> ListTypesAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
