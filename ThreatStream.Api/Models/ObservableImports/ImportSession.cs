using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// An import job (<c>api/v1/importsession/{id}/</c>). Poll it until <see cref="Status"/> is <c>done</c> or <c>errors</c>.
/// </summary>
/// <remarks>
/// The reference documents only these attributes; the rest of the session (its settings, owner and dates) stays in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class ImportSession : ThreatStreamResource
{
	/// <summary><c>processing</c> while the import runs, then <c>done</c> or <c>errors</c> (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>The number of observables accepted for import, once done (<c>numIndicators</c>).</summary>
	[JsonPropertyName("numIndicators")]
	public int? NumIndicators { get; init; }

	/// <summary>The number of observables rejected, once done (<c>numRejected</c>).</summary>
	[JsonPropertyName("numRejected")]
	public int? NumRejected { get; init; }
}
