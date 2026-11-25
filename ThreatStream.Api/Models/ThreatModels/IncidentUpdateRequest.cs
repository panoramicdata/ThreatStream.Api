using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of an incident update (<c>PATCH api/v1/incident/{id}/</c>): only the properties that are set are changed.</summary>
public class IncidentUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>The ID of the incident's status (<c>status</c>), from <c>api/v1/incidentstatustype/</c>.</summary>
	[JsonPropertyName("status")]
	public long? Status { get; init; }

	/// <summary>Free-form text about the incident's status (<c>status_desc</c>).</summary>
	[JsonPropertyName("status_desc")]
	public string? StatusDescription { get; init; }

	/// <summary>When the incident was first known to be active (<c>start_date</c>).</summary>
	[JsonPropertyName("start_date")]
	public DateTimeOffset? StartDate { get; init; }

	/// <summary>When the incident is known to have ended (<c>end_date</c>).</summary>
	[JsonPropertyName("end_date")]
	public DateTimeOffset? EndDate { get; init; }

	/// <summary>The IDs of the victim types the incident affected (<c>victims</c>), from <c>api/v1/victimtype/</c>.</summary>
	[JsonPropertyName("victims")]
	public IReadOnlyList<long>? Victims { get; init; }

	/// <summary>The IDs of the actors to associate (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }

	/// <summary>The IDs of the sandbox reports to associate (<c>sandbox_reports</c>).</summary>
	[JsonPropertyName("sandbox_reports")]
	public IReadOnlyList<long>? SandboxReports { get; init; }
}
