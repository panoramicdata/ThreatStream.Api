using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>
/// The <c>multipart/form-data</c> body of <c>POST api/v1/orgwhitelist/upload/</c>: a CSV file whose header row is
/// <c>value_type,value,notes</c>, followed by one entry per row (such as <c>domain,*wildcarddomain.com,</c> or
/// <c>ip,8.8.8.8,notes on ip address</c>).
/// </summary>
public sealed class WhitelistUploadRequest : ThreatStreamMultipartRequest
{
	/// <summary>The CSV file (<c>file</c>), typically of type <c>text/csv</c>.</summary>
	[JsonPropertyName("file")]
	public required ThreatStreamFile File { get; init; }
}
