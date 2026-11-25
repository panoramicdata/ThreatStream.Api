using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>
/// A tag on an observable or threat model entity: <c>{"name": "...", "tlp": "red"}</c> when sent, with ThreatStream's
/// <c>id</c> and owning organisation when read.
/// </summary>
public sealed class ThreatStreamTag : ThreatStreamObject
{
	/// <summary>The tag's ID (<c>id</c>), when read; ThreatStream sends it as text.</summary>
	[JsonPropertyName("id")]
	public string? Id { get; init; }

	/// <summary>The tag text (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	/// <summary>Who may see the tag (<c>tlp</c>); <see langword="null"/> to leave it to ThreatStream's default.</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>The ID of the organisation that added the tag (<c>org_id</c>), when read.</summary>
	[JsonPropertyName("org_id")]
	public long? OrgId { get; init; }
}
