using System.Net;
using ThreatStream.Api.Handlers;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public class ReadOnlyHandlerTests
{
	private const string Base = "https://threatstream.test/prefix/";

	private static async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path)
	{
		using var harness = new HandlerHarness(new ReadOnlyHandler(new Uri(Base)));
		harness.Stub.Enqueue(HttpStatusCode.OK);
		return await harness.SendAsync(method, Base + path);
	}

	[Theory]
	[InlineData("GET", "api/v2/intelligence/")]
	[InlineData("HEAD", "api/v1/feed/")]
	[InlineData("POST", "oauth/token")]
	[InlineData("POST", "oauth/token/")]
	[InlineData("POST", "api/v2/intelligence/")]
	[InlineData("POST", "api/v2/intelligence")]
	[InlineData("POST", "api/v2/intelligence/?limit=0")]
	[InlineData("POST", "api/v1/threat_model_search/")]
	[InlineData("POST", "api/v1/integration_package/direct/")]
	public async Task Allows_ReadOnlyOperations(string method, string path)
	{
		using var response = await SendAsync(new HttpMethod(method), path);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Theory]
	[InlineData("GET", "api/v1/alfred/report/?resource_name=actor")]
	[InlineData("GET", "api/v1/alfred/report?resource_name=actor")]
	[InlineData("GET", "api/v1/useraction/?async_result=true")]
	[InlineData("PATCH", "api/v2/intelligence/")]
	[InlineData("DELETE", "api/v2/intelligence/")]
	[InlineData("PUT", "api/v2/intelligence/")]
	[InlineData("POST", "api/v2/intelligence/import/")]
	[InlineData("POST", "api/v2/intelligence/bulk_tagging/")]
	[InlineData("POST", "api/v2/intelligence/bulk_workgroups/")]
	[InlineData("POST", "api/v2/intelligence/123/clone/")]
	[InlineData("POST", "api/v1/intelligence/")]
	[InlineData("POST", "api/v1/threat_model_search/x/")]
	[InlineData("POST", "api/v1/integration_package/direct/ip/1.2.3.4/")]
	[InlineData("POST", "api/v1/snapshot/")]
	[InlineData("POST", "api/v1/submit/new/")]
	[InlineData("POST", "api/v1/orgadmin/")]
	[InlineData("PATCH", "api/v1/orgadmin/5/")]
	[InlineData("DELETE", "api/v1/rule/?id__in=1")]
	[InlineData("POST", "API/V2/Intelligence/")]
	[InlineData("POST", "oauth/token/x")]
	[InlineData("POST", "api/v2/intelligence/../../api/v1/orgadmin/")]
	public async Task Refuses_EverythingElse(string method, string path)
	{
		var act = () => SendAsync(new HttpMethod(method), path);

		var thrown = await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		thrown.Which.Method.Should().Be(method);
		thrown.Which.Path.Should().NotContain("?");
	}

	[Theory]
	[InlineData("https://threatstream.test/api/v2/intelligence/")]
	[InlineData("https://other.test/prefix/api/v2/intelligence/")]
	[InlineData("http://threatstream.test/prefix/api/v2/intelligence/")]
	public async Task Refuses_RequestsOutsideTheBaseAddress(string url)
	{
		using var harness = new HandlerHarness(new ReadOnlyHandler(new Uri(Base)));

		var act = () => harness.SendAsync(HttpMethod.Post, url + "?q=x");

		var thrown = await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		thrown.Which.Path.Should().Be(url);
		thrown.Which.Message.Should().Be($"The ThreatStream client is read-only and refused POST {url}.");
		harness.Stub.Calls.Should().BeEmpty();
	}

	[Fact]
	public async Task Allows_AGetOutsideTheBaseAddress()
	{
		using var harness = new HandlerHarness(new ReadOnlyHandler(new Uri(Base)));
		harness.Stub.Enqueue(HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get, "https://other.test/api/v1/alfred/report/");

		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task Client_IsReadOnly_RefusesBeforeSending()
	{
		var stub = new StubHandler();
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);
		var probe = client.For<IProbe>();

		var act = () => probe.PatchAsync("api/v2/intelligence/", new Dictionary<string, string?>(), TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		thrown.Which.Path.Should().Be("https://threatstream.test/api/v2/intelligence/");
		stub.Calls.Should().BeEmpty();
	}

	[Fact]
	public async Task Client_IsReadOnly_AllowsALongIntelligenceQuery()
	{
		var stub = TestClient.Stub("""{"meta":{},"objects":[]}""");
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		await client.For<IProbe>().PostAsync("api/v2/intelligence/", new Dictionary<string, string?> { ["q"] = new string('a', 5000) }, TestContext.Current.CancellationToken);

		stub.Calls.Should().ContainSingle().Which.Method.Should().Be(HttpMethod.Post);
	}

	[Fact]
	public async Task Client_IsReadOnly_StillObtainsOAuthTokens()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, TestClient.TokenJson("t"));
		stub.Enqueue(HttpStatusCode.OK, """{"min_confidence":1}""");
		using var client = TestClient.Create(stub, o =>
		{
			TestClient.UseOAuth(o);
			o.IsReadOnly = true;
		});

		await client.Confidence.GetTrendAsync("x", TestContext.Current.CancellationToken);

		stub.Calls.Select(c => c.Uri.AbsolutePath).Should().Equal("/oauth/token", "/api/v1/inteldetails/confidence_trend/");
	}

	[Fact]
	public async Task Client_NotReadOnly_SendsWrites()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK);
		using var client = TestClient.Create(stub);

		await client.For<IProbe>().DeleteAsync("api/v1/rule/5/", TestContext.Current.CancellationToken);

		stub.Calls[0].Method.Should().Be(HttpMethod.Delete);
	}
}
