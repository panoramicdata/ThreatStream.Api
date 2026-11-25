using System.Net;

namespace ThreatStream.Api;

/// <summary>Raised when ThreatStream returns a non-success response.</summary>
/// <param name="statusCode">The HTTP status code.</param>
/// <param name="message">
/// The exception message: the error text ThreatStream returned (a JSON <c>message</c>, <c>error</c>, <c>detail</c>,
/// <c>reason</c> or <c>error_message</c>, else the plain-text body), or a fallback naming the status.
/// </param>
/// <param name="retryAfter">How long ThreatStream asked the caller to wait (<c>Retry-After</c>), if it said.</param>
public sealed class ThreatStreamApiException(HttpStatusCode statusCode, string message, TimeSpan? retryAfter)
	: Exception(message)
{
	/// <summary>The HTTP status code.</summary>
	public HttpStatusCode StatusCode { get; } = statusCode;

	/// <summary>
	/// How long ThreatStream asked the caller to wait before trying again (from <c>Retry-After</c>, typically on a 429 when
	/// the rate limit is exhausted), or <see langword="null"/> when it did not say.
	/// </summary>
	public TimeSpan? RetryAfter { get; } = retryAfter;
}
