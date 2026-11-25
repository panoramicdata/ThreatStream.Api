using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>The body of <c>PATCH api/v1/orgwhitelist/{id}/</c>: only an entry's note can be changed.</summary>
public sealed class WhitelistUpdateRequest
{
	/// <summary>The entry's new note (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public required string Notes { get; init; }
}
