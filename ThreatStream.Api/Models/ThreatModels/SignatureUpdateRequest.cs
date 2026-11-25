using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Changes to a signature (<c>PATCH api/v1/signature/{id}/</c>); only the fields set are sent.</summary>
public class SignatureUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>The signature's definition and any other notes (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public string? Notes { get; init; }

	/// <summary>The ID of the signature's type (<c>s_type</c>), from <see cref="Interfaces.ISignatures.ListTypesAsync"/>.</summary>
	[JsonPropertyName("s_type")]
	public long? SignatureType { get; init; }
}
