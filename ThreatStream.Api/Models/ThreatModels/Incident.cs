using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>An incident: a security event affecting an organisation (<c>api/v1/incident/</c>).</summary>
public sealed class Incident : ThreatModelEntity
{
	/// <summary>The incident's status (<c>status</c>); see <c>api/v1/incidentstatustype/</c>.</summary>
	[JsonPropertyName("status")]
	public ThreatStreamReference? Status { get; init; }

	/// <summary>Free-form text about the incident's status (<c>status_desc</c>).</summary>
	[JsonPropertyName("status_desc")]
	public string? StatusDescription { get; init; }

	/// <summary>The actors associated with the incident (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<ThreatStreamReference> Actors { get; init; } = [];

	/// <summary>The victim types the incident affected (<c>victims</c>); see <c>api/v1/victimtype/</c>.</summary>
	[JsonPropertyName("victims")]
	public IReadOnlyList<ThreatStreamReference> Victims { get; init; } = [];

	/// <summary>The sandbox reports associated with the incident (<c>sandbox_reports</c>).</summary>
	[JsonPropertyName("sandbox_reports")]
	public IReadOnlyList<ThreatStreamReference> SandboxReports { get; init; } = [];

	/// <summary>When the incident was first known to be active (<c>start_date</c>).</summary>
	[JsonPropertyName("start_date")]
	public DateTimeOffset? StartDate { get; init; }

	/// <summary>When the incident is known to have ended (<c>end_date</c>).</summary>
	[JsonPropertyName("end_date")]
	public DateTimeOffset? EndDate { get; init; }
}
