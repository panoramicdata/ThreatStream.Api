using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>
/// An asynchronous ThreatStream job (<c>api/v1/generic_async_result/{id}/</c>), such as a user activity report or a
/// query-based bulk tagging request. Poll it with <see cref="Interfaces.IAsyncResults.GetAsync"/> until
/// <see cref="Status"/> is <c>done</c>, then fetch its result with <see cref="Interfaces.IAsyncResults.GetResultAsync"/>,
/// or let <see cref="ThreatStreamClient.RunAsyncJobAsync"/> do both.
/// </summary>
public sealed class GenericAsyncResult : ThreatStreamResource
{
	/// <summary>The job's state (<c>status</c>): <c>inprogress</c> while it runs, <c>done</c> when its result is ready.</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>The HTTP status of the finished job (<c>response_status</c>): 200 on success, <see langword="null"/> while it runs.</summary>
	[JsonPropertyName("response_status")]
	public int? ResponseStatus { get; init; }

	/// <summary>
	/// The API path that started the job (<c>path</c>), such as <c>/api/v1/useraction/</c> or
	/// <c>/api/v2/intelligence/bulk_tagging/</c>.
	/// </summary>
	[JsonPropertyName("path")]
	public string? Path { get; init; }

	/// <summary>When the job was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the job last changed (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
