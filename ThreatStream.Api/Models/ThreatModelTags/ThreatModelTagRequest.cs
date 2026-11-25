using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModelTags;

/// <summary>The body of <c>POST api/v1/{t}/bulk_tagging/</c>: tags to add to and remove from the given threat model entities.</summary>
public sealed class ThreatModelTagRequest
{
	/// <summary>The IDs of the entities (<c>ids</c>).</summary>
	[JsonPropertyName("ids")]
	public required IReadOnlyList<long> Ids { get; init; }

	/// <summary>
	/// The tags to add (<c>tags</c>). A tag's <see cref="ThreatStreamTag.Tlp"/> sets who sees it: <see cref="Tlp.Red"/> for
	/// your organization only; left <see langword="null"/>, ThreatStream makes the tag public (<c>white</c>).
	/// </summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }

	/// <summary>The tags to remove, by name (<c>remove_tags</c>); by default up to 25 per entity.</summary>
	[JsonPropertyName("remove_tags")]
	public IReadOnlyList<ThreatStreamTag>? RemoveTags { get; init; }
}
