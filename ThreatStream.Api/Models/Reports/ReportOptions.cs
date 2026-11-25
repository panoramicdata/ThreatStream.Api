using Refit;

namespace ThreatStream.Api.Models.Reports;

/// <summary>What <c>GET api/v1/alfred/report/</c> reports on.</summary>
public sealed class ReportOptions
{
	/// <summary>
	/// The resource type (<c>resource_name</c>): <c>sandbox</c>, <c>rulematch</c>, <c>dashboard</c>, <c>eula</c>,
	/// <c>actor</c>, <c>attackpattern</c>, <c>campaign</c>, <c>incident</c>, <c>malware</c>, <c>signature</c>,
	/// <c>tipreport</c>, <c>ttp</c> or <c>vulnerability</c>.
	/// </summary>
	[AliasAs("resource_name")]
	public required string ResourceName { get; set; }

	/// <summary>The resource's ID (<c>resource_id</c>).</summary>
	[AliasAs("resource_id")]
	public required long ResourceId { get; set; }

	/// <summary>Whether to generate the report synchronously (<c>sync</c>); the reference's examples send <see langword="false"/>.</summary>
	[AliasAs("sync")]
	public bool? Sync { get; set; }
}
