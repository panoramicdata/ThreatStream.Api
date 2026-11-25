using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Workgroups;

namespace ThreatStream.Api.Interfaces;

/// <summary>The workgroups of your organization (<c>api/v1/workgroup/</c>).</summary>
public interface IWorkgroups
{
	/// <summary>
	/// Lists the workgroups of your organization, with the IDs that <see cref="IObservableWorkgroups.RestrictAsync"/> and
	/// imports take (<c>GET api/v1/workgroup/</c>).
	/// </summary>
	/// <param name="options">Paging and ordering, or <see langword="null"/> for the first 20 workgroups.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of workgroups; page through them with <see cref="ThreatStreamClient.EnumerateAsync{T}"/>.</returns>
	/// <remarks>
	/// Not one of the endpoints with a documented rate limit, so ThreatStream's default for those applies: one request
	/// per minute per user.
	/// </remarks>
	[Get("api/v1/workgroup/")]
	Task<ThreatStreamPage<Workgroup>> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}
