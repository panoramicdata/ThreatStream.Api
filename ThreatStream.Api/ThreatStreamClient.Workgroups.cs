using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>The workgroups of your organization (<c>api/v1/workgroup/</c>).</summary>
	public IWorkgroups Workgroups => field ??= For<IWorkgroups>();

	/// <summary>Workgroup restrictions on observables in bulk (<c>api/v2/intelligence/bulk_workgroups/</c>).</summary>
	public IObservableWorkgroups ObservableWorkgroups => field ??= For<IObservableWorkgroups>();
}
