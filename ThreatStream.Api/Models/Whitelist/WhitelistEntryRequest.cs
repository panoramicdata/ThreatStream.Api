using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>One entry to add to the import whitelist, in a <see cref="WhitelistAddRequest"/>.</summary>
public sealed class WhitelistEntryRequest
{
	/// <summary>The kind of value (<c>value_type</c>).</summary>
	[JsonPropertyName("value_type")]
	public required WhitelistValueType ValueType { get; init; }

	/// <summary>The value to whitelist (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	public required string Value { get; init; }

	/// <summary>A contextual note on the entry (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public string? Notes { get; init; }
}
