using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableTags;

/// <summary>The body of <c>POST api/v2/intelligence/bulk_tagging/</c>: tags to add to and remove from the given observables.</summary>
public sealed class ObservableTagRequest
{
	/// <summary>The IDs of the observables (<c>ids</c>); up to 250 tags can be managed this way.</summary>
	[JsonPropertyName("ids")]
	public required IReadOnlyList<long> Ids { get; init; }

	/// <summary>
	/// The tags to add (<c>tags</c>). A tag's <see cref="ThreatStreamTag.Tlp"/> sets who sees it: <see cref="Tlp.Red"/> for
	/// your organization only; left <see langword="null"/>, ThreatStream makes the tag public (<c>white</c>).
	/// </summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }

	/// <summary>The tags to remove, by name (<c>remove_tags</c>).</summary>
	[JsonPropertyName("remove_tags")]
	public IReadOnlyList<ThreatStreamTag>? RemoveTags { get; init; }
}
