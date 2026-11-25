using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Keyword-match rules (<c>api/v1/rule/</c>).</summary>
	public IRules Rules => field ??= For<IRules>();
}
