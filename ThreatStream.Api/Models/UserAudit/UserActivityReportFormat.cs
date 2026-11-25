using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.UserAudit;

/// <summary>The file format of a user activity report (<c>format</c>).</summary>
public enum UserActivityReportFormat
{
	/// <summary>A format this library does not know.</summary>
	Unknown = 0,

	/// <summary>Comma-separated values (<c>csv</c>).</summary>
	[JsonStringEnumMemberName("csv")]
	Csv = 1,

	/// <summary>JSON (<c>json</c>).</summary>
	[JsonStringEnumMemberName("json")]
	Json = 2
}
