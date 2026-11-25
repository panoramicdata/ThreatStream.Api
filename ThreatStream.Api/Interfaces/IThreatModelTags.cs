using Refit;
using ThreatStream.Api.Models.ThreatModelTags;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Interfaces;

/// <summary>Tags on threat model entities in bulk (<c>api/v1/{t}/bulk_tagging/</c>).</summary>
public interface IThreatModelTags
{
	/// <summary>Adds and removes tags on threat model entities of one type (<c>POST api/v1/{t}/bulk_tagging/</c>).</summary>
	/// <param name="entityType">The type of the entities, such as <see cref="ThreatModelType.Actor"/>.</param>
	/// <param name="request">The entities and the tags to add and remove.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has applied the tags.</returns>
	/// <remarks>
	/// <b>Side effect:</b> changes the entities' tags; public tags are visible to every ThreatStream user. Refused by a
	/// read-only client. By default up to 25 tags can be removed from an entity at once. The endpoint has no documented
	/// rate limit, so ThreatStream's default applies: one request per minute per user. The reference documents no response
	/// body.
	/// </remarks>
	[Post("api/v1/{entityType}/bulk_tagging/")]
	Task BulkTagAsync(ThreatModelType entityType, [Body] ThreatModelTagRequest request, CancellationToken cancellationToken);
}
