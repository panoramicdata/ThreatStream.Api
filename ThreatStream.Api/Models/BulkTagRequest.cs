using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>
/// The body of the bulk tagging endpoints for listed objects: observables (<c>POST api/v2/intelligence/bulk_tagging/</c>)
/// and threat model entities of one type (<c>POST api/v1/{t}/bulk_tagging/</c>).
/// </summary>
public sealed class BulkTagRequest : TagChangeRequest
{
	/// <summary>The IDs of the observables or entities to tag (<c>ids</c>).</summary>
	[JsonPropertyName("ids")]
	[JsonPropertyOrder(-1)]
	public required IReadOnlyList<long> Ids { get; init; }
}
