using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The attributes every threat model entity (actor, attack pattern, campaign, incident, malware, signature, threat
/// bulletin, TTP, vulnerability) shares. Each type's model adds its own.
/// </summary>
/// <remarks>
/// Attributes the reference calls "related" (associations, circles, lookup values) arrive as IDs or as objects; both read
/// into <see cref="ThreatModelReference"/>. Each list is empty, never <see langword="null"/>, when ThreatStream sends
/// none.
/// </remarks>
public abstract class ThreatModelEntity : ThreatStreamResource
{
	/// <summary>The entity's name, unique within its organisation, up to 255 characters (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>Free-form text about the entity (<c>description</c>).</summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }

	/// <summary>The entity's tags (<c>tags</c>), sent as plain names or as tag objects.</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag> Tags { get; init; } = [];

	/// <summary>The entity's tags with their TLP and owning organisation (<c>tags_v2</c>).</summary>
	[JsonPropertyName("tags_v2")]
	public IReadOnlyList<ThreatStreamTag> TagsV2 { get; init; } = [];

	/// <summary>The entity's Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>Whether the entity is public; <see langword="false"/> when private or shared with trusted circles (<c>is_public</c>).</summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; init; }

	/// <summary>The trusted circles the entity is shared with (<c>circles</c>).</summary>
	[JsonPropertyName("circles")]
	public IReadOnlyList<ThreatModelReference> Circles { get; init; } = [];

	/// <summary>The entity's review state (<c>publication_status</c>).</summary>
	[JsonPropertyName("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; init; }

	/// <summary>When the original source created the entity (<c>source_created</c>).</summary>
	[JsonPropertyName("source_created")]
	public DateTimeOffset? SourceCreated { get; init; }

	/// <summary>When the original source last changed the entity (<c>source_modified</c>).</summary>
	[JsonPropertyName("source_modified")]
	public DateTimeOffset? SourceModified { get; init; }

	/// <summary>When the entity was created on ThreatStream (<c>created_ts</c>); set by ThreatStream.</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the entity was last changed on ThreatStream (<c>modified_ts</c>); set by ThreatStream.</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }

	/// <summary>When the entity was published on ThreatStream (<c>published_ts</c>); set by ThreatStream.</summary>
	[JsonPropertyName("published_ts")]
	public DateTimeOffset? PublishedTs { get; init; }

	/// <summary>The ID of the organisation that created the entity (<c>organization_id</c>).</summary>
	[JsonPropertyName("organization_id")]
	public long? OrganizationId { get; init; }

	/// <summary>The organisation that created the entity (<c>organization</c>).</summary>
	[JsonPropertyName("organization")]
	public ThreatModelReference? Organization { get; init; }

	/// <summary>The ID of the user that created the entity (<c>owner_user_id</c>).</summary>
	[JsonPropertyName("owner_user_id")]
	public long? OwnerUserId { get; init; }

	/// <summary>The user that created the entity (<c>owner_user</c>).</summary>
	[JsonPropertyName("owner_user")]
	public ThreatModelReference? OwnerUser { get; init; }

	/// <summary>The ID of the threat feed that provided the entity (<c>feed_id</c>).</summary>
	[JsonPropertyName("feed_id")]
	public long? FeedId { get; init; }

	/// <summary>The entity's STIX UUID (<c>uuid</c>).</summary>
	[JsonPropertyName("uuid")]
	public string? Uuid { get; init; }

	/// <summary>Whether the owning user and organisation are anonymised (<c>is_anonymous</c>).</summary>
	[JsonPropertyName("is_anonymous")]
	public bool? IsAnonymous { get; init; }

	/// <summary>Whether the entity can be cloned (<c>is_cloneable</c>), as ThreatStream sends it (documented as a string).</summary>
	[JsonPropertyName("is_cloneable")]
	public string? IsCloneable { get; init; }

	/// <summary>Whether the calling user has starred the entity (<c>starred_by_me</c>).</summary>
	[JsonPropertyName("starred_by_me")]
	public bool? StarredByMe { get; init; }

	/// <summary>How many users have starred the entity (<c>starred_total_count</c>).</summary>
	[JsonPropertyName("starred_total_count")]
	public int? StarredTotalCount { get; init; }

	/// <summary>Whether the calling user watches the entity (<c>watched_by_me</c>).</summary>
	[JsonPropertyName("watched_by_me")]
	public bool? WatchedByMe { get; init; }

	/// <summary>How many users watch the entity (<c>watched_total_count</c>).</summary>
	[JsonPropertyName("watched_total_count")]
	public int? WatchedTotalCount { get; init; }

	/// <summary>The workgroups the entity is visible to (<c>workgroups</c>).</summary>
	[JsonPropertyName("workgroups")]
	public IReadOnlyList<ThreatModelReference> Workgroups { get; init; } = [];

	/// <summary>On ThreatStream OnPrem, whether the entity is remote, stored on the cloud (<c>remote_api</c>).</summary>
	[JsonPropertyName("remote_api")]
	public bool? RemoteApi { get; init; }

	/// <summary>The observables associated with the entity (<c>intelligence</c>).</summary>
	[JsonPropertyName("intelligence")]
	public IReadOnlyList<ThreatModelReference> Intelligence { get; init; } = [];

	/// <summary>The signatures associated with the entity (<c>signatures</c>).</summary>
	[JsonPropertyName("signatures")]
	public IReadOnlyList<ThreatModelReference> Signatures { get; init; } = [];

	/// <summary>The TTPs associated with the entity (<c>ttps</c>).</summary>
	[JsonPropertyName("ttps")]
	public IReadOnlyList<ThreatModelReference> Ttps { get; init; } = [];
}
