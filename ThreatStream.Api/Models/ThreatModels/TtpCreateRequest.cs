using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A new TTP (<c>POST api/v1/ttp/</c>).</summary>
public sealed class TtpCreateRequest : TtpRequest
{
	/// <summary>The name, unique within your organisation, up to 255 characters (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }
}
