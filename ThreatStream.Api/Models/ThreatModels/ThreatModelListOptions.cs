using Refit;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Filters for a list of threat model entities of one type (<c>GET api/v1/{type}/</c>).</summary>
public class ThreatModelListOptions : ListOptions
{
	/// <summary>An advanced query expression (<c>q</c>).</summary>
	[AliasAs("q")]
	public string? Query { get; set; }

	/// <summary>Only entities with exactly this name (<c>name</c>).</summary>
	[AliasAs("name")]
	public string? Name { get; set; }

	/// <summary>Only entities created on ThreatStream at or after this moment (<c>created_ts__gte</c>).</summary>
	[AliasAs("created_ts__gte")]
	public DateTimeOffset? CreatedTsGte { get; set; }

	/// <summary>Only entities changed on ThreatStream at or after this moment (<c>modified_ts__gte</c>).</summary>
	[AliasAs("modified_ts__gte")]
	public DateTimeOffset? ModifiedTsGte { get; set; }
}
