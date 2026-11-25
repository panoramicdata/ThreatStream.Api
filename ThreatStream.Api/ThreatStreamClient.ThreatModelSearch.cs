using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Search across every threat model type (<c>api/v1/threat_model_search/</c>).</summary>
	public IThreatModelSearch ThreatModelSearch => field ??= For<IThreatModelSearch>();
}
