using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A threat model entity type, by the name ThreatStream uses in its API paths (<c>api/v1/{type}/</c>) and in
/// <c>model_type</c>.
/// </summary>
public enum ThreatModelType
{
	/// <summary>A type this library does not know. Never send it.</summary>
	Unknown = 0,

	/// <summary>Actors (<c>actor</c>).</summary>
	[JsonStringEnumMemberName("actor")]
	Actor = 1,

	/// <summary>Attack patterns (<c>attackpattern</c>).</summary>
	[JsonStringEnumMemberName("attackpattern")]
	AttackPattern = 2,

	/// <summary>Campaigns (<c>campaign</c>).</summary>
	[JsonStringEnumMemberName("campaign")]
	Campaign = 3,

	/// <summary>Incidents (<c>incident</c>).</summary>
	[JsonStringEnumMemberName("incident")]
	Incident = 4,

	/// <summary>Malware (<c>malware</c>).</summary>
	[JsonStringEnumMemberName("malware")]
	Malware = 5,

	/// <summary>Signatures (<c>signature</c>).</summary>
	[JsonStringEnumMemberName("signature")]
	Signature = 6,

	/// <summary>Threat bulletins (<c>tipreport</c>).</summary>
	[JsonStringEnumMemberName("tipreport")]
	ThreatBulletin = 7,

	/// <summary>Tactics, techniques and procedures (<c>ttp</c>).</summary>
	[JsonStringEnumMemberName("ttp")]
	Ttp = 8,

	/// <summary>Vulnerabilities (<c>vulnerability</c>).</summary>
	[JsonStringEnumMemberName("vulnerability")]
	Vulnerability = 9
}
