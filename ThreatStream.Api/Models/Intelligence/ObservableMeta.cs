using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Intelligence;

/// <summary>
/// The <c>meta</c> object of an <see cref="Observable"/>. The WHOIS <c>registrant_*</c> attributes stay in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </summary>
public sealed class ObservableMeta : ThreatStreamObject
{
	/// <summary>A tag that relates the observable to others (<c>detail</c>).</summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	/// <summary>Details of the observable's state, such as why it is a false positive (<c>detail2</c>).</summary>
	[JsonPropertyName("detail2")]
	public string? Detail2 { get; init; }

	/// <summary>The malware the observable is associated with (<c>maltype</c>).</summary>
	[JsonPropertyName("maltype")]
	public string? Maltype { get; init; }

	/// <summary>The severity: <c>low</c>, <c>medium</c>, <c>high</c> or <c>very-high</c> (<c>severity</c>).</summary>
	[JsonPropertyName("severity")]
	public string? Severity { get; init; }
}
