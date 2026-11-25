using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Intelligence feeds (<c>api/v1/feed/</c>).</summary>
	public IFeeds Feeds => field ??= For<IFeeds>();
}
