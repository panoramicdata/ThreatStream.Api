using Refit;
using ThreatStream.Api.Models.Enrichments;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Passive DNS enrichment (<c>api/v1/pdns/</c>), from Spamhaus, Farsight, OpenDNS Investigate, Google Threat Intelligence
/// and Microsoft Defender Threat Intelligence, as your organization subscribes to them.
/// </summary>
public interface IPassiveDns
{
	/// <summary>Gets the passive DNS resolutions of a domain or IP address (<c>GET api/v1/pdns/{domain or ip}/{value}/</c>).</summary>
	/// <param name="lookupType">Whether <paramref name="value"/> is a domain or an IP address.</param>
	/// <param name="value">The domain or IP address.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The resolutions found.</returns>
	/// <remarks>
	/// Limited to 20 requests per minute per user. The value is sent as one path segment, so any <c>/</c> in it is escaped
	/// (<c>%2F</c>); the reference's text mentions URL observables, but its examples cover only domains and IP addresses.
	/// </remarks>
	[Get("api/v1/pdns/{lookupType}/{value}/")]
	Task<PassiveDnsResult> GetAsync(PassiveDnsLookupType lookupType, string value, CancellationToken cancellationToken);
}
