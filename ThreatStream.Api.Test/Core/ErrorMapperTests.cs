using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public class ErrorMapperTests
{
	private static async Task<ThreatStreamApiException> MapAsync(HttpStatusCode status, HttpContent content, string? reason = null, Action<HttpResponseMessage>? configure = null)
	{
		using var response = new HttpResponseMessage(status) { Content = content, ReasonPhrase = reason };
		configure?.Invoke(response);
		var exception = await ThreatStreamErrorMapper.CreateAsync(response);
		return exception.Should().BeOfType<ThreatStreamApiException>().Subject;
	}

	private static Task<ThreatStreamApiException> MapAsync(string body, HttpStatusCode status = HttpStatusCode.BadRequest)
		=> MapAsync(status, new StringContent(body));

	[Fact]
	public async Task Success_IsNotAnError()
	{
		using var response = new HttpResponseMessage(HttpStatusCode.NoContent);

		(await ThreatStreamErrorMapper.CreateAsync(response)).Should().BeNull();
	}

	[Theory]
	[InlineData("""{"message":"Invalid observable value"}""", "Invalid observable value")]
	[InlineData("""{"error":"Rate limit exceeded"}""", "Rate limit exceeded")]
	[InlineData("""{"detail":"Not found."}""", "Not found.")]
	[InlineData("""{"reason":"Forbidden"}""", "Forbidden")]
	[InlineData("""{"error_message":"Sorry, this request could not be processed."}""", "Sorry, this request could not be processed.")]
	[InlineData("""{"message":"  ","error":" second "}""", "second")]
	[InlineData("""{"error":{"message":"nested"}}""", "nested")]
	[InlineData("""{"error":{"code":5},"detail":"after nested"}""", "after nested")]
	[InlineData("""{"error":7,"detail":"text wins"}""", "text wins")]
	[InlineData("Invalid API key", "Invalid API key")]
	[InlineData("  \"quoted\"  ", "\"quoted\"")]
	public async Task Message_IsReadTolerantly(string body, string expected)
	{
		var exception = await MapAsync(body, HttpStatusCode.Forbidden);

		exception.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		exception.Message.Should().Be(expected);
		exception.RetryAfter.Should().BeNull();
	}

	[Fact]
	public async Task LongText_IsTruncated()
	{
		var exception = await MapAsync(new string('x', 600));

		exception.Message.Should().Be(new string('x', ThreatStreamErrorMapper.MaxTextLength) + "...");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("[1,2]")]
	[InlineData("{not json")]
	[InlineData("{}")]
	[InlineData("""{"error":{"error":{"message":"too deep"}}}""")]
	[InlineData("""{"objects":[]}""")]
	[InlineData("<html><body>Bad gateway</body></html>")]
	public async Task NoReadableMessage_FallsBackToTheStatus(string body)
		=> (await MapAsync(body, HttpStatusCode.BadGateway)).Message.Should().Be("HTTP 502 (Bad Gateway)");

	[Theory]
	[InlineData(null, "HTTP 599 (599)")]
	[InlineData("Custom", "HTTP 599 (Custom)")]
	public async Task Fallback_NamesTheReasonPhraseOrStatus(string? reason, string expected)
		=> (await MapAsync((HttpStatusCode)599, new StringContent(""), reason)).Message.Should().Be(expected);

	[Fact]
	public async Task RetryAfter_IsCarried()
	{
		var exception = await MapAsync(HttpStatusCode.TooManyRequests, new StringContent(""), configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(60)));

		exception.RetryAfter.Should().Be(TimeSpan.FromSeconds(60));
	}

	[Fact]
	public async Task RetryAfter_AsAPastDate_IsZero()
	{
		var exception = await MapAsync(HttpStatusCode.TooManyRequests, new StringContent(""), configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(-1)));

		exception.RetryAfter.Should().Be(TimeSpan.Zero);
	}

	[Fact]
	public async Task RetryAfter_AsAFutureDate_IsTheWait()
	{
		var exception = await MapAsync(HttpStatusCode.TooManyRequests, new StringContent(""), configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(10)));

		exception.RetryAfter.Should().BeCloseTo(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1));
	}

	[Fact]
	public async Task UnsupportedCharset_FallsBackToTheStatus()
	{
		var content = new ByteArrayContent(Encoding.UTF8.GetBytes("""{"message":"x"}"""));
		content.Headers.TryAddWithoutValidation("Content-Type", "application/json; charset=no-such-charset");

		(await MapAsync(HttpStatusCode.InternalServerError, content)).Message.Should().Be("HTTP 500 (Internal Server Error)");
	}

	private sealed class FailingContent(Exception failure) : HttpContent
	{
		protected override Task SerializeToStreamAsync(Stream stream, System.Net.TransportContext? context)
		{
			ArgumentNullException.ThrowIfNull(stream);
			_ = context;
			throw failure;
		}

		protected override bool TryComputeLength(out long length)
		{
			length = 0;
			return false;
		}
	}

	public static TheoryData<Exception> ReadFailures => new()
	{
		new HttpRequestException("reset"),
		new IOException("broken pipe")
	};

	[Theory]
	[MemberData(nameof(ReadFailures))]
	public async Task BodyLostWhileReading_FallsBackToTheStatus(Exception failure)
		=> (await MapAsync(HttpStatusCode.ServiceUnavailable, new FailingContent(failure))).Message.Should().Be("HTTP 503 (Service Unavailable)");

	[Fact]
	public async Task OtherReadFailures_Propagate()
	{
		using var response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new FailingContent(new NotSupportedException("bug")) };

		var act = () => ThreatStreamErrorMapper.CreateAsync(response);

		await act.Should().ThrowAsync<NotSupportedException>();
	}

	[Fact]
	public Task Client_RaisesTheMappedException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Confidence.GetTrendAsync("x", ct),
			HttpStatusCode.NotFound,
			"""{"detail":"Not found."}""",
			"Not found.");
}
