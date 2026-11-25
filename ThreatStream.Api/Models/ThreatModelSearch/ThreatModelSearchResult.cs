using System.Text.Json.Serialization;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Models.ThreatModelSearch;

/// <summary>
/// One threat model entity found by a threat model search (<c>api/v1/threat_model_search/</c>): an actor, attack pattern,
/// campaign, incident, malware, signature, threat bulletin, TTP or vulnerability. <see cref="ModelType"/> says which.
/// </summary>
/// <remarks>
/// The attributes common to every type are modelled; type-specific ones (aliases, a campaign's status and dates, a
/// signature's type...) and the owning <c>organization</c> stay in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// Read the full entity with its type's own endpoint, using <see cref="ThreatStreamResource.Id"/>.
/// </remarks>
public sealed class ThreatModelSearchResult : ThreatStreamResource
{
	/// <summary>The entity type (<c>model_type</c>), such as <see cref="ThreatModelType.Actor"/>; a type this library does not know reads as <see cref="ThreatModelType.Unknown"/>.</summary>
	[JsonPropertyName("model_type")]
	public ThreatModelType? ModelType { get; init; }

	/// <summary>The entity's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The publication status (<c>publication_status</c>).</summary>
	[JsonPropertyName("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; init; }

	/// <summary>The Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>Whether it is public (<c>is_public</c>).</summary>
	[JsonPropertyName("is_public")]
	public bool? IsPublic { get; init; }

	/// <summary>The tags on it (<c>tags</c>), sent as plain names or as tag objects.</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag> Tags { get; init; } = [];

	/// <summary>The feed that provided it (<c>feed_id</c>).</summary>
	[JsonPropertyName("feed_id")]
	public long? FeedId { get; init; }

	/// <summary>The organisation that owns it (<c>organization_id</c>).</summary>
	[JsonPropertyName("organization_id")]
	public long? OrganizationId { get; init; }

	/// <summary>The user who created it (<c>owner_user_id</c>).</summary>
	[JsonPropertyName("owner_user_id")]
	public long? OwnerUserId { get; init; }

	/// <summary>The email address of the user who created it (<c>created_by</c>).</summary>
	[JsonPropertyName("created_by")]
	public string? CreatedBy { get; init; }

	/// <summary>When it was created on ThreatStream (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When it last changed (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }

	/// <summary>When it was published (<c>published_ts</c>).</summary>
	[JsonPropertyName("published_ts")]
	public DateTimeOffset? PublishedTs { get; init; }

	/// <summary>When its source created it (<c>source_created</c>).</summary>
	[JsonPropertyName("source_created")]
	public DateTimeOffset? SourceCreated { get; init; }

	/// <summary>When its source last changed it (<c>source_modified</c>).</summary>
	[JsonPropertyName("source_modified")]
	public DateTimeOffset? SourceModified { get; init; }

	/// <summary>The STIX identifier (<c>uuid</c>).</summary>
	[JsonPropertyName("uuid")]
	public string? Uuid { get; init; }
}
