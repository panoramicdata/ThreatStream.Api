using System.Net;

namespace ThreatStream.Api.Test.Support;

internal static class TestClient
{
	public const string BaseUrl = "https://threatstream.test/";

	/// <summary>The Authorization header every request of a <see cref="Create"/> client carries.</summary>
	public const string ApiKeyAuthorization = "apikey analyst@example.com:fake-key";

	/// <summary>A client for a custom deployment at <see cref="BaseUrl"/>, with API key authentication and no retries.</summary>
	public static ThreatStreamClient Create(StubHandler stub, Action<ThreatStreamClientOptions>? tweak = null)
		=> Create((HttpMessageHandler)stub, tweak);

	/// <summary>As <see cref="Create(StubHandler, Action{ThreatStreamClientOptions}?)"/>, over any transport.</summary>
	public static ThreatStreamClient Create(HttpMessageHandler transport, Action<ThreatStreamClientOptions>? tweak = null)
	{
		var options = new ThreatStreamClientOptions
		{
			Region = ThreatStreamRegion.Custom,
			BaseUrl = BaseUrl,
			Username = "analyst@example.com",
			ApiKey = "fake-key",
			MaxRetries = 0
		};
		tweak?.Invoke(options);
		return new ThreatStreamClient(options, transport);
	}

	/// <summary>Switches options to OAuth 2.0 client credentials (<c>client-id</c>/<c>client-secret</c>).</summary>
	public static void UseOAuth(ThreatStreamClientOptions options)
	{
		options.Username = null;
		options.ApiKey = null;
		options.ClientId = "client-id";
		options.ClientSecret = "client-secret";
	}

	/// <summary>A successful <c>oauth/token</c> response.</summary>
	public static string TokenJson(string accessToken, int expiresIn = 900)
		=> $$"""{"access_token":"{{accessToken}}","token_type":"Bearer","expires_in":{{expiresIn}}}""";

	/// <summary>A stub that answers one request with <paramref name="json"/>.</summary>
	public static StubHandler Stub(string json, HttpStatusCode status = HttpStatusCode.OK)
	{
		var stub = new StubHandler();
		stub.Enqueue(status, json);
		return stub;
	}

	/// <summary>
	/// Sends <paramref name="call"/> through a client whose transport answers <paramref name="response"/>, and returns the
	/// one request it made. Every test kit's capture helper delegates here.
	/// </summary>
	public static async Task<RecordedCall> CaptureAsync(Func<ThreatStreamClient, CancellationToken, Task> call, string response)
	{
		var stub = Stub(response);
		using var client = Create(stub);
		await call(client, TestContext.Current.CancellationToken);
		return stub.Calls.Should().ContainSingle().Subject;
	}

	/// <summary>Sends <paramref name="call"/> through a client whose transport answers <paramref name="response"/>, and returns what it read.</summary>
	public static async Task<T> ReadAsync<T>(Func<ThreatStreamClient, CancellationToken, Task<T>> call, string response)
	{
		using var client = Create(Stub(response));
		return await call(client, TestContext.Current.CancellationToken);
	}

	/// <summary>Asserts that <paramref name="call"/> raises <see cref="ThreatStreamApiException"/> when ThreatStream answers <paramref name="status"/> and <paramref name="response"/>.</summary>
	public static async Task ShouldFailAsync(Func<ThreatStreamClient, CancellationToken, Task> call, HttpStatusCode status, string response, string message)
	{
		using var client = Create(Stub(response, status));
		await ShouldFailWithAsync(() => call(client, TestContext.Current.CancellationToken), status, message);
	}

	/// <summary>Asserts that <paramref name="act"/> raises <see cref="ThreatStreamApiException"/> with the status and message.</summary>
	public static async Task ShouldFailWithAsync(Func<Task> act, HttpStatusCode status, string message)
	{
		var thrown = await act.Should().ThrowAsync<ThreatStreamApiException>();
		thrown.Which.StatusCode.Should().Be(status);
		thrown.Which.Message.Should().Be(message);
	}
}
