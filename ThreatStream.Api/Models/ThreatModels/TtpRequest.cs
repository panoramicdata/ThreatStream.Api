using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The optional fields of a TTP create or update request.</summary>
public abstract class TtpRequest : ThreatModelRequest
{
	/// <summary>The free-form description (<c>description</c>).</summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }

	/// <summary>The review stage (<c>publication_status</c>).</summary>
	[JsonPropertyName("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; init; }

	/// <summary>The IDs of the actors to associate (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }

	/// <summary>The IDs of the signatures to associate (<c>signatures</c>).</summary>
	[JsonPropertyName("signatures")]
	public IReadOnlyList<long>? Signatures { get; init; }
}
