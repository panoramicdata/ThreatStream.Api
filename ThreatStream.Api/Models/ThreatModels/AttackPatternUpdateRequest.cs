using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The body of an attack pattern update (<c>PATCH api/v1/attackpattern/{id}/</c>): only the properties that are set are
/// changed.
/// </summary>
public class AttackPatternUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>Other names the attack pattern is known by (<c>aliases</c>), as the reference documents it: one string.</summary>
	[JsonPropertyName("aliases")]
	public string? Aliases { get; init; }

	/// <summary>The ID of the user to assign the attack pattern to (<c>assignee_user</c>).</summary>
	[JsonPropertyName("assignee_user")]
	public long? AssigneeUser { get; init; }

	/// <summary>The body text (<c>body</c>), in <see cref="BodyContentType"/>.</summary>
	[JsonPropertyName("body")]
	public string? Body { get; init; }

	/// <summary>The format of the body text (<c>body_content_type</c>): Markdown by default, and fixed once set.</summary>
	[JsonPropertyName("body_content_type")]
	public ThreatModelBodyContentType? BodyContentType { get; init; }

	/// <summary>
	/// The Lockheed Martin kill chain stages (<c>killchainstages</c>): <c>action-on-objectives</c>,
	/// <c>command-and-control</c>, <c>delivery</c>, <c>exploitation</c>, <c>installation</c>, <c>reconnaissance</c> or
	/// <c>weaponization</c>. Settable by PATCH only, and only on attack patterns that are not from MITRE.
	/// </summary>
	[JsonPropertyName("killchainstages")]
	public IReadOnlyList<string>? KillChainStages { get; init; }

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
