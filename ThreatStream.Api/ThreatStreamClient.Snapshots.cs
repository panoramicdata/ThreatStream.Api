using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Snapshots of threat intelligence (<c>api/v1/snapshot/</c>).</summary>
	public ISnapshots Snapshots => field ??= For<ISnapshots>();
}
