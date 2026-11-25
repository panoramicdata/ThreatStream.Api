using Refit;

namespace ThreatStream.Api.Models;

/// <summary>
/// The paging and ordering parameters every ThreatStream list accepts. Pass <see langword="null"/> for ThreatStream's
/// defaults (20 objects from the start, in the endpoint's default order). Endpoints with their own filters derive from it.
/// </summary>
public class ListOptions
{
	/// <summary>The page size (<c>limit</c>): 20 by default; 0 asks for as many as ThreatStream allows (up to 1000).</summary>
	[AliasAs("limit")]
	public int? Limit { get; set; }

	/// <summary>The number of objects to skip (<c>offset</c>).</summary>
	[AliasAs("offset")]
	public int? Offset { get; set; }

	/// <summary>The field to order by (<c>order_by</c>); prefix it with <c>-</c> for descending order, such as <c>-created_ts</c>.</summary>
	[AliasAs("order_by")]
	public string? OrderBy { get; set; }
}
