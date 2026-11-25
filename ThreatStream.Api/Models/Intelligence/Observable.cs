using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Intelligence;

/// <summary>An observable (indicator) held in ThreatStream: an IP address, domain, URL, email address or hash.</summary>
public sealed class Observable : ThreatStreamResource
{
	/// <summary>The observable's value (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	public string? Value { get; init; }

	/// <summary>The observable's indicator type, such as <c>mal_ip</c> (<c>itype</c>).</summary>
	[JsonPropertyName("itype")]
	public string? IType { get; init; }
}
