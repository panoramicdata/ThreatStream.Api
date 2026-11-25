using System.Net;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public class ClientTests
{
	private const string Trend = """{"min_confidence":1}""";

	private static CancellationToken Ct => TestContext.Current.CancellationToken;

	private sealed class TrackingHandler : HttpMessageHandler
	{
		public bool Disposed { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Trend), RequestMessage = request });
		}

		protected override void Dispose(bool disposing)
		{
			Disposed = true;
			base.Dispose(disposing);
		}
	}

	[Fact]
	public void Constructors_RequireArguments()
	{
		var options = new ThreatStreamClientOptions { Username = "u", ApiKey = "k" };

		((Action)(() => _ = new ThreatStreamClient(null!))).Should().Throw<ArgumentNullException>();
		((Action)(() => _ = new ThreatStreamClient(null!, new StubHandler()))).Should().Throw<ArgumentNullException>();
		((Action)(() => _ = new ThreatStreamClient(options, null!))).Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Constructor_ValidatesOptions()
	{
		var act = () => new ThreatStreamClient(new ThreatStreamClientOptions { Username = "u" });

		act.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("ApiKey");
	}

	[Fact]
	public void Constructor_WithOwnTransport_CreatesAUsableClient()
	{
		using var client = new ThreatStreamClient(new ThreatStreamClientOptions { Region = ThreatStreamRegion.Eu, Username = "u", ApiKey = "k" });

		client.BaseAddress.Should().Be(new Uri("https://api-eu.threatstream.com/"));
		client.Confidence.Should().NotBeNull().And.BeSameAs(client.Confidence);
	}

	[Theory]
	[InlineData("https://ts.example.com", "https://ts.example.com/")]
	[InlineData("https://ts.example.com/", "https://ts.example.com/")]
	[InlineData("http://proxy.test/threatstream", "http://proxy.test/threatstream/")]
	public void BaseAddress_AlwaysEndsWithSlash(string baseUrl, string expected)
	{
		using var client = TestClient.Create(new StubHandler(), o => o.BaseUrl = baseUrl);

		client.BaseAddress.AbsoluteUri.Should().Be(expected);
	}

	[Fact]
	public async Task Requests_KeepThePathPrefix_AndCarryTheApiKey()
	{
		var stub = TestClient.Stub(Trend);
		using var client = TestClient.Create(stub, o => o.BaseUrl = "https://proxy.test/threatstream");

		await client.Confidence.GetTrendAsync("192.0.2.4", Ct);

		stub.Calls[0].Uri.AbsolutePath.Should().Be("/threatstream/api/v1/inteldetails/confidence_trend/");
		stub.Calls[0].Authorization.Should().Be(TestClient.ApiKeyAuthorization);
	}

	[Fact]
	public async Task Requests_NeverPutCredentialsInTheUrl()
	{
		var stub = TestClient.Stub(Trend);
		using var client = TestClient.Create(stub);

		await client.Confidence.GetTrendAsync("192.0.2.4", Ct);

		stub.Calls[0].Uri.ToString().Should().NotContain("fake-key").And.NotContain("analyst");
	}

	[Fact]
	public async Task DisposingTheClient_DisposesTheTransport()
	{
		var handler = new TrackingHandler();
		var client = TestClient.Create(handler);
		var confidence = client.Confidence;

		(await confidence.GetTrendAsync("x", Ct)).MinConfidence.Should().Be(1);
		client.Dispose();
		client.Dispose();
		var act = () => confidence.GetTrendAsync("x", Ct);

		handler.Disposed.Should().BeTrue();
		await act.Should().ThrowAsync<ObjectDisposedException>();
	}
}
