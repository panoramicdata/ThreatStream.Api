using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Tags on observables in bulk (<c>api/v2/intelligence/bulk_tagging/</c>).</summary>
	public IObservableTags ObservableTags => field ??= For<IObservableTags>();
}
