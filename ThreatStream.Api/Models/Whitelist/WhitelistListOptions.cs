using Refit;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>
/// The parameters of <c>GET api/v1/orgwhitelist/</c>. ThreatStream returns 10 entries by default here, not 20;
/// <see cref="ListOptions.Limit"/> 0 asks for up to 1000.
/// </summary>
public sealed class WhitelistListOptions : ListOptions
{
	/// <summary>Whether entries include their contextual notes (<c>showNote</c>; ThreatStream's default is true).</summary>
	[AliasAs("showNote")]
	public bool? ShowNote { get; set; }
}
