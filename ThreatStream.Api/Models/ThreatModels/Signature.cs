using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A detection signature, such as a Snort or YARA rule (<c>api/v1/signature/</c>).</summary>
public sealed class Signature : ThreatModelEntity
{
	/// <summary>The signature's definition and any other notes (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public string? Notes { get; init; }

	/// <summary>
	/// The signature's type (<c>s_type</c>), such as <c>Snort</c> or <c>YARA</c>; a numeric ID or an object is kept as its
	/// text. See <see cref="Interfaces.ISignatures.ListTypesAsync"/>.
	/// </summary>
	[JsonPropertyName("s_type")]
	public string? SignatureType { get; init; }

	/// <summary>Associated observables (<c>intelligence</c>), each an ID or an object.</summary>
	[JsonPropertyName("intelligence")]
	public IReadOnlyList<JsonElement> Intelligence { get; init; } = [];

	/// <summary>On ThreatStream OnPrem, whether the entity is remote, held in the cloud (<c>remote_api</c>).</summary>
	[JsonPropertyName("remote_api")]
	public bool? RemoteApi { get; init; }
}
