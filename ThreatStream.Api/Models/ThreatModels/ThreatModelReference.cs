using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A reference from a threat model entity to something else: an associated entity (<c>campaigns</c>,
/// <c>intelligence</c>, <c>ttps</c>...), a trusted circle, a lookup value (<c>victims</c>, <c>soph_type</c>,
/// <c>status</c>), an alias, the owning organisation or user, or one item of a lookup list such as
/// <c>api/v1/victimtype/</c>.
/// </summary>
/// <remarks>
/// The API reference documents these attributes as "related" without showing a response, and ThreatStream sends them
/// either as bare IDs or as objects. Both read into this type: a number (or numeric string) sets only <see cref="Id"/>,
/// any other string sets only <see cref="Name"/>, and an object sets whichever of the properties below it carries, keeping
/// the rest in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
[JsonConverter(typeof(ThreatModelReferenceConverter))]
public sealed class ThreatModelReference : ThreatStreamObject
{
	/// <summary>The referenced object's ID (<c>id</c>), when known.</summary>
	[JsonPropertyName("id")]
	public long? Id { get; init; }

	/// <summary>The referenced object's name (<c>name</c>), or the text ThreatStream sent instead of an object.</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The lookup value's text (<c>value</c>), such as <c>Health Care</c> for a victim type.</summary>
	[JsonPropertyName("value")]
	public string? Value { get; init; }

	/// <summary>The lookup value's display text (<c>display_name</c>).</summary>
	[JsonPropertyName("display_name")]
	public string? DisplayName { get; init; }

	/// <summary>The referenced object's API path (<c>resource_uri</c>).</summary>
	[JsonPropertyName("resource_uri")]
	public string? ResourceUri { get; init; }
}
