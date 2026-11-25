using Microsoft.Extensions.Logging;

namespace ThreatStream.Api.Handlers;

/// <summary>Source-generated log messages. Only methods and paths are logged, never query strings or credentials.</summary>
internal static partial class Log
{
	[LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "ThreatStream {Method} {Path} (attempt {Attempt})")]
	public static partial void Sending(ILogger logger, HttpMethod method, string path, int attempt);

	[LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "ThreatStream returned {Status} for {Method} {Path}; retrying in {Delay}")]
	public static partial void Retrying(ILogger logger, int status, HttpMethod method, string path, TimeSpan delay);

	[LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "ThreatStream connection failed ({Error}) for {Method} {Path}; retrying in {Delay}")]
	public static partial void RetryingConnection(ILogger logger, HttpRequestError error, HttpMethod method, string path, TimeSpan delay);

	[LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "ThreatStream returned {Status} for {Method} {Path} asking to wait {RetryAfter}, longer than MaxRetryDelay; not retrying")]
	public static partial void RetryAfterTooLong(ILogger logger, int status, HttpMethod method, string path, TimeSpan retryAfter);

	[LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "ThreatStream OAuth token obtained, valid for {Lifetime}")]
	public static partial void TokenObtained(ILogger logger, TimeSpan lifetime);
}
