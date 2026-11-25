using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The fields every threat model create or update request may send (<c>POST api/v1/{type}/</c>,
/// <c>PATCH api/v1/{type}/{id}/</c>). Fields left <see langword="null"/> are not sent, so an update changes only what it sets.
/// </summary>
public abstract class ThreatModelRequest
{
	/// <summary>
	/// Tags (<c>tags</c>), sent as plain strings as the reference's examples do. A threat bulletin takes tags with their
	/// own TLP in <c>tags_v2</c> instead.
	/// </summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<string>? Tags { get; init; }

	/// <summary>Who may see the entity (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>Whether the entity is public (<c>is_public</c>); ThreatStream's default is private.</summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; init; }

	/// <summary>The IDs of the trusted circles to share the entity with (<c>circles</c>).</summary>
	[JsonPropertyName("circles")]
	public IReadOnlyList<long>? Circles { get; init; }

	/// <summary>When the original source created the entity (<c>source_created</c>).</summary>
	[JsonPropertyName("source_created")]
	public DateTimeOffset? SourceCreated { get; init; }

	/// <summary>When the original source last changed the entity (<c>source_modified</c>).</summary>
	[JsonPropertyName("source_modified")]
	public DateTimeOffset? SourceModified { get; init; }

	/// <summary>The IDs of the observables to associate with the entity (<c>intelligence</c>).</summary>
	[JsonPropertyName("intelligence")]
	public IReadOnlyList<long>? Intelligence { get; init; }
}
