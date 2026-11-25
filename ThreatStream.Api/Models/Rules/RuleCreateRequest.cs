using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

/// <summary>
/// The body of <c>POST api/v1/rule/</c>. A rule needs a name, keywords and at least one of the <c>Match...</c> flags (such
/// as <see cref="RuleRequest.MatchObservables"/>) set to <see langword="true"/>.
/// </summary>
public sealed class RuleCreateRequest : RuleRequest
{
	/// <summary>The rule's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	/// <summary>
	/// The keywords to match (<c>keywords</c>), each at least three characters. IP addresses must be regular expressions
	/// (such as <c>192\.168\.</c>), subnets CIDR notation (such as <c>10.20.100.0/26</c>), and no keyword may start or end
	/// with <c>.*</c>.
	/// </summary>
	[JsonPropertyName("keywords")]
	public required IReadOnlyList<string> Keywords { get; init; }
}
