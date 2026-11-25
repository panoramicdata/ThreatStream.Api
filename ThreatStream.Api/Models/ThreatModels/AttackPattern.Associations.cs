using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

public sealed partial class AttackPattern
{
	/// <summary>The actors associated with the attack pattern (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<ThreatModelReference> Actors { get; init; } = [];

	/// <summary>The attack patterns associated with the attack pattern (<c>attackpatterns</c>).</summary>
	[JsonPropertyName("attackpatterns")]
	public IReadOnlyList<ThreatModelReference> AttackPatterns { get; init; } = [];

	/// <summary>The campaigns associated with the attack pattern (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<ThreatModelReference> Campaigns { get; init; } = [];

	/// <summary>The courses of action associated with the attack pattern (<c>courseofaction</c>).</summary>
	[JsonPropertyName("courseofaction")]
	public IReadOnlyList<ThreatModelReference> CoursesOfAction { get; init; } = [];

	/// <summary>The identities associated with the attack pattern (<c>identities</c>).</summary>
	[JsonPropertyName("identities")]
	public IReadOnlyList<ThreatModelReference> Identities { get; init; } = [];

	/// <summary>The incidents associated with the attack pattern (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<ThreatModelReference> Incidents { get; init; } = [];

	/// <summary>The infrastructure entities associated with the attack pattern (<c>infrastructure</c>).</summary>
	[JsonPropertyName("infrastructure")]
	public IReadOnlyList<ThreatModelReference> Infrastructure { get; init; } = [];

	/// <summary>The intelligence initiatives associated with the attack pattern (<c>intelligence_initiatives</c>); set by the source.</summary>
	[JsonPropertyName("intelligence_initiatives")]
	public IReadOnlyList<ThreatModelReference> IntelligenceInitiatives { get; init; } = [];

	/// <summary>The intrusion sets associated with the attack pattern (<c>intrusionsets</c>).</summary>
	[JsonPropertyName("intrusionsets")]
	public IReadOnlyList<ThreatModelReference> IntrusionSets { get; init; } = [];

	/// <summary>The malware associated with the attack pattern (<c>malwares</c>).</summary>
	[JsonPropertyName("malwares")]
	public IReadOnlyList<ThreatModelReference> Malware { get; init; } = [];

	/// <summary>The threat bulletins associated with the attack pattern (<c>tipreports</c>).</summary>
	[JsonPropertyName("tipreports")]
	public IReadOnlyList<ThreatModelReference> ThreatBulletins { get; init; } = [];

	/// <summary>The tools associated with the attack pattern (<c>tools</c>).</summary>
	[JsonPropertyName("tools")]
	public IReadOnlyList<ThreatModelReference> Tools { get; init; } = [];

	/// <summary>The vulnerabilities associated with the attack pattern (<c>vulnerability</c>, singular on the wire).</summary>
	[JsonPropertyName("vulnerability")]
	public IReadOnlyList<ThreatModelReference> Vulnerabilities { get; init; } = [];
}
