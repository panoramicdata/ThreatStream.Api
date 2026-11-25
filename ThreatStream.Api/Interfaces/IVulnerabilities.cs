using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Vulnerability threat model entities (<c>api/v1/vulnerability/</c>).</summary>
public interface IVulnerabilities
{
	/// <summary>Lists vulnerabilities (<c>GET api/v1/vulnerability/</c>).</summary>
	/// <param name="options">Filters, paging and order, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of vulnerabilities.</returns>
	/// <remarks>
	/// Anomali recommends the threat model search (<c>api/v1/threat_model_search/?model_type=vulnerability</c>) for listing.
	/// One request per minute per user, ThreatStream's default for endpoints without a documented limit.
	/// </remarks>
	[Get("api/v1/vulnerability/")]
	Task<ThreatStreamPage<Vulnerability>> ListAsync([Query] ThreatModelListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a vulnerability (<c>POST api/v1/vulnerability/</c>).</summary>
	/// <param name="request">The new vulnerability, with a name unique within your organisation.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The created vulnerability.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Post("api/v1/vulnerability/")]
	Task<Vulnerability> CreateAsync([Body] VulnerabilityCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Gets a vulnerability with its associations (<c>GET api/v1/vulnerability/{id}/</c>).</summary>
	/// <param name="id">The vulnerability's ID.</param>
	/// <param name="options">ThreatStream OnPrem options, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The vulnerability.</returns>
	[Get("api/v1/vulnerability/{id}/")]
	Task<Vulnerability> GetAsync(long id, [Query] ThreatModelDetailOptions? options, CancellationToken cancellationToken);

	/// <summary>Changes a vulnerability (<c>PATCH api/v1/vulnerability/{id}/</c>).</summary>
	/// <param name="id">The vulnerability's ID.</param>
	/// <param name="request">The fields to change; those left <see langword="null"/> are kept.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The changed vulnerability.</returns>
	/// <remarks>Changes ThreatStream: a read-only client refuses it.</remarks>
	[Patch("api/v1/vulnerability/{id}/")]
	Task<Vulnerability> UpdateAsync(long id, [Body] VulnerabilityUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Lists the vulnerabilities associated with an observable value (<c>GET api/v1/vulnerability/associated_with_intelligence/?value=</c>).</summary>
	/// <param name="value">The observable's value, such as <c>google.com</c>.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of vulnerabilities.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/vulnerability/associated_with_intelligence/")]
	Task<ThreatStreamPage<Vulnerability>> ListAssociatedWithIntelligenceAsync([AliasAs("value")] string value, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the vulnerabilities associated with any of the given observables (<c>GET api/v1/vulnerability/associated_with_intelligence/?ids=</c>).</summary>
	/// <param name="ids">The observables' IDs, sent comma-separated.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of vulnerabilities.</returns>
	/// <remarks>150 requests per minute per user.</remarks>
	[Get("api/v1/vulnerability/associated_with_intelligence/")]
	Task<ThreatStreamPage<Vulnerability>> ListAssociatedWithIntelligenceByIdsAsync([AliasAs("ids")] IReadOnlyList<long> ids, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Lists the entities of one type linked to a vulnerability (<c>GET api/v1/vulnerability/{id}/{entity_type}/</c>), such
	/// as its malware (<c>api/v1/vulnerability/234673/malware/</c>).
	/// </summary>
	/// <param name="id">The vulnerability's ID.</param>
	/// <param name="entityType">The type of the linked entities.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of linked entities.</returns>
	[Get("api/v1/vulnerability/{id}/{entityType}/")]
	Task<ThreatStreamPage<ThreatModelLinkedEntity>> ListLinkedEntitiesAsync(long id, ThreatModelEntityType entityType, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists the observables associated with a vulnerability (<c>GET api/v1/vulnerability/{id}/intelligence/</c>).</summary>
	/// <param name="id">The vulnerability's ID.</param>
	/// <param name="options">Paging and order, or <see langword="null"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of observables.</returns>
	[Get("api/v1/vulnerability/{id}/intelligence/")]
	Task<ThreatStreamPage<Observable>> ListIntelligenceAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);
}
