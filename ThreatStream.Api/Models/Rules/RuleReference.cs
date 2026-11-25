using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

/// <summary>An object a <see cref="Rule"/> refers to (its <c>investigation</c> or <c>organization</c>): ID, name and resource URI.</summary>
public sealed class RuleReference : ThreatStreamResource
{
	/// <summary>The object's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
