using Refit;

namespace ThreatStream.Api.Models.Sandbox;

/// <summary>What <c>GET api/v1/submit/search/</c> looks for.</summary>
public sealed class SandboxSearchOptions : ListOptions
{
	/// <summary>A keyword matched against the vendor, platform, status, result and notes (tags) of each detonation (<c>q</c>).</summary>
	[AliasAs("q")]
	public string? Query { get; set; }
}
