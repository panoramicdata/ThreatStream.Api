using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A new threat bulletin (<c>POST api/v1/tipreport/</c>).</summary>
public sealed class ThreatBulletinCreateRequest : ThreatBulletinUpdateRequest
{
	/// <summary>The bulletin's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public new required string Name { get; init; }
}
