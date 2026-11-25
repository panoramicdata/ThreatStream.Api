using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>The organisation's users, as an Org Admin manages them (<c>api/v1/orgadmin/</c>).</summary>
	public IOrgAdminUsers OrgAdminUsers => field ??= For<IOrgAdminUsers>();
}
