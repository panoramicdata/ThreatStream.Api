using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of a new actor (<c>POST api/v1/actor/</c>).</summary>
public sealed class ActorCreateRequest : ActorUpdateRequest
{
	/// <summary>The actor's name, unique within the organisation, up to 255 characters (<c>name</c>). Required.</summary>
	[JsonPropertyName("name")]
	public new required string Name { get; init; }
}
