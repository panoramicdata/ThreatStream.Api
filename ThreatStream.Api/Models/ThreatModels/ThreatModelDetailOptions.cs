using Refit;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// ThreatStream OnPrem options for reading one threat model entity (<c>GET api/v1/{type}/{id}/</c>). The cloud ignores
/// them; pass <see langword="null"/> there.
/// </summary>
public class ThreatModelDetailOptions
{
	/// <summary>
	/// Leaves out the entity's threat model and observable associations (<c>skip_associations</c>), which OnPrem otherwise
	/// returns (up to 1000), slowly for a heavily associated entity.
	/// </summary>
	[AliasAs("skip_associations")]
	public bool? SkipAssociations { get; set; }

	/// <summary>Leaves out the entity's associated observables (<c>skip_intelligence</c>).</summary>
	[AliasAs("skip_intelligence")]
	public bool? SkipIntelligence { get; set; }

	/// <summary>Reads the remote entity held in the cloud rather than a local one (<c>remote_api</c>).</summary>
	[AliasAs("remote_api")]
	public bool? RemoteApi { get; set; }
}
