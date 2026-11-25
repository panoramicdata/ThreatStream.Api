using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

public sealed partial class AttackPattern
{
	/// <summary>The intelligence initiatives associated with the attack pattern (<c>intelligence_initiatives</c>); set by the source.</summary>
	[JsonPropertyName("intelligence_initiatives")]
	public IReadOnlyList<ThreatStreamReference> IntelligenceInitiatives { get; init; } = [];

	/// <summary>The malware associated with the attack pattern (<c>malwares</c>).</summary>
	[JsonPropertyName("malwares")]
	public IReadOnlyList<ThreatStreamReference> Malware { get; init; } = [];
}
