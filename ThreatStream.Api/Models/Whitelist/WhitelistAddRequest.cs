using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>The body of <c>POST api/v1/orgwhitelist/bulk/</c>: <c>{"whitelist": [...]}</c>.</summary>
public sealed class WhitelistAddRequest
{
	/// <summary>The entries to add (<c>whitelist</c>).</summary>
	[JsonPropertyName("whitelist")]
	public required IReadOnlyList<WhitelistEntryRequest> Entries { get; init; }
}
