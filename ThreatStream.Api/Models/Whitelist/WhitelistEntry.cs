using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>
/// An entry of your organization's import whitelist (<c>api/v1/orgwhitelist/</c>): a value that imports never turn into
/// an observable.
/// </summary>
public sealed class WhitelistEntry : ThreatStreamResource
{
	/// <summary>The kind of value (<c>value_type</c>).</summary>
	[JsonPropertyName("value_type")]
	public WhitelistValueType ValueType { get; init; }

	/// <summary>The whitelisted value (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	public string? Value { get; init; }

	/// <summary>The contextual note on the entry (<c>notes</c>); not sent when the list asked for <c>showNote=false</c>.</summary>
	[JsonPropertyName("notes")]
	public string? Notes { get; init; }

	/// <summary>When the entry was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the entry was last modified (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
