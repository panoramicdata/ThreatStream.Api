using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Intelligence;

/// <summary>
/// A search saved from the ThreatStream user interface (<c>api/v1/search_filter/</c>). Its <see cref="ThreatStreamResource.Id"/>
/// runs the search as <see cref="ObservableListOptions.SearchFilter"/> or
/// <see cref="ThreatModelSearch.ThreatModelSearchOptions.SearchFilter"/>.
/// </summary>
/// <remarks>
/// <c>created_by_user</c> and <c>es_query</c> stay in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class SearchFilter : ThreatStreamResource
{
	/// <summary>The filter's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The advanced search expression it runs, such as <c>(status="active") and (severity="high")</c> (<c>query</c>).</summary>
	[JsonPropertyName("query")]
	public string? Query { get; init; }

	/// <summary>Whether the user may change it (<c>is_editable</c>).</summary>
	[JsonPropertyName("is_editable")]
	public bool? IsEditable { get; init; }

	/// <summary>When it was saved (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When it last changed (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
