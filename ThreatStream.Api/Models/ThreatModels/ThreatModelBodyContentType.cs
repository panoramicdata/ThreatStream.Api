using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The format of a threat model entity's body or description text (<c>body_content_type</c>); it cannot be changed once set.</summary>
public enum ThreatModelBodyContentType
{
	/// <summary>A format this library does not know.</summary>
	Unknown = 0,

	/// <summary>Markdown, ThreatStream's default (<c>markdown</c>).</summary>
	[JsonStringEnumMemberName("markdown")]
	Markdown = 1,

	/// <summary>Rich text (<c>richtext</c>).</summary>
	[JsonStringEnumMemberName("richtext")]
	RichText = 2
}
