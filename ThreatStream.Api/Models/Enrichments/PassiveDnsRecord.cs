using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>One passive DNS resolution: a domain seen resolving to an IP address, by one provider, over a period.</summary>
public sealed class PassiveDnsRecord : ThreatStreamObject
{
	/// <summary>The domain (<c>domain</c>).</summary>
	[JsonPropertyName("domain")]
	public string? Domain { get; init; }

	/// <summary>The IP address it resolved to (<c>ip</c>).</summary>
	[JsonPropertyName("ip")]
	public string? Ip { get; init; }

	/// <summary>The DNS record type, such as <c>A</c> (<c>rrtype</c>).</summary>
	[JsonPropertyName("rrtype")]
	public string? RrType { get; init; }

	/// <summary>The passive DNS provider, such as Spamhaus or Farsight (<c>source</c>).</summary>
	[JsonPropertyName("source")]
	public string? Source { get; init; }

	/// <summary>When the resolution was first seen (<c>first_seen</c>).</summary>
	[JsonPropertyName("first_seen")]
	public DateTimeOffset? FirstSeen { get; init; }

	/// <summary>When the resolution was last seen (<c>last_seen</c>).</summary>
	[JsonPropertyName("last_seen")]
	public DateTimeOffset? LastSeen { get; init; }
}
