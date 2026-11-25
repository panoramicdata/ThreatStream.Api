using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// An import without approval (<c>PATCH api/v2/intelligence/</c>): <c>{"meta": {...}, "objects": [...]}</c>. At most 10 MB.
/// </summary>
public sealed class ObservableBulkImportRequest
{
	/// <summary>Settings for every observable, which each observable's own settings override (<c>meta</c>).</summary>
	[JsonPropertyName("meta")]
	public ObservableImportMeta? Meta { get; init; }

	/// <summary>The observables (<c>objects</c>).</summary>
	[JsonPropertyName("objects")]
	public required IReadOnlyList<ObservableImportObject> Objects { get; init; }
}
