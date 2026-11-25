using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A new signature (<c>POST api/v1/signature/</c>).</summary>
public sealed class SignatureCreateRequest : SignatureRequest
{
	/// <summary>The name, unique within your organisation, up to 255 characters (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	/// <summary>The ID of the signature's type (<c>s_type</c>), from <see cref="Interfaces.ISignatures.ListTypesAsync"/>.</summary>
	[JsonPropertyName("s_type")]
	public required long SignatureType { get; init; }
}
