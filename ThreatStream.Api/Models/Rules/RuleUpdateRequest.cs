using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

/// <summary>The body of <c>PATCH api/v1/rule/{id}/</c>: only the fields that are not <see langword="null"/> are changed.</summary>
public sealed class RuleUpdateRequest : RuleRequest
{
	/// <summary>The rule's new name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The rule's new keywords, replacing the old ones (<c>keywords</c>); see <see cref="RuleCreateRequest.Keywords"/>.</summary>
	[JsonPropertyName("keywords")]
	public IReadOnlyList<string>? Keywords { get; init; }
}
