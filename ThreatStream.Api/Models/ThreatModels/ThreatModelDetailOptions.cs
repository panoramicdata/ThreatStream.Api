using Refit;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The ThreatStream OnPrem parameters of a threat model entity's details (<c>api/v1/{type}/{id}/</c>). Pass
/// <see langword="null"/> on the cloud.
/// </summary>
public class ThreatModelDetailOptions
{
	/// <summary>
	/// OnPrem only: <see langword="true"/> leaves out the associations (up to 1000 are returned by default), which Anomali
	/// recommends for entities with many associations (<c>skip_associations</c>).
	/// </summary>
	[AliasAs("skip_associations")]
	public bool? SkipAssociations { get; set; }

	/// <summary>OnPrem only: <see langword="true"/> leaves out the associated observables (<c>skip_intelligence</c>).</summary>
	[AliasAs("skip_intelligence")]
	public bool? SkipIntelligence { get; set; }

	/// <summary>OnPrem only: <see langword="true"/> gets a remote entity, stored on the cloud (<c>remote_api</c>).</summary>
	[AliasAs("remote_api")]
	public bool? RemoteApi { get; set; }
}
