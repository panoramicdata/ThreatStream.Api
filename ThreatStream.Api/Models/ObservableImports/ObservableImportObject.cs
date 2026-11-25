using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// One observable in an import without approval. Set the property for its type (<see cref="SourceIp"/>,
/// <see cref="Domain"/>, <see cref="Url"/>, <see cref="Email"/> or <see cref="Md5"/>, or <see cref="Value"/>) and
/// <see cref="Itype"/>; email and hash observables also need a <see cref="ObservableImportSettings.Confidence"/>, here or
/// in the <c>meta</c> section.
/// </summary>
public sealed class ObservableImportObject : ObservableImportSettings
{
	/// <summary>The indicator type, such as <c>mal_ip</c> or <c>apt_domain</c> (<c>itype</c>).</summary>
	[JsonPropertyName("itype")]
	public required string Itype { get; init; }

	/// <summary>An IP address (<c>srcip</c>).</summary>
	[JsonPropertyName("srcip")]
	public string? SourceIp { get; init; }

	/// <summary>A domain (<c>domain</c>).</summary>
	[JsonPropertyName("domain")]
	public string? Domain { get; init; }

	/// <summary>A URL (<c>url</c>).</summary>
	[JsonPropertyName("url")]
	public string? Url { get; init; }

	/// <summary>An email address (<c>email</c>).</summary>
	[JsonPropertyName("email")]
	public string? Email { get; init; }

	/// <summary>A hash of any type (<c>md5</c>).</summary>
	[JsonPropertyName("md5")]
	public string? Md5 { get; init; }

	/// <summary>The observable of any type, as in the reference's Python example (<c>value</c>).</summary>
	[JsonPropertyName("value")]
	public string? Value { get; init; }
}
