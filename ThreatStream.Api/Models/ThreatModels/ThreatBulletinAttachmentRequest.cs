using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A file to attach to a threat bulletin (<c>POST api/v1/tipreport/{id}/attachment/</c>, multipart).</summary>
public sealed class ThreatBulletinAttachmentRequest : ThreatStreamMultipartRequest
{
	/// <summary>The file (<c>attachment</c>).</summary>
	[JsonPropertyName("attachment")]
	public required ThreatStreamFile Attachment { get; init; }

	/// <summary>The name to give the attachment (<c>filename</c>), usually the file's own name.</summary>
	[JsonPropertyName("filename")]
	public required string FileName { get; init; }
}
