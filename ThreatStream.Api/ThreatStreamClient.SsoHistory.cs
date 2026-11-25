using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>The single sign-on activity log (<c>api/v1/ssohistory/</c>).</summary>
	public ISsoHistory SsoHistory => field ??= For<ISsoHistory>();
}
