using System.Net;
using ThreatStream.Api.Handlers;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public partial class AuthenticationHandlerTests
{
	private const string Trend = """{"min_confidence":1}""";

	private static CancellationToken Ct => TestContext.Current.CancellationToken;

	/// <summary>An OAuth client over <paramref name="stub"/>, on a clock the test controls.</summary>
	private static ThreatStreamClient OAuthClient(StubHandler stub, ManualTimeProvider clock, Action<ThreatStreamClientOptions>? tweak = null)
		=> TestClient.Create(stub, o =>
		{
			TestClient.UseOAuth(o);
			o.TimeProvider = clock;
			tweak?.Invoke(o);
		});

	[Fact]
	public async Task ApiKey_IsSentOnEveryRequest()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, Trend);
		stub.Enqueue(HttpStatusCode.OK, Trend);
		using var client = TestClient.Create(stub);

		await client.Confidence.GetTrendAsync("a", Ct);
		await client.Confidence.GetTrendAsync("b", Ct);

		stub.Calls.Select(c => c.Authorization).Should().AllBe("apikey analyst@example.com:fake-key");
	}

	[Fact]
	public async Task OAuth_ObtainsATokenOnce_AndSendsItAsBearer()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-1"));
		stub.Enqueue(HttpStatusCode.OK, Trend);
		stub.Enqueue(HttpStatusCode.OK, Trend);
		using var client = OAuthClient(stub, new ManualTimeProvider());

		await client.Confidence.GetTrendAsync("a", Ct);
		await client.Confidence.GetTrendAsync("b", Ct);

		stub.Calls.Should().HaveCount(3);
		var token = stub.Calls[0];
		token.Method.Should().Be(HttpMethod.Post);
		token.Uri.Should().Be(new Uri("https://threatstream.test/oauth/token"));
		token.ContentType.Should().Be("application/x-www-form-urlencoded");
		token.Body.Should().Be("grant_type=client_credentials&client_id=client-id&client_secret=client-secret");
		token.Authorization.Should().BeNull();
		stub.Calls.Skip(1).Select(c => c.Authorization).Should().AllBe("Bearer token-1");
	}

	[Theory]
	[InlineData(900, 839, false)]
	[InlineData(900, 840, true)]
	[InlineData(60, 29, false)]
	[InlineData(60, 30, true)]
	public async Task OAuth_RenewsTheTokenBeforeItExpires(int expiresIn, int elapsedSeconds, bool renewed)
	{
		var clock = new ManualTimeProvider();
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-1", expiresIn));
		stub.Enqueue(HttpStatusCode.OK, Trend);
		stub.Enqueue(HttpStatusCode.OK, renewed ? TestClient.TokenJson("token-2") : Trend);
		stub.Enqueue(HttpStatusCode.OK, Trend);
		using var client = OAuthClient(stub, clock);

		await client.Confidence.GetTrendAsync("a", Ct);
		clock.Advance(TimeSpan.FromSeconds(elapsedSeconds));
		await client.Confidence.GetTrendAsync("b", Ct);

		stub.Calls.Should().HaveCount(renewed ? 4 : 3);
		stub.Calls[^1].Authorization.Should().Be(renewed ? "Bearer token-2" : "Bearer token-1");
	}

	[Fact]
	public async Task OAuth_Unauthorized_RenewsOnceAndResends()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-1"));
		stub.Enqueue(HttpStatusCode.Unauthorized);
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-2"));
		stub.Enqueue(HttpStatusCode.OK, Trend);
		using var client = OAuthClient(stub, new ManualTimeProvider());

		var trend = await client.Confidence.GetTrendAsync("a", Ct);

		trend.MinConfidence.Should().Be(1);
		stub.Calls.Select(c => c.Authorization).Should().Equal(null, "Bearer token-1", null, "Bearer token-2");
	}

	[Fact]
	public async Task OAuth_UnauthorizedTwice_RaisesTheSecond401()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-1"));
		stub.Enqueue(HttpStatusCode.Unauthorized);
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-2"));
		stub.Enqueue(HttpStatusCode.Unauthorized, """{"detail":"Token revoked"}""");
		using var client = OAuthClient(stub, new ManualTimeProvider());

		await TestClient.ShouldFailWithAsync(() => client.Confidence.GetTrendAsync("a", Ct), HttpStatusCode.Unauthorized, "Token revoked");
		stub.Calls.Should().HaveCount(4);
	}

	[Fact]
	public async Task OAuth_Unauthorized_WithAStreamBody_IsNotResent()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-1"));
		stub.Enqueue(HttpStatusCode.Unauthorized);
		using var client = OAuthClient(stub, new ManualTimeProvider());

		using var response = await SendStreamAsync(client);

		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		stub.Calls.Should().HaveCount(2);
	}

	[Fact]
	public async Task OAuth_FailedTokenRequest_RaisesItsError_AttributedToTheCallersRequest()
	{
		var stub = TestClient.Stub("""{"error":"invalid_client"}""", HttpStatusCode.BadRequest);
		using var client = OAuthClient(stub, new ManualTimeProvider());

		await TestClient.ShouldFailWithAsync(() => client.Confidence.GetTrendAsync("a", Ct), HttpStatusCode.BadRequest, "invalid_client");
		stub.Calls.Should().ContainSingle();
	}

	[Fact]
	public async Task OAuth_FailedRenewal_AfterUnauthorized_RaisesTheRenewalError()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("token-1"));
		stub.Enqueue(HttpStatusCode.Unauthorized);
		stub.Enqueue(HttpStatusCode.Unauthorized, """{"error":"client disabled"}""");
		using var client = OAuthClient(stub, new ManualTimeProvider());

		await TestClient.ShouldFailWithAsync(() => client.Confidence.GetTrendAsync("a", Ct), HttpStatusCode.Unauthorized, "client disabled");
	}

	[Theory]
	[InlineData("oauth/token")]
	[InlineData("oauth/token/")]
	public async Task OAuth_ExplicitTokenRequest_IsSentAsGiven(string path)
	{
		var stub = TestClient.Stub(TestClient.TokenJson("t"));
		using var client = OAuthClient(stub, new ManualTimeProvider());

		await client.For<IProbe>().PostAsync(path, new Dictionary<string, string?> { ["grant_type"] = "client_credentials" }, Ct);

		stub.Calls.Should().ContainSingle().Which.Authorization.Should().BeNull();
	}

	[Fact]
	public async Task OAuth_LogsTheTokenLifetime_NeverTheToken()
	{
		var logger = new CapturingLogger();
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("secret-token"));
		stub.Enqueue(HttpStatusCode.OK, Trend);
		using var client = OAuthClient(stub, new ManualTimeProvider(), o => o.Logger = logger);

		await client.Confidence.GetTrendAsync("a", Ct);

		logger.Messages.Should().Contain("ThreatStream OAuth token obtained, valid for 00:15:00");
		logger.Messages.Should().NotContain(m => m.Contains("secret", StringComparison.Ordinal));
	}

	private static Task<HttpResponseMessage> SendStreamAsync(ThreatStreamClient client)
		=> client.For<IStreamProbe>().PostAsync(new StreamContent(new MemoryStream([1, 2, 3])), Ct);

	/// <summary>Sends a raw <see cref="HttpContent"/> body through a client's pipeline.</summary>
	public interface IStreamProbe
	{
		[Refit.Post("api/v1/probe/")]
		Task<HttpResponseMessage> PostAsync([Refit.Body] HttpContent content, CancellationToken cancellationToken);
	}
}
