using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>PDF reports on resources (<c>api/v1/alfred/report/</c> and <c>api/v1/alfred/async/</c>).</summary>
	public IReports Reports => field ??= For<IReports>();
}
