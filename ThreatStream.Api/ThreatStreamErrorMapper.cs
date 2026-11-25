using System.Text.Json;

namespace ThreatStream.Api;

/// <summary>
/// Turns a non-success response into a <see cref="ThreatStreamApiException"/>. ThreatStream documents no error format, so
/// the message is read tolerantly: from a JSON object's <c>message</c>, <c>error</c>, <c>detail</c>, <c>reason</c> or
/// <c>error_message</c> (a string, or an object holding one of those), else from a plain-text body, else a fallback naming
/// the status. HTML error pages are not used as messages.
/// </summary>
internal static class ThreatStreamErrorMapper
{
	/// <summary>The longest plain-text body used as a message.</summary>
	internal const int MaxTextLength = 500;

	private static readonly string[] MessageProperties = ["message", "error", "detail", "reason", "error_message"];

	public static async Task<Exception?> CreateAsync(HttpResponseMessage response)
	{
		if (response.IsSuccessStatusCode)
		{
			return null;
		}

		var body = await ReadBodyAsync(response).ConfigureAwait(false);
		var text = ParseMessage(body)
			?? $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase ?? response.StatusCode.ToString()})";
		return new ThreatStreamApiException(response.StatusCode, text, RetryAfter(response));
	}

	/// <summary>The error text in <paramref name="body"/>, or <see langword="null"/> when it holds none.</summary>
	internal static string? ParseMessage(string body)
	{
		var trimmed = body.Trim();
		if (trimmed.Length == 0 || trimmed.StartsWith('<'))
		{
			return null;
		}

		if (trimmed.StartsWith('{') || trimmed.StartsWith('['))
		{
			return ParseJson(trimmed);
		}

		return trimmed.Length > MaxTextLength ? trimmed[..MaxTextLength] + "..." : trimmed;
	}

	private static string? ParseJson(string body)
	{
		try
		{
			using var document = JsonDocument.Parse(body);
			return FindMessage(document.RootElement, 0);
		}
		catch (JsonException)
		{
			return null;
		}
	}

	private static string? FindMessage(JsonElement element, int depth)
	{
		if (element.ValueKind != JsonValueKind.Object)
		{
			return null;
		}

		foreach (var name in MessageProperties)
		{
			if (!element.TryGetProperty(name, out var property))
			{
				continue;
			}

			var text = property.ValueKind == JsonValueKind.String ? property.GetString()!.Trim() : string.Empty;
			if (text.Length > 0)
			{
				return text;
			}

			// One level down: {"error": {"message": "..."}}.
			if (depth == 0 && FindMessage(property, 1) is { } nested)
			{
				return nested;
			}
		}

		return null;
	}

	/// <summary>The wait the response asks for: a <c>Retry-After</c> delta, or the time until its date (never negative).</summary>
	internal static TimeSpan? RetryAfter(HttpResponseMessage response)
	{
		var header = response.Headers.RetryAfter;
		if (header?.Delta is { } delta)
		{
			return delta;
		}

		if (header?.Date is { } date)
		{
			var wait = date - DateTimeOffset.UtcNow;
			return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
		}

		return null;
	}

	private static async Task<string> ReadBodyAsync(HttpResponseMessage response)
	{
		try
		{
			return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		}
		catch (Exception exception) when (exception is InvalidOperationException or HttpRequestException or IOException)
		{
			// An unsupported charset in Content-Type, or a connection lost while reading: report the status, not the read.
			return string.Empty;
		}
	}
}
