using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Associations;

/// <summary>
/// A kind of entity that can be associated with another (the <c>entity_type</c> and <c>associated_entity_type</c> path
/// segments of <c>api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_add/</c>).
/// </summary>
public enum AssociationEntityType
{
	/// <summary>A kind this library does not know.</summary>
	Unknown = 0,

	/// <summary>An actor (<c>actor</c>).</summary>
	[JsonStringEnumMemberName("actor")]
	Actor = 1,

	/// <summary>An attack pattern (<c>attackpattern</c>).</summary>
	[JsonStringEnumMemberName("attackpattern")]
	AttackPattern = 2,

	/// <summary>A campaign (<c>campaign</c>).</summary>
	[JsonStringEnumMemberName("campaign")]
	Campaign = 3,

	/// <summary>A course of action (<c>courseofaction</c>).</summary>
	[JsonStringEnumMemberName("courseofaction")]
	CourseOfAction = 4,

	/// <summary>An identity (<c>identity</c>).</summary>
	[JsonStringEnumMemberName("identity")]
	Identity = 5,

	/// <summary>An infrastructure (<c>infrastructure</c>).</summary>
	[JsonStringEnumMemberName("infrastructure")]
	Infrastructure = 6,

	/// <summary>An intrusion set (<c>intrusionset</c>).</summary>
	[JsonStringEnumMemberName("intrusionset")]
	IntrusionSet = 7,

	/// <summary>An incident (<c>incident</c>).</summary>
	[JsonStringEnumMemberName("incident")]
	Incident = 8,

	/// <summary>A malware family (<c>malware</c>).</summary>
	[JsonStringEnumMemberName("malware")]
	Malware = 9,

	/// <summary>A signature (<c>signature</c>).</summary>
	[JsonStringEnumMemberName("signature")]
	Signature = 10,

	/// <summary>A threat bulletin (<c>tipreport</c>).</summary>
	[JsonStringEnumMemberName("tipreport")]
	ThreatBulletin = 11,

	/// <summary>A tool (<c>tool</c>).</summary>
	[JsonStringEnumMemberName("tool")]
	Tool = 12,

	/// <summary>A TTP (<c>ttp</c>).</summary>
	[JsonStringEnumMemberName("ttp")]
	Ttp = 13,

	/// <summary>A vulnerability (<c>vulnerability</c>).</summary>
	[JsonStringEnumMemberName("vulnerability")]
	Vulnerability = 14,

	/// <summary>
	/// Observables (<c>intelligence</c>): valid as the associated entity type only, as in the reference's example
	/// <c>api/v1/tipreport/744/intelligence/bulk_add/</c>.
	/// </summary>
	[JsonStringEnumMemberName("intelligence")]
	Intelligence = 15
}
