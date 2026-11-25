using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The format of a threat model entity's body text (<c>body_content_type</c>). ThreatStream's default is
/// <see cref="Markdown"/>; once set, it cannot be changed.
/// </summary>
public enum ThreatModelBodyContentType
{
	/// <summary>A format this library does not know.</summary>
	Unknown = 0,

	/// <summary>Markdown (<c>markdown</c>).</summary>
	[JsonStringEnumMemberName("markdown")]
	Markdown = 1,

	/// <summary>Rich text, as HTML (<c>richtext</c>).</summary>
	[JsonStringEnumMemberName("richtext")]
	RichText = 2
}
