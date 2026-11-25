using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class ImportSessionsTests
{
	// The attributes the ThreatStream API reference documents for a finished import session, plus an unmodelled one.
	private const string SessionJson = """
		{
			"id": 482,
			"resource_uri": "/api/v1/importsession/482/",
			"status": "done",
			"numIndicators": "12",
			"numRejected": 3,
			"name": "unit test import"
		}
		""";

	[Fact]
	public async Task GetAsync_SendsGetToTheSession()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ImportSessions.GetAsync(482, ct), SessionJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/importsession/482/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var session = await TestClient.ReadAsync((client, ct) => client.ImportSessions.GetAsync(482, ct), SessionJson);

		session.Id.Should().Be(482);
		session.ResourceUri.Should().Be("/api/v1/importsession/482/");
		session.Status.Should().Be("done");
		session.NumIndicators.Should().Be(12);
		session.NumRejected.Should().Be(3);
		session.AdditionalProperties["name"].GetString().Should().Be("unit test import");
	}

	[Fact]
	public Task GetAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ImportSessions.GetAsync(1, ct),
			HttpStatusCode.NotFound,
			"""{"message": "Not found"}""",
			"Not found");

	[Fact]
	public async Task ListIntelligenceAsync_SendsGetWithPaging()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.ImportSessions.ListIntelligenceAsync(482, new ListOptions { Limit = 100, Offset = 100 }, ct),
			ObservablePage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/importsession/482/intelligence/", "?limit=100&offset=100");
	}

	[Fact]
	public async Task ListIntelligenceAsync_MapsTheObservables()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.ImportSessions.ListIntelligenceAsync(482, null, ct), ObservablePage);

		ShouldBeTheExampleObservable(page.Objects.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task ApproveAllAsync_SendsPatchWithoutABody()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ImportSessions.ApproveAllAsync(482, ct), string.Empty);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/importsession/482/approve_all/");
	}

	[Fact]
	public Task ApproveAllAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ImportSessions.ApproveAllAsync(482, ct), "PATCH", "api/v1/importsession/482/approve_all/");

	[Fact]
	public async Task ApproveSelectedAsync_SendsPatchWithTheIds()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ImportSessions.ApproveSelectedAsync(482, [11, 12, 13], ct), string.Empty);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/importsession/482/approve_selected/", "?ids=11%2C12%2C13");
	}

	[Fact]
	public Task ApproveSelectedAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ImportSessions.ApproveSelectedAsync(482, [11], ct), "PATCH", "api/v1/importsession/482/approve_selected/");

	[Fact]
	public Task ApproveSelectedAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ImportSessions.ApproveSelectedAsync(482, [11], ct),
			HttpStatusCode.Forbidden,
			"""{"message": "You need the Approve Intelligence permission"}""",
			"You need the Approve Intelligence permission");
}
