using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// An observable associated with a threat model entity, as returned by <c>api/v1/{type}/{id}/intelligence/</c>. The
/// commonly used attributes are modelled; the rest stay in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </summary>
/// <remarks>
/// A deliberately small model: the full observable model belongs to the intelligence category, which may replace this one.
/// </remarks>
public sealed class ThreatModelObservable : ThreatStreamResource
{
	/// <summary>The observable's value, such as an IP address, domain, URL, email address or hash (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	public string? Value { get; init; }

	/// <summary>The observable's type, such as <c>ip</c>, <c>domain</c>, <c>url</c>, <c>email</c> or <c>md5</c> (<c>type</c>).</summary>
	[JsonPropertyName("type")]
	public string? Type { get; init; }

	/// <summary>The indicator type, such as <c>apt_ip</c> or <c>mal_domain</c> (<c>itype</c>).</summary>
	[JsonPropertyName("itype")]
	public string? IType { get; init; }

	/// <summary>The threat type, such as <c>apt</c> or <c>malware</c> (<c>threat_type</c>).</summary>
	[JsonPropertyName("threat_type")]
	public string? ThreatType { get; init; }

	/// <summary>The status: <c>active</c>, <c>inactive</c> or <c>falsepos</c> (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>ThreatStream's confidence that the observable is malicious, 0 to 100 (<c>confidence</c>).</summary>
	[JsonPropertyName("confidence")]
	public int? Confidence { get; init; }

	/// <summary>The observable's Traffic Light Protocol designation (<c>tlp</c>).</summary>
	[JsonPropertyName("tlp")]
	public Tlp? Tlp { get; init; }

	/// <summary>The observable's tags (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag> Tags { get; init; } = [];

	/// <summary>When the observable was created on ThreatStream (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the observable was last changed on ThreatStream (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }
}
