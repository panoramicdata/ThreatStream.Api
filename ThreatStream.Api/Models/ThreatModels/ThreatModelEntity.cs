using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The attributes every threat model entity (actor, attack pattern, campaign, incident, malware, signature, threat
/// bulletin, TTP, vulnerability) shares.
/// </summary>
public abstract class ThreatModelEntity : ThreatStreamResource
{
	/// <summary>The entity's name, unique within the owning organisation, up to 255 characters (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The entity's free-form description (<c>description</c>).</summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }

	/// <summary>The entity's tags (<c>tags</c>); a tag ThreatStream sends as an object is kept as its JSON text.</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<string> Tags { get; init; } = [];

	/// <summary>Who may see the entity (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>Whether the entity is public rather than private or shared with trusted circles only (<c>is_public</c>).</summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; init; }

	/// <summary>The trusted circles the entity belongs to (<c>circles</c>), each an ID or an object.</summary>
	[JsonPropertyName("circles")]
	public IReadOnlyList<JsonElement> Circles { get; init; } = [];

	/// <summary>The entity's review stage (<c>publication_status</c>); only published entities are visible outside the organisation.</summary>
	[JsonPropertyName("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; init; }

	/// <summary>When the original source created the entity (<c>source_created</c>).</summary>
	[JsonPropertyName("source_created")]
	public DateTimeOffset? SourceCreated { get; init; }

	/// <summary>When the original source last changed the entity (<c>source_modified</c>).</summary>
	[JsonPropertyName("source_modified")]
	public DateTimeOffset? SourceModified { get; init; }

	/// <summary>When the entity was created on ThreatStream (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the entity was last changed on ThreatStream (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
