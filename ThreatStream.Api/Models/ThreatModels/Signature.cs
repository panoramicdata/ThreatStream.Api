using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A detection signature, such as a Snort or YARA rule (<c>api/v1/signature/</c>).</summary>
public sealed class Signature : ThreatModelEntity
{
	/// <summary>The signature's definition and any other notes (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public string? Notes { get; init; }

	/// <summary>
	/// The signature's type (<c>s_type</c>), such as <c>Snort</c> or <c>YARA</c>, as a name, an ID or an object. See
	/// <see cref="Interfaces.ISignatures.ListTypesAsync"/>.
	/// </summary>
	[JsonPropertyName("s_type")]
	public ThreatStreamReference? SignatureType { get; init; }
}
