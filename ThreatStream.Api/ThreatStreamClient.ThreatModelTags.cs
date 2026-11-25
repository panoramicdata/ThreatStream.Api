using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Tags on threat model entities in bulk (<c>api/v1/{t}/bulk_tagging/</c>).</summary>
	public IThreatModelTags ThreatModelTags => field ??= For<IThreatModelTags>();
}
