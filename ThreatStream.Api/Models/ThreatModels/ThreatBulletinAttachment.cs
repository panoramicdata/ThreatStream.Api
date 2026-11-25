using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>ThreatStream's answer to attaching a file to a threat bulletin (<c>POST api/v1/tipreport/{id}/attachment/</c>).</summary>
/// <remarks>
/// The reference does not document the response. The attachment's ID and name are modelled; everything else ThreatStream
/// sends is in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class ThreatBulletinAttachment : ThreatStreamObject
{
	/// <summary>The attachment's ID (<c>id</c>), when sent.</summary>
	[JsonPropertyName("id")]
	public long? Id { get; init; }

	/// <summary>The attachment's name (<c>filename</c>), when sent.</summary>
	[JsonPropertyName("filename")]
	public string? FileName { get; init; }
}
