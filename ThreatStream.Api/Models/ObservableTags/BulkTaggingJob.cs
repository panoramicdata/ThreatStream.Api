using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableTags;

/// <summary>
/// The asynchronous job a query-based bulk tagging request creates (an <c>api/v1/generic_async_result/{id}/</c> resource).
/// Poll that resource until <see cref="Status"/> is <c>done</c> and <see cref="ResponseStatus"/> is 200, then fetch
/// <c>api/v1/generic_async_result/{id}/get_result/</c> to finish the job.
/// </summary>
public sealed class BulkTaggingJob : ThreatStreamResource
{
	/// <summary>The job's state (<c>status</c>): <c>inprogress</c> while it runs, <c>done</c> when it has finished.</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>The HTTP status of the finished work (<c>response_status</c>); <see langword="null"/> while the job runs.</summary>
	[JsonPropertyName("response_status")]
	public int? ResponseStatus { get; init; }

	/// <summary>The API path that created the job (<c>path</c>), <c>/api/v2/intelligence/bulk_tagging/</c>.</summary>
	[JsonPropertyName("path")]
	public string? Path { get; init; }

	/// <summary>When the job was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the job last changed (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
