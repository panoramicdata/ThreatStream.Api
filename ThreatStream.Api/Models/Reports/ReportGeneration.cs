using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Reports;

/// <summary>
/// The answer to a report request (<c>api/v1/alfred/report/</c>): where to view the report, and the asynchronous job
/// that exports it as PDF.
/// </summary>
/// <remarks>
/// ThreatStream echoes the request's credentials in <c>params</c> (kept in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>): never log or persist this object as it is.
/// </remarks>
public sealed class ReportGeneration : ThreatStreamObject
{
	/// <summary>The request's ID (<c>id</c>).</summary>
	[JsonPropertyName("id")]
	public long Id { get; init; }

	/// <summary>Whether the request was accepted (<c>success</c>).</summary>
	[JsonPropertyName("success")]
	public bool? Success { get; init; }

	/// <summary>Whether the report was generated synchronously (<c>sync</c>).</summary>
	[JsonPropertyName("sync")]
	public bool? Sync { get; init; }

	/// <summary>The URL that shows the report in a browser (<c>printableUrl</c>).</summary>
	[JsonPropertyName("printableUrl")]
	public string? PrintableUrl { get; init; }

	/// <summary>The URL of the PDF export job (<c>callbackUrl</c>), such as <c>https://optic.threatstream.com/api/v1/alfred/async/407262/</c>.</summary>
	[JsonPropertyName("callbackUrl")]
	public string? CallbackUrl { get; init; }

	/// <summary>The ID of the PDF export job, for <see cref="Interfaces.IReports.GetAsyncReportAsync"/> (<c>async_report</c>).</summary>
	[JsonPropertyName("async_report")]
	public long? AsyncReport { get; init; }

	/// <summary>When the report was requested (<c>dateRequested</c>).</summary>
	[JsonPropertyName("dateRequested")]
	public DateTimeOffset? DateRequested { get; init; }

	/// <summary>When the report was completed; <see langword="null"/> until then (<c>dateCompleted</c>).</summary>
	[JsonPropertyName("dateCompleted")]
	public DateTimeOffset? DateCompleted { get; init; }

	/// <summary>When the request was created (<c>createdAt</c>).</summary>
	[JsonPropertyName("createdAt")]
	public DateTimeOffset? CreatedAt { get; init; }

	/// <summary>When the request last changed (<c>updatedAt</c>).</summary>
	[JsonPropertyName("updatedAt")]
	public DateTimeOffset? UpdatedAt { get; init; }

	/// <summary>The ID of the application that made the request (<c>ClientAppId</c>).</summary>
	[JsonPropertyName("ClientAppId")]
	public long? ClientAppId { get; init; }
}
