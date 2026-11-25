using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A campaign: a set of related malicious activity over a period of time (<c>api/v1/campaign/</c>).</summary>
public sealed class Campaign : ThreatModelEntity
{
	/// <summary>The campaign's status (<c>status</c>); see <c>api/v1/campaignstatus/</c>.</summary>
	[JsonPropertyName("status")]
	public ThreatModelReference? Status { get; init; }

	/// <summary>When the campaign was first known to be active (<c>start_date</c>).</summary>
	[JsonPropertyName("start_date")]
	public DateTimeOffset? StartDate { get; init; }

	/// <summary>When the campaign is known to have ended (<c>end_date</c>).</summary>
	[JsonPropertyName("end_date")]
	public DateTimeOffset? EndDate { get; init; }

	/// <summary>The victim types the campaign targets (<c>victims</c>); see <c>api/v1/victimtype/</c>.</summary>
	[JsonPropertyName("victims")]
	public IReadOnlyList<ThreatModelReference> Victims { get; init; } = [];

	/// <summary>The actors associated with the campaign (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<ThreatModelReference> Actors { get; init; } = [];

	/// <summary>The incidents associated with the campaign (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<ThreatModelReference> Incidents { get; init; } = [];
}
