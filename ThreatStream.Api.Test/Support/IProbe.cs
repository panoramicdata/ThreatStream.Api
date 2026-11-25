using Refit;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Test.Support;

/// <summary>Sends any verb to any path through a client's full pipeline (via <c>ThreatStreamClient.For&lt;IProbe&gt;()</c>).</summary>
public interface IProbe
{
	[Get("{**path}")]
	Task<ThreatStreamPage<ProbeObject>> GetAsync(string path, CancellationToken cancellationToken);

	[Post("{**path}")]
	Task PostAsync(string path, [Body] IDictionary<string, string?> body, CancellationToken cancellationToken);

	[Patch("{**path}")]
	Task PatchAsync(string path, [Body] IDictionary<string, string?> body, CancellationToken cancellationToken);

	[Delete("{**path}")]
	Task DeleteAsync(string path, CancellationToken cancellationToken);
}

/// <summary>Any ThreatStream object, for probes.</summary>
public sealed class ProbeObject : ThreatStreamResource
{
	/// <summary>The object's name.</summary>
	public string? Name { get; init; }

	/// <summary>The object's <c>update_id</c>.</summary>
	public long UpdateId { get; init; }
}
