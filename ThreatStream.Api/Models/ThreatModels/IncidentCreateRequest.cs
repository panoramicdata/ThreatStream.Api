using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of a new incident (<c>POST api/v1/incident/</c>).</summary>
public sealed class IncidentCreateRequest : IncidentUpdateRequest
{
	/// <summary>The incident's name, unique within the organisation, up to 255 characters (<c>name</c>). Required.</summary>
	[JsonPropertyName("name")]
	public new required string Name { get; init; }
}
