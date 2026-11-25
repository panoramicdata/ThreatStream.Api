using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>
/// One page of a ThreatStream list: <c>{"meta": {...}, "objects": [...]}</c>. Use
/// <see cref="ThreatStreamClient.EnumerateAsync{T}"/> to read every page.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public sealed class ThreatStreamPage<T>
{
	/// <summary>Paging information: the limit, offset, total count and links to the next and previous pages.</summary>
	[JsonPropertyName("meta")]
	public ThreatStreamPageMeta? Meta { get; init; }

	/// <summary>The objects on this page; empty (never <see langword="null"/>) when there are none.</summary>
	[JsonPropertyName("objects")]
	public IReadOnlyList<T> Objects { get; init; } = [];
}
