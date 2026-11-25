using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The associations an attack pattern or malware update can set (see <see cref="StixThreatModelEntity"/>): the IDs of
/// the entities to associate. Observables, signatures and TTPs are in <see cref="ThreatModelUpdateRequest.Intelligence"/>,
/// <see cref="ThreatModelUpdateRequest.Signatures"/> and <see cref="ThreatModelUpdateRequest.Ttps"/>.
/// </summary>
public abstract class StixThreatModelUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>The IDs of the actors to associate (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }

	/// <summary>The IDs of the campaigns to associate (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<long>? Campaigns { get; init; }

	/// <summary>The IDs of the courses of action to associate (<c>courseofaction</c>).</summary>
	[JsonPropertyName("courseofaction")]
	public IReadOnlyList<long>? CoursesOfAction { get; init; }

	/// <summary>The IDs of the identities to associate (<c>identities</c>).</summary>
	[JsonPropertyName("identities")]
	public IReadOnlyList<long>? Identities { get; init; }

	/// <summary>The IDs of the incidents to associate (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<long>? Incidents { get; init; }

	/// <summary>The IDs of the infrastructure entities to associate (<c>infrastructure</c>).</summary>
	[JsonPropertyName("infrastructure")]
	public IReadOnlyList<long>? Infrastructure { get; init; }

	/// <summary>The IDs of the intrusion sets to associate (<c>intrusionsets</c>).</summary>
	[JsonPropertyName("intrusionsets")]
	public IReadOnlyList<long>? IntrusionSets { get; init; }

	/// <summary>The IDs of the threat bulletins to associate (<c>tipreports</c>).</summary>
	[JsonPropertyName("tipreports")]
	public IReadOnlyList<long>? ThreatBulletins { get; init; }

	/// <summary>The IDs of the vulnerabilities to associate (<c>vulnerability</c>, singular on the wire).</summary>
	[JsonPropertyName("vulnerability")]
	public IReadOnlyList<long>? Vulnerabilities { get; init; }
}
