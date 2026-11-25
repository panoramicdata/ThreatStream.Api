using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Changes to a signature (<c>PATCH api/v1/signature/{id}/</c>); only the fields set are sent.</summary>
public sealed class SignatureUpdateRequest : SignatureRequest
{
	/// <summary>A new name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The ID of a new signature type (<c>s_type</c>).</summary>
	[JsonPropertyName("s_type")]
	public long? SignatureType { get; init; }
}
