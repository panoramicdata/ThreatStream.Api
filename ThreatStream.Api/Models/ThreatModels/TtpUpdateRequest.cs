using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Changes to a TTP (<c>PATCH api/v1/ttp/{id}/</c>); only the fields set are sent.</summary>
public sealed class TtpUpdateRequest : TtpRequest
{
	/// <summary>A new name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
