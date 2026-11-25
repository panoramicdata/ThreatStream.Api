using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of a campaign update (<c>PATCH api/v1/campaign/{id}/</c>): only the properties that are set are changed.</summary>
public class CampaignUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>The ID of the campaign's status (<c>status</c>), from <c>api/v1/campaignstatus/</c>.</summary>
	[JsonPropertyName("status")]
	public long? Status { get; init; }

	/// <summary>When the campaign was first known to be active (<c>start_date</c>).</summary>
	[JsonPropertyName("start_date")]
	public DateTimeOffset? StartDate { get; init; }

	/// <summary>When the campaign is known to have ended (<c>end_date</c>).</summary>
	[JsonPropertyName("end_date")]
	public DateTimeOffset? EndDate { get; init; }

	/// <summary>The IDs of the victim types the campaign targets (<c>victims</c>), from <c>api/v1/victimtype/</c>.</summary>
	[JsonPropertyName("victims")]
	public IReadOnlyList<long>? Victims { get; init; }

	/// <summary>The IDs of the actors to associate (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }

	/// <summary>The IDs of the incidents to associate (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<long>? Incidents { get; init; }
}
