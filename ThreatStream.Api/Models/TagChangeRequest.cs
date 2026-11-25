using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>The tags a bulk tagging request adds and removes (<c>tags</c> and <c>remove_tags</c>).</summary>
public abstract class TagChangeRequest
{
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
