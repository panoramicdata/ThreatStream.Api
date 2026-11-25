namespace ThreatStream.Api;

/// <summary>
/// Raised, before anything is sent, when a client created with <see cref="ThreatStreamClientOptions.IsReadOnly"/> is asked
/// to make a request that could change ThreatStream.
/// </summary>
/// <param name="method">The refused HTTP method.</param>
/// <param name="path">The refused request URL, without the query string.</param>
public sealed class ThreatStreamReadOnlyException(string method, string path)
	: InvalidOperationException($"The ThreatStream client is read-only and refused {method} {path}.")
{
	/// <summary>The refused HTTP method.</summary>
	public string Method { get; } = method;

	/// <summary>The refused request URL, without the query string.</summary>
	public string Path { get; } = path;
}
