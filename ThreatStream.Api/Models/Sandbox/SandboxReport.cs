using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Sandbox;

/// <summary>
/// A sandbox detonation, as found by a sandbox search (<c>api/v1/submit/search/</c>) or at its status path
/// (<c>api/v1/submit/{id}/</c>).
/// </summary>
/// <remarks>
/// The reference documents no response attributes for these operations; it says only that a search matches the
/// <c>vendor</c>, <c>platform</c>, <c>status</c>, <c>result</c> and <c>notes</c> fields. Those that are plain text are
/// modelled; everything else stays in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class SandboxReport : ThreatStreamResource
{
	/// <summary>The sandbox vendor (<c>vendor</c>).</summary>
	[JsonPropertyName("vendor")]
	public string? Vendor { get; init; }

	/// <summary>The platform detonated on, such as <c>WINDOWS7</c> (<c>platform</c>).</summary>
	[JsonPropertyName("platform")]
	public string? Platform { get; init; }

	/// <summary>The detonation's status (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }
}
