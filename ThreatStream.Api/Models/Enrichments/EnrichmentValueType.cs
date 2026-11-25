using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>The kind of value a <see cref="DirectEnrichmentLookup"/> looks up (<c>type</c>).</summary>
public enum EnrichmentValueType
{
	/// <summary>A kind this library does not know.</summary>
	Unknown = 0,

	/// <summary>An IP address (<c>ip</c>).</summary>
	[JsonStringEnumMemberName("ip")]
	Ip = 1,

	/// <summary>A URL (<c>url</c>).</summary>
	[JsonStringEnumMemberName("url")]
	Url = 2,

	/// <summary>A domain (<c>domain</c>).</summary>
	[JsonStringEnumMemberName("domain")]
	Domain = 3,

	/// <summary>A file hash (<c>hash</c>).</summary>
	[JsonStringEnumMemberName("hash")]
	Hash = 4
}
