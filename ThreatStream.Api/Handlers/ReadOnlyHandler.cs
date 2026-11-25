using System.Text.RegularExpressions;

namespace ThreatStream.Api.Handlers;

/// <summary>
/// Refuses, before anything is sent, every request that could change ThreatStream: any method other than GET or HEAD,
/// except POSTs to the read-only operations in <see cref="ReadOnlyPosts"/>. See
/// <see cref="ThreatStreamClientOptions.IsReadOnly"/> for the documented allow-list.
/// </summary>
internal sealed partial class ReadOnlyHandler(Uri baseUri) : DelegatingHandler
{
	/// <summary>
	/// POST endpoints that read but do not change ThreatStream, each matched exactly (trailing slash optional): obtaining
	/// an OAuth token; an observable query or a threat model search too long for a URL (over 4000 characters), which
	/// ThreatStream takes as a POST; and a Recorded Future enrichment lookup, which ThreatStream documents as read-only.
	/// </summary>
	[GeneratedRegex(@"^(oauth/token|api/v2/intelligence|api/v1/threat_model_search|api/v1/integration_package/direct)/?$", RegexOptions.CultureInvariant)]
	private static partial Regex ReadOnlyPosts();

	/// <inheritdoc />
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (!IsAllowed(request))
		{
			throw new ThreatStreamReadOnlyException(request.Method.Method, request.RequestUri!.GetLeftPart(UriPartial.Path));
		}

		return base.SendAsync(request, cancellationToken);
	}

	/// <summary>
	/// GET operations that start a server-side job and so are not read-only: generating a PDF report. A request that asks
	/// for an asynchronous result (<c>async_result=true</c>, used by user audit reports) is refused too.
	/// </summary>
	[GeneratedRegex(@"^api/v1/alfred/report/?$", RegexOptions.CultureInvariant)]
	private static partial Regex JobStartingGets();

	private bool StartsJob(HttpRequestMessage request)
		=> request.RequestUri!.Query.Contains("async_result=true", StringComparison.OrdinalIgnoreCase)
			|| (RelativePath.Of(baseUri, request.RequestUri) is { } relative && JobStartingGets().IsMatch(relative));

	private bool IsAllowed(HttpRequestMessage request)
	{
		if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
		{
			return !StartsJob(request);
		}

		return request.Method == HttpMethod.Post
			&& RelativePath.Of(baseUri, request.RequestUri!) is { } relative
			&& ReadOnlyPosts().IsMatch(relative);
	}
}
