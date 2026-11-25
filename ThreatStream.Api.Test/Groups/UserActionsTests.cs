using System.Net;
using ThreatStream.Api.Models.UserAudit;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAuditTestKit;

namespace ThreatStream.Api.Test.Groups;

public class UserActionsTests
{
	[Fact]
	public async Task StartReportAsync_SendsGetWithAsyncResult()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserActions.StartReportAsync(null, ct), JobJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/useraction/", "?async_result=true");
	}

	[Fact]
	public async Task StartReportAsync_SendsTheReferenceExample()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserActions.StartReportAsync(ExampleOptions(), ct), JobJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/useraction/", "?async_result=true&" + ExampleQuery);
	}

	[Fact]
	public async Task StartReportAsync_SendsSeveralUsersAndJson()
	{
		var options = new UserActivityReportOptions { UserIds = [56, 57], Format = UserActivityReportFormat.Json, Offset = 20 };

		var call = await TestClient.CaptureAsync((client, ct) => client.UserActions.StartReportAsync(options, ct), JobJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/useraction/", "?async_result=true&format=json&user_id__in=56%2C57&offset=20");
	}

	[Fact]
	public async Task StartReportAsync_MapsTheJob()
	{
		var job = await TestClient.ReadAsync((client, ct) => client.UserActions.StartReportAsync(ExampleOptions(), ct), JobJson);

		job.ShouldBeTheExampleJob();
	}

	[Fact]
	public async Task GetReportAsync_SendsGetWithoutAsyncResult()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserActions.GetReportAsync(ExampleOptions(), ct), ReportCsv);

		call.ShouldBe(HttpMethod.Get, "/api/v1/useraction/", "?" + ExampleQuery);
	}

	[Fact]
	public async Task GetReportAsync_ReturnsTheReportText()
	{
		var report = await TestClient.ReadAsync((client, ct) => client.UserActions.GetReportAsync(null, ct), ReportCsv);

		report.Should().Be(ReportCsv);
	}

	[Fact]
	public Task StartReportAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.UserActions.StartReportAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "Only organization administrators can audit users"}""",
			"Only organization administrators can audit users");
}
