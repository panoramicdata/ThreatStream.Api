using Refit;
using ThreatStream.Api.Models.ObservableTags;

namespace ThreatStream.Api.Interfaces;

/// <summary>Tags on observables in bulk (<c>api/v2/intelligence/bulk_tagging/</c>).</summary>
/// <remarks>
/// The endpoint has no documented rate limit, so ThreatStream's default applies: one request per minute per user.
/// </remarks>
public interface IObservableTags
{
	/// <summary>Adds and removes tags on the given observables (<c>POST api/v2/intelligence/bulk_tagging/</c>).</summary>
	/// <param name="request">The observables and the tags to add and remove.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has applied the tags.</returns>
	/// <remarks>
	/// <para>
	/// <b>Side effect:</b> changes the observables' tags; public tags are visible to every ThreatStream user. Refused by a
	/// read-only client. Up to 250 tags can be managed this way; use <see cref="BulkTagMatchingAsync"/> for more. The
	/// reference documents no response body.
	/// </para>
	/// <para>
	/// The reference's removal example sends <c>{"remove_tags": [{"name": ["test", "tagB"]}]}</c>, while its attribute
	/// table specifies one object per tag (<c>[{"name": "test"}, {"name": "tagB"}]</c>); this method sends the table's form.
	/// </para>
	/// </remarks>
	[Post("api/v2/intelligence/bulk_tagging/")]
	Task BulkTagAsync([Body] ObservableTagRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Starts an asynchronous job that adds and removes tags on up to 10,000 observables selected by a query
	/// (<c>POST api/v2/intelligence/bulk_tagging/?async_result=true</c>).
	/// </summary>
	/// <param name="request">The query selecting the observables, and the tags to add and remove.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The job, to poll until it is done.</returns>
	/// <remarks>
	/// <b>Side effect:</b> changes the tags of every observable the query selects. Refused by a read-only client. An
	/// organization can run one such job at a time: starting another while one runs fails. The reference spells the job's
	/// <c>resource_uri</c> <c>/api/vl/...</c>; ThreatStream's path is <c>/api/v1/generic_async_result/{id}/</c>.
	/// </remarks>
	[Post("api/v2/intelligence/bulk_tagging/?async_result=true")]
	Task<BulkTaggingJob> BulkTagMatchingAsync([Body] ObservableTagQueryRequest request, CancellationToken cancellationToken);
}
