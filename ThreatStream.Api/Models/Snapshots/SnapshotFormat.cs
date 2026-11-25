using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Snapshots;

/// <summary>The file format of a snapshot.</summary>
public enum SnapshotFormat
{
	/// <summary>A format this library does not know.</summary>
	Unknown = 0,

	/// <summary>CSV with domain, email, MD5, IP and URL observables only (<c>csv</c>).</summary>
	[JsonStringEnumMemberName("csv")]
	Csv = 1,

	/// <summary>CSV with every observable type (<c>csv_v2</c>), which Anomali recommends.</summary>
	[JsonStringEnumMemberName("csv_v2")]
	CsvV2 = 2,

	/// <summary>JSON with domain, email, MD5, IP and URL observables only (<c>json</c>).</summary>
	[JsonStringEnumMemberName("json")]
	Json = 3,

	/// <summary>JSON with every observable type (<c>json_v2</c>), which Anomali recommends.</summary>
	[JsonStringEnumMemberName("json_v2")]
	JsonV2 = 4,

	/// <summary>STIX (<c>stix</c>).</summary>
	[JsonStringEnumMemberName("stix")]
	Stix = 5
}
