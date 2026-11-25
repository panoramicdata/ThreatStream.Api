using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Sandbox detonations (<c>api/v1/submit/</c>).</summary>
	public ISandboxSubmissions SandboxSubmissions => field ??= For<ISandboxSubmissions>();
}
