using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableTags;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAuditTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class AsyncResultsTests
{
	private static string Job(string? status, int? responseStatus = null)
	{
		var statusJson = status is null ? "null" : "\"" + status + "\"";
		var responseStatusJson = responseStatus?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "null";
		return "{\"id\": 4242, \"status\": " + statusJson + ", \"response_status\": " + responseStatusJson + "}";
	}

	[Fact]
	public async Task RunAsyncJobAsync_PollsUntilDone_ThenDownloads()
	{
		var stub = Responses(JobJson, Job("inprogress"), Job(null), DoneJobJson, ReportCsv);
		using var client = TestClient.Create(stub);

		var report = await client.RunAsyncJobAsync(ct => client.UserActions.StartReportAsync(ExampleOptions(), ct), TimeSpan.Zero, TestContext.Current.CancellationToken);

		report.Should().Be(ReportCsv);
		stub.Calls.Select(c => c.Uri.PathAndQuery).Should().Equal(
			"/api/v1/useraction/?async_result=true&" + ExampleQuery,
			"/api/v1/generic_async_result/4242/",
			"/api/v1/generic_async_result/4242/",
			"/api/v1/generic_async_result/4242/",
			"/api/v1/generic_async_result/4242/get_result/");
	}

	[Fact]
	public async Task RunAsyncJobAsync_DrivesABulkTaggingJob()
	{
		var request = new ObservableTagQueryRequest { Query = new ObservableTagQuery { Q = "itype = mal_ip" } };
		var stub = Responses(Job("inprogress"), Job("done", 200), "{}");
		using var client = TestClient.Create(stub);

		var result = await client.RunAsyncJobAsync(ct => client.ObservableTags.BulkTagMatchingAsync(request, ct), TimeSpan.Zero, TestContext.Current.CancellationToken);

		result.Should().Be("{}");
		stub.Calls.Select(c => (c.Method, c.Uri.PathAndQuery)).Should().Equal(
			(HttpMethod.Post, "/api/v2/intelligence/bulk_tagging/?async_result=true"),
			(HttpMethod.Get, "/api/v1/generic_async_result/4242/"),
			(HttpMethod.Get, "/api/v1/generic_async_result/4242/get_result/"));
	}

	[Theory]
	[InlineData(HttpStatusCode.Accepted)]
	[InlineData(HttpStatusCode.NoContent)]
	public async Task RunAsyncJobAsync_NoJobStarted_Throws(HttpStatusCode status)
	{
		var stub = new StubHandler();
		stub.Enqueue(status, string.Empty);
		using var client = TestClient.Create(stub);

		var act = () => client.RunAsyncJobAsync(ct => client.ApiUserActions.StartReportAsync(null, ct), TimeSpan.Zero, TestContext.Current.CancellationToken);

		(await act.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().Contain("no asynchronous job was started");
		stub.Calls.Should().ContainSingle();
	}

	[Fact]
	public async Task RunAsyncJobAsync_AlreadyDone_DownloadsAtOnce()
	{
		var stub = Responses(Job(" DONE ", 200), ReportCsv);
		using var client = TestClient.Create(stub);

		var report = await client.RunAsyncJobAsync(ct => client.ApiUserActions.StartReportAsync(null, ct), TimeSpan.Zero, TestContext.Current.CancellationToken);

		report.Should().Be(ReportCsv);
		stub.Calls.Should().HaveCount(2);
	}

	[Theory]
	[InlineData("failed", null)]
	[InlineData("Error", null)]
	[InlineData("done", 500)]
	[InlineData("inprogress", 400)]
	public async Task RunAsyncJobAsync_FailedJob_Throws(string status, int? responseStatus)
	{
		var stub = Responses(JobJson, Job(status, responseStatus));
		using var client = TestClient.Create(stub);

		var act = () => client.RunAsyncJobAsync(ct => client.UserActions.StartReportAsync(null, ct), TimeSpan.Zero, TestContext.Current.CancellationToken);

		(await act.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().StartWith("ThreatStream asynchronous job 4242 failed");
		stub.Calls.Should().HaveCount(2);
	}

	[Fact]
	public async Task RunAsyncJobAsync_Cancelled_StopsPolling()
	{
		using var cancellation = new CancellationTokenSource();
		var stub = Responses(JobJson);
		using var client = TestClient.Create(stub);

		var act = () => client.RunAsyncJobAsync(
			async ct =>
			{
				var job = await client.UserActions.StartReportAsync(null, ct);
				await cancellation.CancelAsync();
				return job;
			},
			TimeSpan.FromMinutes(1),
			cancellation.Token);

		await act.Should().ThrowAsync<OperationCanceledException>();
		stub.Calls.Should().ContainSingle();
	}

	[Fact]
	public async Task RunAsyncJobAsync_RejectsBadArguments()
	{
		using var client = TestClient.Create(new StubHandler());
		Task<GenericAsyncResult?> start(CancellationToken ct) => client.UserActions.StartReportAsync(null, ct);

		var noStart = () => client.RunAsyncJobAsync(null!, TimeSpan.Zero, TestContext.Current.CancellationToken);
		var negative = () => client.RunAsyncJobAsync(start, TimeSpan.FromSeconds(-1), TestContext.Current.CancellationToken);

		await noStart.Should().ThrowAsync<ArgumentNullException>();
		await negative.Should().ThrowAsync<ArgumentOutOfRangeException>();
	}
}
