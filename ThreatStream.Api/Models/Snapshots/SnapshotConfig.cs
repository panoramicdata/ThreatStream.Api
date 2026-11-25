using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Snapshots;

/// <summary>
/// What a snapshot holds and how it is split (<c>custom_config</c>). Set exactly one of <see cref="ChunkSize"/> and
/// <see cref="ChunkIocCount"/>: ThreatStream refuses a snapshot with neither or both.
/// </summary>
public sealed class SnapshotConfig
{
	/// <summary>The advanced search expression selecting the observables, such as <c>state="active" AND confidence&gt;=70</c> (<c>filter</c>).</summary>
	[JsonPropertyName("filter")]
	public string? Filter { get; init; }

	/// <summary>The most observables to include (<c>max_results</c>).</summary>
	[JsonPropertyName("max_results")]
	public long? MaxResults { get; init; }

	/// <summary>Only observables whose fields have one of these values, such as <c>{"itype": ["mal_ip", "mal_domain"]}</c>; fields combine with AND (<c>whitelist</c>).</summary>
	[JsonPropertyName("whitelist")]
	public IReadOnlyDictionary<string, IReadOnlyList<string>>? Whitelist { get; init; }

	/// <summary>No observables whose fields have one of these values; fields combine with AND (<c>blacklist</c>).</summary>
	[JsonPropertyName("blacklist")]
	public IReadOnlyDictionary<string, IReadOnlyList<string>>? Blacklist { get; init; }

	/// <summary>Only these fields of each observable; include at least one value field such as <c>srcip</c> or <c>domain</c> (<c>field_whitelist</c>).</summary>
	[JsonPropertyName("field_whitelist")]
	public IReadOnlyList<string>? FieldWhitelist { get; init; }

	/// <summary>All but these fields of each observable (<c>field_blacklist</c>).</summary>
	[JsonPropertyName("field_blacklist")]
	public IReadOnlyList<string>? FieldBlacklist { get; init; }

	/// <summary>The size of each file, in KB or MB, such as <c>1MB</c>; at most <c>200MB</c> (<c>chunk_size</c>).</summary>
	[JsonPropertyName("chunk_size")]
	public string? ChunkSize { get; init; }

	/// <summary>The number of observables in each file, at most 250,000 (<c>chunk_ioc_count</c>).</summary>
	[JsonPropertyName("chunk_ioc_count")]
	public int? ChunkIocCount { get; init; }

	/// <summary>The file format (<c>format</c>).</summary>
	[JsonPropertyName("format")]
	public SnapshotFormat? Format { get; init; }

	/// <summary>Whether to leave out observables on your organisation's import whitelist (<c>use_org_whitelist</c>).</summary>
	[JsonPropertyName("use_org_whitelist")]
	public bool? UseOrgWhitelist { get; init; }
}
