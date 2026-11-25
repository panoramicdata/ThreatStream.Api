using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>The response of <c>GET api/v1/pdns/{type}/{value}/</c>.</summary>
/// <remarks>
/// The reference shows no example response. This model follows the shape ThreatStream integrations read: a
/// <c>success</c> flag and a <c>results</c> list of resolutions. Anything else stays in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class PassiveDnsResult : ThreatStreamObject
{
	/// <summary>Whether the lookup succeeded (<c>success</c>).</summary>
	[JsonPropertyName("success")]
	public bool? Success { get; init; }

	/// <summary>The resolutions found (<c>results</c>); empty when there are none.</summary>
	[JsonPropertyName("results")]
	public IReadOnlyList<PassiveDnsRecord> Results { get; init; } = [];
}
