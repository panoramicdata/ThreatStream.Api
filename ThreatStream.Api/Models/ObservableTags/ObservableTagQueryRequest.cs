using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableTags;

/// <summary>
/// The body of <c>POST api/v2/intelligence/bulk_tagging/?async_result=true</c>: tags to add to and remove from up to 10,000
/// observables selected by a query.
/// </summary>
public sealed class ObservableTagQueryRequest : TagChangeRequest
{
	/// <summary>The query that selects the observables (<c>query</c>).</summary>
	[JsonPropertyName("query")]
	[JsonPropertyOrder(-1)]
	public required ObservableTagQuery Query { get; init; }
}
