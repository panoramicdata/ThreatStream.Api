using Refit;
using ThreatStream.Api.Models.Enrichments;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Third-party enrichments through the Integration Package API (<c>api/v1/integration_package/direct/</c>): AbuseIPDB,
/// Shodan Database and Recorded Future.
/// </summary>
/// <remarks>
/// Only services your organization subscribes to return data. None of these endpoints has a documented rate limit, so
/// ThreatStream's default applies: one request per minute per user.
/// </remarks>
public interface IEnrichments
{
	/// <summary>
	/// Looks up an IP address with AbuseIPDB or Shodan Database
	/// (<c>GET api/v1/integration_package/direct/ip/{ip}/?name=...</c>).
	/// </summary>
	/// <param name="ipAddress">The IP address. Some services do not support IPv6.</param>
	/// <param name="service">The service to ask.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>
	/// The service's answers: one for AbuseIPDB; Shodan Database may call several of its endpoints and return one answer for
	/// each, some without data.
	/// </returns>
	/// <remarks>
	/// The reference's example URLs spell Shodan Database's name <c>Shodan+Database</c>; this method sends
	/// <c>Shodan%20Database</c>, the same text in a query string. The reference's AbuseIPDB example response is not valid
	/// JSON (a missing comma and a trailing one); ThreatStream's real response is a JSON array.
	/// </remarks>
	[Get("api/v1/integration_package/direct/ip/{ipAddress}/")]
	Task<IReadOnlyList<EnrichmentResult>> GetIpAsync(string ipAddress, [AliasAs("name")] IpEnrichmentService service, CancellationToken cancellationToken);

	/// <summary>Looks up a value with Recorded Future (<c>POST api/v1/integration_package/direct/</c>), the form the reference recommends.</summary>
	/// <param name="lookup">The service, and the kind and value to look up.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The service's answers; <see cref="EnrichmentResult.StatusCode"/> is 201 on success.</returns>
	/// <remarks>
	/// Although it is a POST, the lookup changes nothing, so a read-only client allows it.
	/// </remarks>
	[Post("api/v1/integration_package/direct/")]
	Task<IReadOnlyList<EnrichmentResult>> LookupAsync([Body] DirectEnrichmentLookup lookup, CancellationToken cancellationToken);

	/// <summary>Looks up a value with Recorded Future through the query string (<c>GET api/v1/integration_package/direct/</c>).</summary>
	/// <param name="lookup">The service, and the kind and value to look up.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The service's answers; <see cref="EnrichmentResult.StatusCode"/> is 201 on success.</returns>
	/// <remarks>The reference recommends <see cref="LookupAsync"/>, the POST form, over this one.</remarks>
	[Get("api/v1/integration_package/direct/")]
	Task<IReadOnlyList<EnrichmentResult>> LookupWithGetAsync([Query] DirectEnrichmentLookup lookup, CancellationToken cancellationToken);
}
