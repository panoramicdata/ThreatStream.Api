using System.Text.Json.Serialization;
using ThreatStream.Api.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>
/// A reference from one ThreatStream object to another: an owning organisation, investigation or user, an entity a threat
/// model entity is associated with (<c>campaigns</c>, <c>intelligence</c>, <c>ttps</c>...), a trusted circle, a lookup value
/// (<c>victims</c>, <c>soph_type</c>, <c>status</c>), an alias, or one item of a lookup list such as
/// <c>api/v1/victimtype/</c>.
/// </summary>
/// <remarks>
/// ThreatStream sends references as objects, bare IDs or text, and the API reference rarely says which. All of them read
/// into this type: a number (or numeric string) sets only <see cref="Id"/>, an API path (text starting <c>/api/</c>) sets
/// only <see cref="ResourceUri"/>, any other text sets only <see cref="Name"/>, and an object sets whichever of the
/// properties below it carries, keeping the rest in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
[JsonConverter(typeof(ThreatStreamReferenceConverter))]
public sealed class ThreatStreamReference : ThreatStreamObject
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

	/// <summary>The referenced object's API path (<c>resource_uri</c>), such as <c>/api/v1/organization/1/</c>.</summary>
	[JsonPropertyName("resource_uri")]
	public string? ResourceUri { get; init; }
}
