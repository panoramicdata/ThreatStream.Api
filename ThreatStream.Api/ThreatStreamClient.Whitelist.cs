using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Your organization's import whitelist (<c>api/v1/orgwhitelist/</c>).</summary>
	public IWhitelist Whitelist => field ??= For<IWhitelist>();
}
