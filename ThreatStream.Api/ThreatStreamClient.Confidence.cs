using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Confidence scores of observables (<c>api/v1/inteldetails/confidence_trend/</c>).</summary>
	public IConfidence Confidence => field ??= For<IConfidence>();
}
