using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ThreatStream.Api.Handlers;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public partial class RetryHandlerTests
{
	private const string Url = "https://threatstream.test/api/v2/intelligence/?value=secret.example";

	/// <summary>A retry handler over a stub, recording the delays it would wait instead of waiting.</summary>
	private sealed class Harness : IDisposable
	{
		private readonly HandlerHarness _harness;

		public Harness(Action<ThreatStreamClientOptions>? tweak = null, HttpMessageHandler? inner = null)
		{
			var options = new ThreatStreamClientOptions
			{
				MaxRetries = 3,
				RetryBaseDelay = TimeSpan.FromSeconds(1),
				MaxRetryDelay = TimeSpan.FromSeconds(30),
				Logger = Logger
			};
			tweak?.Invoke(options);
			var handler = new RetryHandler(options)
			{
				Delay = (delay, _) =>
				{
					Delays.Add(delay);
					return Task.CompletedTask;
				}
			};
			_harness = new HandlerHarness(handler, inner);
		}

		public StubHandler Stub => _harness.Stub;

		public List<TimeSpan> Delays { get; } = [];

		public CapturingLogger Logger { get; } = new();

		public Harness Respond(params HttpStatusCode[] statuses)
		{
			foreach (var status in statuses)
			{
				Stub.Enqueue(status);
			}

			return this;
		}

		public Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpContent? content = null)
			=> _harness.SendAsync(method, Url, content);

		public void Dispose() => _harness.Dispose();
	}

	[Theory]
	[InlineData("GET", HttpStatusCode.InternalServerError)]
	[InlineData("PUT", HttpStatusCode.BadGateway)]
	[InlineData("DELETE", HttpStatusCode.GatewayTimeout)]
	[InlineData("HEAD", HttpStatusCode.InternalServerError)]
	[InlineData("POST", HttpStatusCode.ServiceUnavailable)]
	[InlineData("POST", HttpStatusCode.TooManyRequests)]
	[InlineData("PATCH", HttpStatusCode.TooManyRequests)]
	public async Task RetriesTransientFailures_UntilSuccess(string method, HttpStatusCode status)
	{
		using var harness = new Harness().Respond(status, status, HttpStatusCode.OK);

		using var response = await harness.SendAsync(new HttpMethod(method));

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		harness.Stub.Calls.Should().HaveCount(3);
		harness.Delays.Should().Equal(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
	}

	[Theory]
	[InlineData("POST", HttpStatusCode.InternalServerError)]
	[InlineData("PATCH", HttpStatusCode.BadGateway)]
	[InlineData("GET", HttpStatusCode.NotFound)]
	[InlineData("GET", HttpStatusCode.Unauthorized)]
	[InlineData("GET", HttpStatusCode.OK)]
	public async Task DoesNotRetry_OtherResponses(string method, HttpStatusCode status)
	{
		using var harness = new Harness().Respond(status);

		using var response = await harness.SendAsync(new HttpMethod(method));

		response.StatusCode.Should().Be(status);
		harness.Stub.Calls.Should().ContainSingle();
		harness.Delays.Should().BeEmpty();
	}

	[Fact]
	public async Task StopsAfterMaxRetries_ReturningTheLastResponse()
	{
		using var harness = new Harness(o => o.MaxRetries = 2).Respond(HttpStatusCode.ServiceUnavailable, HttpStatusCode.ServiceUnavailable, HttpStatusCode.BadGateway);

		using var response = await harness.SendAsync(HttpMethod.Get);

		response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
		harness.Stub.Calls.Should().HaveCount(3);
	}

	[Fact]
	public async Task BackOff_DoublesUpToTheMaximum()
	{
		using var harness = new Harness(o => (o.MaxRetries, o.RetryBaseDelay, o.MaxRetryDelay) = (5, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10)))
			.Respond([.. Enumerable.Repeat(HttpStatusCode.ServiceUnavailable, 6)]);

		using var response = await harness.SendAsync(HttpMethod.Get);

		harness.Delays.Should().Equal(
			TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(6), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
	}

	[Fact]
	public async Task BackOff_ABaseAboveTheMaximumIsCapped()
	{
		using var harness = new Harness(o => (o.MaxRetries, o.RetryBaseDelay, o.MaxRetryDelay) = (1, TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10)))
			.Respond(HttpStatusCode.ServiceUnavailable, HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		harness.Delays.Should().Equal(TimeSpan.FromSeconds(10));
	}

	public static TheoryData<RetryConditionHeaderValue, TimeSpan> RetryAfterValues => new()
	{
		{ new RetryConditionHeaderValue(TimeSpan.FromSeconds(7)), TimeSpan.FromSeconds(7) },
		{ new RetryConditionHeaderValue(TimeSpan.FromSeconds(30)), TimeSpan.FromSeconds(30) },
		{ new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddDays(-1)), TimeSpan.Zero }
	};

	[Theory]
	[MemberData(nameof(RetryAfterValues))]
	public async Task RetryAfter_IsHonoured(RetryConditionHeaderValue retryAfter, TimeSpan expected)
	{
		using var harness = new Harness();
		harness.Stub.Enqueue(HttpStatusCode.TooManyRequests, configure: r => r.Headers.RetryAfter = retryAfter);
		harness.Stub.Enqueue(HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Post);

		harness.Delays.Should().Equal(expected);
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	public static TheoryData<RetryConditionHeaderValue> LongRetryAfterValues => new()
	{
		new RetryConditionHeaderValue(TimeSpan.FromSeconds(31)),
		new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddHours(1))
	};

	[Theory]
	[MemberData(nameof(LongRetryAfterValues))]
	public async Task RetryAfter_LongerThanTheMaximum_ReturnsTheRateLimitAtOnce(RetryConditionHeaderValue retryAfter)
	{
		using var harness = new Harness();
		harness.Stub.Enqueue(HttpStatusCode.TooManyRequests, configure: r => r.Headers.RetryAfter = retryAfter);

		using var response = await harness.SendAsync(HttpMethod.Get);

		response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		harness.Stub.Calls.Should().ContainSingle();
		harness.Delays.Should().BeEmpty();
		harness.Logger.Messages.Should().ContainSingle(m => m.Contains("longer than MaxRetryDelay; not retrying", StringComparison.Ordinal));
	}

	[Fact]
	public async Task RetryAfter_LongerThanTheMaximum_WithoutALogger_ReturnsTheRateLimit()
	{
		using var harness = new Harness(o => o.Logger = null);
		harness.Stub.Enqueue(HttpStatusCode.TooManyRequests, configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMinutes(1)));

		using var response = await harness.SendAsync(HttpMethod.Get);

		response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		harness.Logger.Entries.Should().BeEmpty();
	}

	[Fact]
	public async Task Client_RateLimitBeyondTheMaximum_RaisesTheExceptionWithRetryAfter()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.TooManyRequests, """{"error":"Rate limit exceeded"}""", r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(90)));
		using var client = TestClient.Create(stub, o => o.MaxRetries = 3);

		var act = () => client.Confidence.GetTrendAsync("1.2.3.4", TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<ThreatStreamApiException>();
		thrown.Which.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		thrown.Which.Message.Should().Be("Rate limit exceeded");
		thrown.Which.RetryAfter.Should().Be(TimeSpan.FromSeconds(90));
		stub.Calls.Should().ContainSingle();
	}

	public static TheoryData<string, Func<HttpContent>> ReplayableContent => new()
	{
		{ """{"x":1}""", () => new StringContent("""{"x":1}""", Encoding.UTF8, "application/json") },
		{ """{"y":2}""", () => JsonContent.Create(new Dictionary<string, int> { ["y"] = 2 }) },
		{ "raw", () => new ByteArrayContent("raw"u8.ToArray()) },
		{ "memory", () => new ReadOnlyMemoryContent("memory"u8.ToArray()) }
	};

	[Theory]
	[MemberData(nameof(ReplayableContent))]
	public async Task BufferedBodies_AreReplayedIdentically(string body, Func<HttpContent> content)
	{
		using var harness = new Harness().Respond(HttpStatusCode.ServiceUnavailable, HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Post, content());

		harness.Stub.Calls.Select(c => c.Body).Should().Equal(body, body);
	}

	[Fact]
	public async Task MultipartBodies_OfBufferedParts_AreReplayed()
	{
		using var harness = new Harness().Respond(HttpStatusCode.TooManyRequests, HttpStatusCode.OK);
		using var content = new MultipartFormDataContent { { new StringContent("v"), "field" }, { new ByteArrayContent([1, 2]), "file", "a.bin" } };

		using var response = await harness.SendAsync(HttpMethod.Post, content);

		harness.Stub.Calls.Should().HaveCount(2);
		harness.Stub.Calls.Should().AllSatisfy(c => c.Parts.Select(p => p.Name).Should().Equal("field", "file"));
	}

	[Fact]
	public async Task StreamBodies_AreNotRetried()
	{
		using var harness = new Harness().Respond(HttpStatusCode.ServiceUnavailable);

		using var response = await harness.SendAsync(HttpMethod.Post, new StreamContent(new MemoryStream("data"u8.ToArray())));

		response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
		harness.Stub.Calls.Should().ContainSingle();
	}

	public static TheoryData<Func<HttpContent?>, bool> Replayability => new()
	{
		{ () => null, true },
		{ () => new StringContent("x"), true },
		{ () => JsonContent.Create(1), true },
		{ () => new MultipartFormDataContent(), true },
		{ () => new MultipartFormDataContent { new StringContent("x") }, true },
		{ () => new StreamContent(Stream.Null), false },
		{ () => new MultipartFormDataContent { new StringContent("x"), new StreamContent(Stream.Null) }, false }
	};

	[Theory]
	[MemberData(nameof(Replayability))]
	public void IsReplayable_OnlyForBufferedContent(Func<HttpContent?> create, bool expected)
	{
		using var content = create();

		RetryHandler.IsReplayable(content).Should().Be(expected);
	}
}
