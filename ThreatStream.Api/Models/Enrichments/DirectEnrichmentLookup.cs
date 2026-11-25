using Refit;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Enrichments;

/// <summary>
/// A Recorded Future lookup through <c>api/v1/integration_package/direct/</c>: sent as the JSON body of the POST form, or
/// as the query string of the GET form.
/// </summary>
public sealed class DirectEnrichmentLookup
{
	/// <summary>The enrichment service (<c>name</c>); the reference documents <c>Recorded Future</c>, the default.</summary>
	[JsonPropertyName("name")]
	[AliasAs("name")]
	public string Name { get; init; } = "Recorded Future";

	/// <summary>The kind of value to look up (<c>type</c>).</summary>
	[JsonPropertyName("type")]
	[AliasAs("type")]
	public required EnrichmentValueType Type { get; init; }

	/// <summary>The IP address, URL, domain or hash to look up (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	[AliasAs("value")]
	public required string Value { get; init; }
}
