using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of an actor update (<c>PATCH api/v1/actor/{id}/</c>): only the properties that are set are changed.</summary>
public class ActorUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>Other names the actor is known by (<c>aliases</c>), as the reference documents it: one string.</summary>
	[JsonPropertyName("aliases")]
	public string? Aliases { get; init; }

	/// <summary>Whether the actor is a team rather than an individual (<c>is_team</c>); ThreatStream's default is <see langword="false"/>.</summary>
	[JsonPropertyName("is_team")]
	public bool? IsTeam { get; init; }

	/// <summary>The ID of the actor's sophistication type (<c>soph_type</c>), from <c>api/v1/actorsophisticationtype/</c>.</summary>
	[JsonPropertyName("soph_type")]
	public long? SophisticationType { get; init; }

	/// <summary>Free-form text about the actor's sophistication (<c>soph_desc</c>).</summary>
	[JsonPropertyName("soph_desc")]
	public string? SophisticationDescription { get; init; }

	/// <summary>When the actor was first known to be active (<c>start_date</c>).</summary>
	[JsonPropertyName("start_date")]
	public DateTimeOffset? StartDate { get; init; }

	/// <summary>The IDs of the victim types the actor targets (<c>victims</c>), from <c>api/v1/victimtype/</c>.</summary>
	[JsonPropertyName("victims")]
	public IReadOnlyList<long>? Victims { get; init; }

	/// <summary>The IDs of the campaigns to associate (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<long>? Campaigns { get; init; }

	/// <summary>The IDs of the incidents to associate (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<long>? Incidents { get; init; }
}
