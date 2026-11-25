using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ThreatStream.Api.Handlers;

/// <summary>
/// Authenticates each request: <c>Authorization: apikey &lt;username&gt;:&lt;api_key&gt;</c>, or an OAuth 2.0 bearer token
/// obtained from <c>oauth/token</c> with the client-credentials grant. A token is renewed shortly before it expires; a 401
/// response discards it, and the request is sent once more with a new one when its body can be replayed. A failed token
/// request's response is returned in place of the request's, so it surfaces as <see cref="ThreatStreamApiException"/>.
/// The options are copied at construction.
/// </summary>
internal sealed class AuthenticationHandler : DelegatingHandler
{
	internal const string TokenPath = "oauth/token";

	/// <summary>The lifetime assumed when the token response gives none (ThreatStream documents 900 seconds).</summary>
	internal static readonly TimeSpan DefaultTokenLifetime = TimeSpan.FromSeconds(900);

	/// <summary>How long before expiry a token is renewed, at most (a short-lived token is renewed at half its lifetime).</summary>
	internal static readonly TimeSpan RenewalMargin = TimeSpan.FromSeconds(60);

	/// <summary>The longest <c>expires_in</c> believed (a year); anything longer, zero, negative or unreadable is the default.</summary>
	private const double MaxLifetimeSeconds = 365 * 24 * 60 * 60;

	private readonly AuthenticationKind _kind;
	private readonly AuthenticationHeaderValue? _apiKeyHeader;
	private readonly string? _clientId;
	private readonly string? _clientSecret;
	private readonly Uri _baseUri;
	private readonly Uri _tokenUri;
	private readonly TimeProvider _timeProvider;
	private readonly ILogger? _logger;
	private readonly SemaphoreSlim _tokenLock = new(1, 1);
	private Token? _token;

	public AuthenticationHandler(ThreatStreamClientOptions options, Uri baseUri)
	{
		_kind = options.AuthenticationKind;
		_apiKeyHeader = _kind == AuthenticationKind.ApiKey ? new AuthenticationHeaderValue("apikey", $"{options.Username}:{options.ApiKey}") : null;
		_clientId = options.ClientId;
		_clientSecret = options.ClientSecret;
		_baseUri = baseUri;
		_tokenUri = new Uri(baseUri, TokenPath);
		_timeProvider = options.TimeProvider;
		_logger = options.Logger;
	}

	/// <inheritdoc />
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (_kind == AuthenticationKind.ApiKey)
		{
			request.Headers.Authorization = _apiKeyHeader;
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}

		if (RelativePath.Of(_baseUri, request.RequestUri!) is TokenPath or TokenPath + "/")
		{
			// An explicit call to the token endpoint carries its own credentials.
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}

		var token = await GetTokenAsync(null, cancellationToken).ConfigureAwait(false);
		if (token.Failure is { } failure)
		{
			return InPlaceOf(failure, request);
		}

		var response = await SendWithTokenAsync(request, token.Value!, cancellationToken).ConfigureAwait(false);
		if (response.StatusCode != HttpStatusCode.Unauthorized || !RetryHandler.IsReplayable(request.Content))
		{
			return response;
		}

		// The token was revoked or expired early: obtain a new one once, then resend.
		response.Dispose();
		token = await GetTokenAsync(token.Value, cancellationToken).ConfigureAwait(false);
		return token.Failure is { } renewalFailure
			? InPlaceOf(renewalFailure, request)
			: await SendWithTokenAsync(request, token.Value!, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Attributes a failed token response to the caller's request. The transport set its request to the token request,
	/// whose (disposed, but still referenced) form content holds the client secret.
	/// </summary>
	private static HttpResponseMessage InPlaceOf(HttpResponseMessage failure, HttpRequestMessage request)
	{
		failure.RequestMessage = request;
		return failure;
	}

	private Task<HttpResponseMessage> SendWithTokenAsync(HttpRequestMessage request, string token, CancellationToken cancellationToken)
	{
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		return base.SendAsync(request, cancellationToken);
	}

	/// <summary>
	/// Returns a usable token, requesting one when there is none, when the current one is due for renewal, or when it is
	/// <paramref name="staleToken"/> (concurrent requests that all saw the same 401 request only one new token).
	/// </summary>
	private async Task<TokenResult> GetTokenAsync(string? staleToken, CancellationToken cancellationToken)
	{
		if (Usable(_token, staleToken) is { } current)
		{
			return new(current, null);
		}

		await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			if (Usable(_token, staleToken) is { } renewed)
			{
				return new(renewed, null);
			}

			return await RequestTokenAsync(cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			_tokenLock.Release();
		}
	}

	private string? Usable(Token? token, string? staleToken)
		=> token is not null && token.Value != staleToken && _timeProvider.GetUtcNow() < token.RenewAt ? token.Value : null;

	private async Task<TokenResult> RequestTokenAsync(CancellationToken cancellationToken)
	{
		using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _tokenUri)
		{
			Content = new FormUrlEncodedContent(
			[
				new("grant_type", "client_credentials"),
				new("client_id", _clientId!),
				new("client_secret", _clientSecret!)
			])
		};
		var requestedAt = _timeProvider.GetUtcNow();
		var response = await base.SendAsync(tokenRequest, cancellationToken).ConfigureAwait(false);
		if (!response.IsSuccessStatusCode)
		{
			return new(null, response);
		}

		using (response)
		{
			var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			var (value, lifetime) = ReadToken(body);
			if (_logger is not null)
			{
				Log.TokenObtained(_logger, lifetime);
			}

			_token = new Token(value, requestedAt + lifetime - Margin(lifetime));
			return new(value, null);
		}
	}

	/// <summary>Renew <see cref="RenewalMargin"/> before expiry, or at half the lifetime of a token shorter than twice that.</summary>
	internal static TimeSpan Margin(TimeSpan lifetime) => lifetime >= RenewalMargin * 2 ? RenewalMargin : lifetime / 2;

	/// <summary>Reads <c>access_token</c> and <c>expires_in</c> (seconds, as a number or numeric string) from a token response.</summary>
	internal static (string Value, TimeSpan Lifetime) ReadToken(string body)
	{
		try
		{
			using var document = JsonDocument.Parse(body);
			var root = document.RootElement;
			if (root.ValueKind == JsonValueKind.Object
				&& root.TryGetProperty("access_token", out var token)
				&& token.ValueKind == JsonValueKind.String
				&& token.GetString()!.Length > 0)
			{
				return (token.GetString()!, ReadLifetime(root));
			}
		}
		catch (JsonException)
		{
			// Fall through: the body is not the expected JSON.
		}

		throw new InvalidOperationException("ThreatStream's OAuth token response did not contain an access token.");
	}

	private static TimeSpan ReadLifetime(JsonElement root)
	{
		if (!root.TryGetProperty("expires_in", out var expiresIn))
		{
			return DefaultTokenLifetime;
		}

		var text = expiresIn.ValueKind switch
		{
			JsonValueKind.Number => expiresIn.GetRawText(),
			JsonValueKind.String => expiresIn.GetString(),
			_ => null
		};
		return double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var seconds)
			&& seconds > 0
			&& seconds <= MaxLifetimeSeconds
				? TimeSpan.FromSeconds(seconds)
				: DefaultTokenLifetime;
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_tokenLock.Dispose();
		}

		base.Dispose(disposing);
	}

	/// <summary>An access token and when to renew it.</summary>
	private sealed record Token(string Value, DateTimeOffset RenewAt);

	/// <summary>A token, or the failed token response to return in place of the request's.</summary>
	private sealed record TokenResult(string? Value, HttpResponseMessage? Failure);
}
