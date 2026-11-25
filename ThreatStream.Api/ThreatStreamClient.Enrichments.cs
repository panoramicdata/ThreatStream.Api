using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Third-party enrichments: AbuseIPDB, Shodan Database and Recorded Future (<c>api/v1/integration_package/direct/</c>).</summary>
	public IEnrichments Enrichments => field ??= For<IEnrichments>();

	/// <summary>Passive DNS enrichment (<c>api/v1/pdns/</c>).</summary>
	public IPassiveDns PassiveDns => field ??= For<IPassiveDns>();
}
