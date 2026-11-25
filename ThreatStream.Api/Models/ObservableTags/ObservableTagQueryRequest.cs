using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableTags;

/// <summary>
/// The body of <c>POST api/v2/intelligence/bulk_tagging/?async_result=true</c>: tags to add to and remove from up to 10,000
/// observables selected by a query.
/// </summary>
public sealed class ObservableTagQueryRequest
{
	/// <summary>The query that selects the observables (<c>query</c>).</summary>
	[JsonPropertyName("query")]
	public required ObservableTagQuery Query { get; init; }

	/// <summary>The tags to add (<c>tags</c>); see <see cref="ObservableTagRequest.Tags"/>.</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }

	/// <summary>The tags to remove (<c>remove_tags</c>).</summary>
	[JsonPropertyName("remove_tags")]
	public IReadOnlyList<ThreatStreamTag>? RemoveTags { get; init; }
}
