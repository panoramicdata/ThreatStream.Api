namespace ThreatStream.Api.Models;

/// <summary>A file sent as one part of a <see cref="ThreatStreamMultipartRequest"/>.</summary>
/// <param name="FileName">The file name ThreatStream sees, such as <c>indicators.csv</c>.</param>
/// <param name="Content">The file's bytes. They are kept in memory so the request can be retried.</param>
/// <param name="ContentType">The media type, such as <c>text/csv</c>; <c>application/octet-stream</c> when unknown.</param>
public sealed record ThreatStreamFile(string FileName, ReadOnlyMemory<byte> Content, string ContentType)
{
	/// <summary>Creates a file part of type <c>application/octet-stream</c>.</summary>
	/// <param name="fileName">The file name ThreatStream sees.</param>
	/// <param name="content">The file's bytes.</param>
	public ThreatStreamFile(string fileName, ReadOnlyMemory<byte> content) : this(fileName, content, "application/octet-stream")
	{
	}
}
