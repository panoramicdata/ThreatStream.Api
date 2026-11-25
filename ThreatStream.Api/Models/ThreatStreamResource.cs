using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>The base of ThreatStream entities that have an <c>id</c> and a <c>resource_uri</c>.</summary>
public abstract class ThreatStreamResource : ThreatStreamObject
{
	/// <summary>The entity's ID (<c>id</c>).</summary>
	[JsonPropertyName("id")]
	public long Id { get; init; }

	/// <summary>The entity's own API path (<c>resource_uri</c>), such as <c>/api/v1/actor/123/</c>.</summary>
	[JsonPropertyName("resource_uri")]
	public string? ResourceUri { get; init; }
}
