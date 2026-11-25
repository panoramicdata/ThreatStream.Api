using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>The kind of value a passive DNS lookup takes (the first segment after <c>api/v1/pdns/</c>).</summary>
public enum PassiveDnsLookupType
{
	/// <summary>A kind this library does not know.</summary>
	Unknown = 0,

	/// <summary>A domain (<c>domain</c>): the IP addresses it has resolved to.</summary>
	[JsonStringEnumMemberName("domain")]
	Domain = 1,

	/// <summary>An IP address (<c>ip</c>): the domains that have resolved to it.</summary>
	[JsonStringEnumMemberName("ip")]
	Ip = 2
}
