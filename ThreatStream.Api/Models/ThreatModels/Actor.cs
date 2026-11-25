using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A threat actor: a person or group behind malicious activity (<c>api/v1/actor/</c>).</summary>
public sealed class Actor : ThreatModelEntity
{
	/// <summary>Other names the actor is known by (<c>aliases</c>).</summary>
	[JsonPropertyName("aliases")]
	public IReadOnlyList<ThreatModelReference> Aliases { get; init; } = [];

	/// <summary>Whether the actor is a team rather than an individual (<c>is_team</c>).</summary>
	[JsonPropertyName("is_team")]
	public bool? IsTeam { get; init; }

	/// <summary>
	/// The actor's sophistication, from the STIX threat actor sophistication vocabulary (<c>soph_type</c>); see
	/// <c>api/v1/actorsophisticationtype/</c>.
	/// </summary>
	[JsonPropertyName("soph_type")]
	public ThreatModelReference? SophisticationType { get; init; }

	/// <summary>Free-form text about the actor's sophistication (<c>soph_desc</c>).</summary>
	[JsonPropertyName("soph_desc")]
	public string? SophisticationDescription { get; init; }

	/// <summary>When the actor was first known to be active (<c>start_date</c>).</summary>
	[JsonPropertyName("start_date")]
	public DateTimeOffset? StartDate { get; init; }

	/// <summary>The victim types the actor targets (<c>victims</c>); see <c>api/v1/victimtype/</c>.</summary>
	[JsonPropertyName("victims")]
	public IReadOnlyList<ThreatModelReference> Victims { get; init; } = [];

	/// <summary>The campaigns associated with the actor (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<ThreatModelReference> Campaigns { get; init; } = [];

	/// <summary>The incidents associated with the actor (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<ThreatModelReference> Incidents { get; init; } = [];
}
