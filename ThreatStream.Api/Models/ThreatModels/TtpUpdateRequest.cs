using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Changes to a TTP (<c>PATCH api/v1/ttp/{id}/</c>); only the fields set are sent.</summary>
public class TtpUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>The IDs of the actors to associate (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }
}
