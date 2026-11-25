using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModelTags;

/// <summary>A kind of threat model entity whose tags can be managed in bulk (the <c>{t}</c> of <c>api/v1/{t}/bulk_tagging/</c>).</summary>
public enum ThreatModelTagEntityType
{
	/// <summary>A kind this library does not know.</summary>
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
	TipReport = 7,

	/// <summary>TTPs (<c>ttp</c>).</summary>
	[JsonStringEnumMemberName("ttp")]
	Ttp = 8,

	/// <summary>Vulnerabilities (<c>vulnerability</c>).</summary>
	[JsonStringEnumMemberName("vulnerability")]
	Vulnerability = 9
}
