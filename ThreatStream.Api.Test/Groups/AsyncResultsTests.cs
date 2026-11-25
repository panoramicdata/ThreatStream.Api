using System.Net;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAuditTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class AsyncResultsTests
{
	[Fact]
	public async Task GetAsync_SendsGetForTheJob()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AsyncResults.GetAsync(4242, ct), JobJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/generic_async_result/4242/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var job = await TestClient.ReadAsync((client, ct) => client.AsyncResults.GetAsync(4242, ct), JobJson);

		job.ShouldBeTheExampleJob();
	}

	[Fact]
	public async Task GetAsync_FinishedJob_HasResponseStatus()
	{
		var job = await TestClient.ReadAsync((client, ct) => client.AsyncResults.GetAsync(4242, ct), DoneJobJson);

		job.Status.Should().Be("done");
		job.ResponseStatus.Should().Be(200);
		job.ModifiedTs.Should().Be(new DateTimeOffset(2020, 8, 7, 21, 7, 0, TimeSpan.Zero));
	}

	[Fact]
	public async Task GetResultAsync_SendsGetForTheResult()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AsyncResults.GetResultAsync(4242, ct), ReportCsv);

		call.ShouldBe(HttpMethod.Get, "/api/v1/generic_async_result/4242/get_result/");
	}

	[Fact]
	public async Task GetResultAsync_ReturnsTheResultText()
	{
		const string json = """[{"created_ts": "2019-10-09T10:00:00", "action": "login"}]""";

		var result = await TestClient.ReadAsync((client, ct) => client.AsyncResults.GetResultAsync(4242, ct), json);

		result.Should().Be(json);
	}

	[Fact]
	public Task GetResultAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.AsyncResults.GetResultAsync(4242, ct),
			HttpStatusCode.NotFound,
			"""{"message": "Not found"}""",
			"Not found");
}
