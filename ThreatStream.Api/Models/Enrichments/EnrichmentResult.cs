using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>
/// One answer from a third-party enrichment service through <c>api/v1/integration_package/direct/</c>: the service's
/// status code and its results, as the service returned them.
/// </summary>
/// <remarks>
/// The results differ by service (AbuseIPDB returns <c>{"data": {...}}</c> with fields such as
/// <c>abuseConfidenceScore</c>; Shodan Database and Recorded Future their own documents, or a message on failure), so they
/// are kept as a <see cref="JsonElement"/> to read or deserialize as needed.
/// </remarks>
public sealed class EnrichmentResult : ThreatStreamObject
{
	/// <summary>The service's status code (<c>status_code</c>): 200 from AbuseIPDB and Shodan Database, 201 from Recorded Future on success.</summary>
	[JsonPropertyName("status_code")]
	public int? StatusCode { get; init; }

	/// <summary>The service's results or message (<c>results</c>), or <see langword="null"/> when there are none.</summary>
	[JsonPropertyName("results")]
	public JsonElement? Results { get; init; }
}
