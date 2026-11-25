using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

/// <summary>
/// The optional fields shared by <see cref="RuleCreateRequest"/> and <see cref="RuleUpdateRequest"/>; each is omitted when
/// <see langword="null"/>.
/// </summary>
public abstract partial class RuleRequest
{
	/// <summary>Whether the rule matches keywords in newly created observables (<c>match_observables</c>; default false).</summary>
	[JsonPropertyName("match_observables")]
	public bool? MatchObservables { get; init; }

	/// <summary>Whether the rule matches keywords in newly created sandbox reports (<c>match_reportedfiles</c>; default false).</summary>
	[JsonPropertyName("match_reportedfiles")]
	public bool? MatchReportedFiles { get; init; }

	/// <summary>Whether the rule matches keywords in newly created signatures (<c>match_signatures</c>; default false).</summary>
	[JsonPropertyName("match_signatures")]
	public bool? MatchSignatures { get; init; }

	/// <summary>Whether the rule matches keywords in newly created threat bulletins (<c>match_tips</c>; default false).</summary>
	[JsonPropertyName("match_tips")]
	public bool? MatchTips { get; init; }

	/// <summary>Whether the rule matches keywords in newly created vulnerabilities (<c>match_vulnerabilities</c>; default false).</summary>
	[JsonPropertyName("match_vulnerabilities")]
	public bool? MatchVulnerabilities { get; init; }

	/// <summary>
	/// The indicator types (such as <c>actor_ipv6</c>) to look for matches in, at the exclusion of all others
	/// (<c>match_impacts</c>). Do not set both this and <see cref="ExcludeImpacts"/>.
	/// </summary>
	[JsonPropertyName("match_impacts")]
	public IReadOnlyList<string>? MatchImpacts { get; init; }

	/// <summary>
	/// The indicator types to exclude from matches (<c>exclude_impacts</c>). Do not set both this and
	/// <see cref="MatchImpacts"/>.
	/// </summary>
	[JsonPropertyName("exclude_impacts")]
	public IReadOnlyList<string>? ExcludeImpacts { get; init; }

	/// <summary>Whether to create an investigation the first time the rule is triggered (<c>create_investigation</c>; default false).</summary>
	[JsonPropertyName("create_investigation")]
	public bool? CreateInvestigation { get; init; }

	/// <summary>Whether to exclude observables on your organization's whitelist from matches (<c>exclude_notify_org_whitelisted</c>; default false).</summary>
	[JsonPropertyName("exclude_notify_org_whitelisted")]
	public bool? ExcludeNotifyOrgWhitelisted { get; init; }

	/// <summary>
	/// Whether to leave matches on observables your organization imported out of Keyword Match and Hourly Digest email
	/// notifications (<c>exclude_notify_owner_org</c>).
	/// </summary>
	[JsonPropertyName("exclude_notify_owner_org")]
	public bool? ExcludeNotifyOwnerOrg { get; init; }

	/// <summary>The tags to apply to matched entities (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }
}
