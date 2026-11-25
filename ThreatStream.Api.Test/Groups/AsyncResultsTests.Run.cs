using ThreatStream.Api.Models.UserAudit;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAuditTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class AsyncResultsTests
{
	private static string Job(string? status, int? responseStatus = null)
		=> $$"""{"id": 4242, "status": {{(status is null ? "null" : $"\"{status}\"")}}, "response_status": {{(responseStatus?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "null")}}}""";

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
		Task<GenericAsyncResult> start(CancellationToken ct) => client.AsyncResults.GetAsync(1, ct);

		var noStart = () => client.RunAsyncJobAsync(null!, TimeSpan.Zero, TestContext.Current.CancellationToken);
		var negative = () => client.RunAsyncJobAsync(start, TimeSpan.FromSeconds(-1), TestContext.Current.CancellationToken);

		await noStart.Should().ThrowAsync<ArgumentNullException>();
		await negative.Should().ThrowAsync<ArgumentOutOfRangeException>();
	}
}
