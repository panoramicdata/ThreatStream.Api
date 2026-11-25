using Refit;

namespace ThreatStream.Api.Models.Rules;

/// <summary>The parameters of <c>GET api/v1/rule/</c>: paging and ordering, and on ThreatStream OnPrem the choice of remote rules.</summary>
public sealed class RuleListOptions : ListOptions
{
	/// <summary>
	/// ThreatStream OnPrem only: <see langword="true"/> lists the rules stored on ThreatStream Cloud (<c>remote_api</c>).
	/// Leave it <see langword="null"/> for local rules: the reference warns that sending <see langword="false"/> causes errors.
	/// </summary>
	[AliasAs("remote_api")]
	public bool? RemoteApi { get; set; }
}
