using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A signature type, such as Snort, YARA, OpenIOC or Suricata (<c>api/v1/signature_type/</c>); its <see cref="ThreatStreamResource.Id"/>
/// is what <see cref="SignatureCreateRequest.SignatureType"/> takes.
/// </summary>
public sealed class SignatureType : ThreatStreamResource
{
	/// <summary>The type's name, such as <c>Snort</c> or <c>Carbon Black Query</c> (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
