using Refit;

namespace ThreatStream.Api.Models.Feeds;

/// <summary>Filters for <c>GET api/v1/feed/</c>.</summary>
public sealed class FeedListOptions : ListOptions
{
	/// <summary>The feed with this ID (<c>id</c>).</summary>
	[AliasAs("id")]
	public long? Id { get; set; }

	/// <summary>Feeds with this name (<c>name</c>).</summary>
	[AliasAs("name")]
	public string? Name { get; set; }

	/// <summary>Feeds with this default indicator type (<c>itype</c>).</summary>
	[AliasAs("itype")]
	public string? Itype { get; set; }

	/// <summary>Feeds whose observables stay active this many days (<c>expiry_time_in_days</c>).</summary>
	[AliasAs("expiry_time_in_days")]
	public int? ExpiryTimeInDays { get; set; }
}
