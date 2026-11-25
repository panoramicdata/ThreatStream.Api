using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

/// <summary>
/// A keyword-match rule (<c>api/v1/rule/</c>): keywords ThreatStream looks for in newly created observables, sandbox
/// reports, signatures, threat bulletins or vulnerabilities, with the tags and associations it applies to matches.
/// </summary>
/// <remarks>
/// The fields the reference marks "Unused" (<c>match_actors</c>, <c>match_campaigns</c>, <c>match_incidents</c>,
/// <c>match_malware</c>, <c>match_ttps</c>, <c>org_shared</c>) and <c>user</c>, whose shape the reference does not give, stay
/// in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed partial class Rule : ThreatStreamResource
{
	/// <summary>The rule's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The keywords the rule matches (<c>keywords</c>).</summary>
	[JsonPropertyName("keywords")]
	public IReadOnlyList<string> Keywords { get; init; } = [];

	/// <summary>The total number of keyword matches for the rule (<c>matches</c>).</summary>
	[JsonPropertyName("matches")]
	public long? Matches { get; init; }

	/// <summary>The tags applied to matched entities (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag> Tags { get; init; } = [];

	/// <summary>Whether the rule creates associations between the entities it names and matched entities (<c>has_associations</c>).</summary>
	[JsonPropertyName("has_associations")]
	public bool? HasAssociations { get; init; }

	/// <summary>Whether an investigation is created the first time the rule is triggered (<c>create_investigation</c>).</summary>
	[JsonPropertyName("create_investigation")]
	public bool? CreateInvestigation { get; init; }

	/// <summary>The investigation the rule created, if any (<c>investigation</c>).</summary>
	[JsonPropertyName("investigation")]
	public RuleReference? Investigation { get; init; }

	/// <summary>When the rule was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the rule was last modified (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }

	/// <summary>The ID of the organization that created the rule (<c>org_id</c>).</summary>
	[JsonPropertyName("org_id")]
	public long? OrgId { get; init; }

	/// <summary>The organization that created the rule (<c>organization</c>).</summary>
	[JsonPropertyName("organization")]
	public RuleReference? Organization { get; init; }

	/// <summary>The ID of the user who created the rule (<c>user_id</c>).</summary>
	[JsonPropertyName("user_id")]
	public long? UserId { get; init; }

	/// <summary>Whether the rule is stored on ThreatStream Cloud rather than locally; ThreatStream OnPrem only (<c>remote_api</c>).</summary>
	[JsonPropertyName("remote_api")]
	public bool? RemoteApi { get; init; }
}
