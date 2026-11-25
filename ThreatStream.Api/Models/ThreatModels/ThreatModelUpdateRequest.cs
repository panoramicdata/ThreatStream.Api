using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The attributes every threat model entity's create and update (PATCH) bodies share. Only the properties that are set are
/// sent, so an update changes just those.
/// </summary>
public abstract class ThreatModelUpdateRequest
{
	/// <summary>The entity's name, unique within the organisation, up to 255 characters (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>Free-form text about the entity (<c>description</c>).</summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }

	/// <summary>Tags as plain names (<c>tags</c>), the form the reference's examples send; they take ThreatStream's default TLP.</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<string>? Tags { get; init; }

	/// <summary>Tags with their own TLP (<c>tags_v2</c>), such as <c>{"name": "my_private_tag", "tlp": "red"}</c>.</summary>
	[JsonPropertyName("tags_v2")]
	public IReadOnlyList<ThreatStreamTag>? TagsV2 { get; init; }

	/// <summary>The entity's Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>Whether the entity is public (<c>is_public</c>); ThreatStream's default is <see langword="false"/>.</summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; init; }

	/// <summary>The IDs of the trusted circles to share the entity with (<c>circles</c>).</summary>
	[JsonPropertyName("circles")]
	public IReadOnlyList<long>? Circles { get; init; }

	/// <summary>The entity's review state (<c>publication_status</c>).</summary>
	[JsonPropertyName("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; init; }

	/// <summary>When the original source created the entity (<c>source_created</c>).</summary>
	[JsonPropertyName("source_created")]
	public DateTimeOffset? SourceCreated { get; init; }

	/// <summary>When the original source last changed the entity (<c>source_modified</c>).</summary>
	[JsonPropertyName("source_modified")]
	public DateTimeOffset? SourceModified { get; init; }

	/// <summary>The IDs of the observables to associate (<c>intelligence</c>).</summary>
	[JsonPropertyName("intelligence")]
	public IReadOnlyList<long>? Intelligence { get; init; }

	/// <summary>The IDs of the signatures to associate (<c>signatures</c>).</summary>
	[JsonPropertyName("signatures")]
	public IReadOnlyList<long>? Signatures { get; init; }

	/// <summary>The IDs of the TTPs to associate (<c>ttps</c>).</summary>
	[JsonPropertyName("ttps")]
	public IReadOnlyList<long>? Ttps { get; init; }
}
