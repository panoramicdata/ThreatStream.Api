using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Reports;

/// <summary>
/// The PDF export job of a report (<c>api/v1/alfred/async/{id}/</c>). When <see cref="Succeed"/> is
/// <see langword="true"/>, download the PDF from <see cref="SignedS3Url"/> (valid for an hour) without ThreatStream
/// credentials.
/// </summary>
public sealed class AsyncReport : ThreatStreamResource
{
	/// <summary>
	/// <see langword="true"/> when the PDF is ready, <see langword="false"/> when generating it failed, and
	/// <see langword="null"/> while it is in progress (<c>succeed</c>).
	/// </summary>
	[JsonPropertyName("succeed")]
	public bool? Succeed { get; init; }

	/// <summary>The resource type reported on, such as <c>sandbox</c> or <c>actor</c> (<c>report_type</c>).</summary>
	[JsonPropertyName("report_type")]
	public string? ReportType { get; init; }

	/// <summary>The ID of the resource reported on (<c>r_id</c>).</summary>
	[JsonPropertyName("r_id")]
	public long? ResourceId { get; init; }

	/// <summary>The URL that shows the report in a browser (<c>printable</c>).</summary>
	[JsonPropertyName("printable")]
	public string? Printable { get; init; }

	/// <summary>The PDF's storage URL, once it is ready (<c>s3_url</c>).</summary>
	[JsonPropertyName("s3_url")]
	public string? S3Url { get; init; }

	/// <summary>A pre-signed download URL for the PDF, once it is ready (<c>signed_s3_url</c>).</summary>
	[JsonPropertyName("signed_s3_url")]
	public string? SignedS3Url { get; init; }

	/// <summary>When the job was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the job last changed (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
