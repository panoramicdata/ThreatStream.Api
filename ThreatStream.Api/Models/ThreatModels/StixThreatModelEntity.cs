using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A threat model entity that also lists its associations with STIX 2.1 objects (attack patterns, courses of action,
/// identities, infrastructure, intrusion sets, tools) besides the other threat model types: <see cref="AttackPattern"/>
/// and <see cref="Malware"/>. Observables, signatures and TTPs are in <see cref="ThreatModelEntity.Intelligence"/>,
/// <see cref="ThreatModelEntity.Signatures"/> and <see cref="ThreatModelEntity.Ttps"/>.
/// </summary>
public abstract class StixThreatModelEntity : ThreatModelEntity
{
	/// <summary>The associated actors (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<ThreatStreamReference> Actors { get; init; } = [];

	/// <summary>The associated attack patterns (<c>attackpatterns</c>).</summary>
	[JsonPropertyName("attackpatterns")]
	public IReadOnlyList<ThreatStreamReference> AttackPatterns { get; init; } = [];

	/// <summary>The associated campaigns (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<ThreatStreamReference> Campaigns { get; init; } = [];

	/// <summary>The associated courses of action (<c>courseofaction</c>).</summary>
	[JsonPropertyName("courseofaction")]
	public IReadOnlyList<ThreatStreamReference> CoursesOfAction { get; init; } = [];

	/// <summary>The associated identities (<c>identities</c>).</summary>
	[JsonPropertyName("identities")]
	public IReadOnlyList<ThreatStreamReference> Identities { get; init; } = [];

	/// <summary>The associated incidents (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<ThreatStreamReference> Incidents { get; init; } = [];

	/// <summary>The associated infrastructure entities (<c>infrastructure</c>).</summary>
	[JsonPropertyName("infrastructure")]
	public IReadOnlyList<ThreatStreamReference> Infrastructure { get; init; } = [];

	/// <summary>The associated intrusion sets (<c>intrusionsets</c>).</summary>
	[JsonPropertyName("intrusionsets")]
	public IReadOnlyList<ThreatStreamReference> IntrusionSets { get; init; } = [];

	/// <summary>The associated threat bulletins (<c>tipreports</c>).</summary>
	[JsonPropertyName("tipreports")]
	public IReadOnlyList<ThreatStreamReference> ThreatBulletins { get; init; } = [];

	/// <summary>The associated tools (<c>tools</c>).</summary>
	[JsonPropertyName("tools")]
	public IReadOnlyList<ThreatStreamReference> Tools { get; init; } = [];

	/// <summary>The associated vulnerabilities (<c>vulnerability</c>, singular on the wire).</summary>
	[JsonPropertyName("vulnerability")]
	public IReadOnlyList<ThreatStreamReference> Vulnerabilities { get; init; } = [];
}
