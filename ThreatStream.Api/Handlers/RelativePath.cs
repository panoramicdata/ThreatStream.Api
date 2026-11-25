namespace ThreatStream.Api.Handlers;

/// <summary>Locates a request within the client's base address.</summary>
internal static class RelativePath
{
	/// <summary>
	/// The escaped path of <paramref name="requestUri"/> after <paramref name="baseUri"/>'s path, without a query string,
	/// such as <c>api/v2/intelligence/</c>; <see langword="null"/> when the request is for another origin or lies outside
	/// the base path.
	/// </summary>
	public static string? Of(Uri baseUri, Uri requestUri)
	{
		if (requestUri.Scheme != baseUri.Scheme
			|| !string.Equals(requestUri.Authority, baseUri.Authority, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		var basePath = baseUri.AbsolutePath;
		var path = requestUri.AbsolutePath;
		return path.StartsWith(basePath, StringComparison.Ordinal) ? path[basePath.Length..] : null;
	}
}
