using System.Net;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAuditTestKit;

namespace ThreatStream.Api.Test.Groups;

public class ApiUserActionsTests
{
	[Fact]
	public async Task StartReportAsync_SendsTheReferenceExample()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ApiUserActions.StartReportAsync(ExampleOptions(), ct), JobJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/apiuseraction/", "?async_result=true&" + ExampleQuery);
	}

	[Fact]
	public async Task StartReportAsync_WithoutOptions_SendsOnlyAsyncResult()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ApiUserActions.StartReportAsync(null, ct), JobJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/apiuseraction/", "?async_result=true");
	}

	[Fact]
	public async Task StartReportAsync_MapsTheJob()
	{
		var job = await TestClient.ReadAsync((client, ct) => client.ApiUserActions.StartReportAsync(null, ct), JobJson);

		job.ShouldBeTheExampleJob();
	}

	[Fact]
	public async Task GetReportAsync_SendsGetWithoutAsyncResult()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ApiUserActions.GetReportAsync(null, ct), ReportCsv);

		call.ShouldBe(HttpMethod.Get, "/api/v1/apiuseraction/");
	}

	[Fact]
	public async Task GetReportAsync_ReturnsTheReportText()
	{
		var report = await TestClient.ReadAsync((client, ct) => client.ApiUserActions.GetReportAsync(ExampleOptions(), ct), ReportCsv);

		report.Should().Be(ReportCsv);
	}

	[Fact]
	public Task GetReportAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ApiUserActions.GetReportAsync(null, ct),
			HttpStatusCode.TooManyRequests,
			"""{"detail": "Request was throttled"}""",
			"Request was throttled");
}
