using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>An enrichment service that looks up IP addresses (the <c>name</c> of <c>api/v1/integration_package/direct/ip/{ip}/</c>).</summary>
public enum IpEnrichmentService
{
	/// <summary>A service this library does not know.</summary>
	Unknown = 0,

	/// <summary>AbuseIPDB (<c>AbuseIPDB</c>).</summary>
	[JsonStringEnumMemberName("AbuseIPDB")]
	AbuseIpDb = 1,

	/// <summary>Shodan Database (<c>Shodan Database</c>, sent URL-encoded).</summary>
	[JsonStringEnumMemberName("Shodan Database")]
	ShodanDatabase = 2
}
