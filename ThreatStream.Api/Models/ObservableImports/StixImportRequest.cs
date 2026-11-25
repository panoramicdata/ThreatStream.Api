using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// An import of observables from a STIX file, XML or JSON, STIX 2.1 wildcard patterns included
/// (<c>POST api/v1/stix_import/</c>, sent as <c>multipart/form-data</c>).
/// </summary>
public sealed class StixImportRequest : ThreatStreamMultipartRequest
{
	/// <summary>The STIX file (<c>file</c>).</summary>
	[JsonPropertyName("file")]
	public required ThreatStreamFile File { get; init; }

	/// <summary>The visibility: <c>public</c> or <c>private</c> (<c>classification</c>).</summary>
	[JsonPropertyName("classification")]
	public string? Classification { get; init; }

	/// <summary>The confidence you assign, 0 to 100 (<c>confidence</c>).</summary>
	[JsonPropertyName("confidence")]
	public int? Confidence { get; init; }

	/// <summary>Whether to hide your organisation from other organisations (<c>is_anonymous</c>).</summary>
	[JsonPropertyName("is_anonymous")]
	public bool? IsAnonymous { get; init; }

	/// <summary>The tags to apply, sent as JSON (<c>tags</c>).</summary>
	[JsonPropertyName("tags")]
	public IReadOnlyList<ThreatStreamTag>? Tags { get; init; }

	/// <summary>The trusted circles to import into, sent comma-separated (<c>trustedcircles</c>).</summary>
	[JsonPropertyName("trustedcircles")]
	public IReadOnlyList<long>? TrustedCircles { get; init; }
}
