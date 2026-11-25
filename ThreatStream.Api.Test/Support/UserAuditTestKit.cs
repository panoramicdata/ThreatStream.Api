using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UserAudit;

namespace ThreatStream.Api.Test.Support;

/// <summary>Responses and assertions shared by the user audit tests.</summary>
internal static class UserAuditTestKit
{
	/// <summary>The job in the reference's example (User Audit), just started.</summary>
	public const string JobJson = """
		{
			"created_ts": "2020-08-07T21:05:11.501490",
			"id": 4242,
			"modified_ts": "2020-08-07T21:05:11.501505",
			"path": "/api/v1/useraction/",
			"resource_uri": "/api/v1/generic_async_result/4242/",
			"response_status": null,
			"status": "inprogress"
		}
		""";

	/// <summary>The same job, finished.</summary>
	public const string DoneJobJson = """
		{
			"created_ts": "2020-08-07T21:05:11.501490",
			"id": 4242,
			"modified_ts": "2020-08-07T21:07:00",
			"path": "/api/v1/useraction/",
			"resource_uri": "/api/v1/generic_async_result/4242/",
			"response_status": 200,
			"status": "done"
		}
		""";

	/// <summary>A small CSV user activity report.</summary>
	public const string ReportCsv = "created_ts,user,action\r\n2019-10-09T10:00:00,analyst@example.com,login\r\n";

	/// <summary>The query the reference's example sends for one user's report, after <c>async_result</c>.</summary>
	public const string ExampleQuery = "created_ts__gte=2019-10-09T00%3A00%3A00Z&created_ts__lte=2019-10-10T23%3A59%3A59.999Z&format=csv&user_id__in=56&limit=1000&order_by=-created_ts";

	/// <summary>The options of the reference's example: one user, two days, CSV, up to 1000 records, newest first.</summary>
	public static UserActivityReportOptions ExampleOptions() => new()
	{
		CreatedFrom = new DateTimeOffset(2019, 10, 9, 0, 0, 0, TimeSpan.Zero),
		CreatedUntil = new DateTimeOffset(2019, 10, 10, 23, 59, 59, 999, TimeSpan.Zero),
		Format = UserActivityReportFormat.Csv,
		Limit = 1000,
		OrderBy = "-created_ts",
		UserIds = [56]
	};

	/// <summary>A stub answering the requests in turn with these bodies, each with status 200.</summary>
	public static StubHandler Responses(params string[] responses)
	{
		var stub = new StubHandler();
		foreach (var response in responses)
		{
			stub.Enqueue(HttpStatusCode.OK, response);
		}

		return stub;
	}

	/// <summary>Asserts every modelled field of <see cref="JobJson"/>.</summary>
	public static void ShouldBeTheExampleJob(this GenericAsyncResult? job)
	{
		job.Should().NotBeNull();
		job!.Id.Should().Be(4242);
		job.ResourceUri.Should().Be("/api/v1/generic_async_result/4242/");
		job.Status.Should().Be("inprogress");
		job.ResponseStatus.Should().BeNull();
		job.Path.Should().Be("/api/v1/useraction/");
		job.CreatedTs.Should().Be(new DateTimeOffset(2020, 8, 7, 21, 5, 11, TimeSpan.Zero).AddTicks(5014900));
		job.ModifiedTs.Should().Be(new DateTimeOffset(2020, 8, 7, 21, 5, 11, TimeSpan.Zero).AddTicks(5015050));
	}
}
