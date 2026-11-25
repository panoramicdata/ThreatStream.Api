using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of a new attack pattern (<c>POST api/v1/attackpattern/</c>).</summary>
public sealed class AttackPatternCreateRequest : AttackPatternUpdateRequest
{
	/// <summary>The attack pattern's name, unique within the organisation, up to 255 characters (<c>name</c>). Required.</summary>
	[JsonPropertyName("name")]
	public new required string Name { get; init; }
}
