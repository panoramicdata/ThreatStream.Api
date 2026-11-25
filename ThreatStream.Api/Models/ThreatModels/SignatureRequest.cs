using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The optional fields of a signature create or update request.</summary>
public abstract class SignatureRequest : ThreatModelRequest
{
	/// <summary>The signature's definition and any other notes (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public string? Notes { get; init; }

	/// <summary>The review stage (<c>publication_status</c>).</summary>
	[JsonPropertyName("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; init; }
}
