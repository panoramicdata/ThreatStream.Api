using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

public sealed partial class Rule
{
	/// <summary>Whether the rule matches keywords in newly created observables (<c>match_observables</c>).</summary>
	[JsonPropertyName("match_observables")]
	public bool? MatchObservables { get; init; }

	/// <summary>Whether the rule matches keywords in newly created sandbox reports (<c>match_reportedfiles</c>).</summary>
	[JsonPropertyName("match_reportedfiles")]
	public bool? MatchReportedFiles { get; init; }

	/// <summary>Whether the rule matches keywords in newly created signatures (<c>match_signatures</c>).</summary>
	[JsonPropertyName("match_signatures")]
	public bool? MatchSignatures { get; init; }

	/// <summary>Whether the rule matches keywords in newly created threat bulletins (<c>match_tips</c>).</summary>
	[JsonPropertyName("match_tips")]
	public bool? MatchTips { get; init; }

	/// <summary>Whether the rule matches keywords in newly created vulnerabilities (<c>match_vulnerabilities</c>).</summary>
	[JsonPropertyName("match_vulnerabilities")]
	public bool? MatchVulnerabilities { get; init; }

	/// <summary>Whether observables on your organization's whitelist are excluded from matches (<c>exclude_notify_org_whitelisted</c>).</summary>
	[JsonPropertyName("exclude_notify_org_whitelisted")]
	public bool? ExcludeNotifyOrgWhitelisted { get; init; }

	/// <summary>
	/// Whether matches on observables your organization imported are left out of Keyword Match and Hourly Digest email
	/// notifications (<c>exclude_notify_owner_org</c>).
	/// </summary>
	[JsonPropertyName("exclude_notify_owner_org")]
	public bool? ExcludeNotifyOwnerOrg { get; init; }
}
