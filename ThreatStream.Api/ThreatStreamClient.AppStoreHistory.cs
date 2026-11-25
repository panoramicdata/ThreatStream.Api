using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>The APP Store activity log (<c>api/v1/appstorehistory/</c>).</summary>
	public IAppStoreHistory AppStoreHistory => field ??= For<IAppStoreHistory>();
}
