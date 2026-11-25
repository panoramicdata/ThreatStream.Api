using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableTags;

/// <summary>The observables an <see cref="ObservableTagQueryRequest"/> tags (<c>query</c>): those an advanced query selects.</summary>
public sealed class ObservableTagQuery
{
	/// <summary>The advanced query that selects the observables (<c>q</c>), such as <c>(status = active) and (created_ts &gt;= 2022-10-01T23:00:00)</c>.</summary>
	[JsonPropertyName("q")]
	public required string Q { get; init; }

	/// <summary>The maximum number of observables to tag (<c>limit</c>), at most 10,000.</summary>
	[JsonPropertyName("limit")]
	public int? Limit { get; init; }

	/// <summary>The field the observables are ordered by before the limit applies (<c>order_by</c>), such as <c>-created_ts</c>.</summary>
	[JsonPropertyName("order_by")]
	public string? OrderBy { get; init; }
}
