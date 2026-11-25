using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Changes to a threat bulletin (<c>PATCH api/v1/tipreport/{id}/</c>); only the fields set are sent.</summary>
public sealed class ThreatBulletinUpdateRequest : ThreatBulletinRequest
{
	/// <summary>A new name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
