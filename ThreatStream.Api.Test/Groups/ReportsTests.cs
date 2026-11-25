using System.Net;
using ThreatStream.Api.Models.Reports;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class ReportsTests
{
	// The reference's example response for a sandbox report, with the echoed credentials anonymised.
	private const string GenerationJson = """
		{
			"id": 406721,
			"dateRequested": "2025-08-06T21:24:50.557Z",
			"params": {"api_key": "XXXXXXXX", "username": "analyst@example.com"},
			"sync": false,
			"printableUrl": "https://ui.threatstream.com/sandbox/report/1372328/printable",
			"callbackUrl": "https://optic.threatstream.com/api/v1/alfred/async/407262/",
			"updatedAt": "2025-08-06T21:24:50.562Z",
			"createdAt": "2025-08-06T21:24:50.557Z",
			"dateCompleted": null,
			"ClientAppId": 1,
			"async_report": 407262,
			"success": true
		}
		""";

	// The reference's example export job, with the signature shortened.
	private const string AsyncReportJson = """
		{
			"created_ts": "2025-08-06T21:24:50.526135",
			"id": 407262,
			"modified_ts": "2025-08-06T21:25:13.378704",
			"printable": "https://ui.threatstream.com/sandbox/report/1372328/printable",
			"r_id": 1372328,
			"report_type": "sandbox",
			"resource_uri": "",
			"s3_url": "https://ts-optic.s3.amazonaws.com/userUploads/2025-08-06/407262-sandbox.pdf",
			"signed_s3_url": "https://ts-optic.s3.amazonaws.com/userUploads/2025-08-06/407262-sandbox.pdf?X-Amz-Signature=a9f5",
			"succeed": true
		}
		""";

	private static ReportOptions Options() => new() { ResourceName = "sandbox", ResourceId = 1372328, Sync = false };

	[Fact]
	public async Task GenerateAsync_SendsGetWithTheResource()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Reports.GenerateAsync(Options(), ct), GenerationJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/alfred/report/", "?resource_name=sandbox&resource_id=1372328&sync=false");
	}

	[Fact]
	public async Task GenerateAsync_WithoutSync_LeavesItOut()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.Reports.GenerateAsync(new ReportOptions { ResourceName = "actor", ResourceId = 775054 }, ct),
			GenerationJson);

		call.Uri.Query.Should().Be("?resource_name=actor&resource_id=775054");
	}

	[Fact]
	public async Task GenerateAsync_MapsEveryField()
	{
		var report = await TestClient.ReadAsync((client, ct) => client.Reports.GenerateAsync(Options(), ct), GenerationJson);

		report.Id.Should().Be(406721);
		report.Success.Should().BeTrue();
		report.Sync.Should().BeFalse();
		report.PrintableUrl.Should().Be("https://ui.threatstream.com/sandbox/report/1372328/printable");
		report.CallbackUrl.Should().Be("https://optic.threatstream.com/api/v1/alfred/async/407262/");
		report.AsyncReport.Should().Be(407262);
		report.DateRequested.Should().Be(new DateTimeOffset(2025, 8, 6, 21, 24, 50, 557, TimeSpan.Zero));
		report.DateCompleted.Should().BeNull();
		report.CreatedAt.Should().Be(report.DateRequested);
		report.UpdatedAt.Should().Be(new DateTimeOffset(2025, 8, 6, 21, 24, 50, 562, TimeSpan.Zero));
		report.ClientAppId.Should().Be(1);
		report.AdditionalProperties.Should().ContainKey("params");
	}

	[Fact]
	public Task GenerateAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Reports.GenerateAsync(Options(), ct),
			HttpStatusCode.TooManyRequests,
			"""{"message": "Rate limit exceeded"}""",
			"Rate limit exceeded");

	[Fact]
	public async Task GetAsyncReportAsync_SendsGetToTheJob()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Reports.GetAsyncReportAsync(407262, ct), AsyncReportJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/alfred/async/407262/");
	}

	[Fact]
	public async Task GetAsyncReportAsync_MapsEveryField()
	{
		var job = await TestClient.ReadAsync((client, ct) => client.Reports.GetAsyncReportAsync(407262, ct), AsyncReportJson);

		job.Id.Should().Be(407262);
		job.ResourceUri.Should().BeEmpty();
		job.Succeed.Should().BeTrue();
		job.ReportType.Should().Be("sandbox");
		job.ResourceId.Should().Be(1372328);
		job.Printable.Should().Be("https://ui.threatstream.com/sandbox/report/1372328/printable");
		job.S3Url.Should().Be("https://ts-optic.s3.amazonaws.com/userUploads/2025-08-06/407262-sandbox.pdf");
		job.SignedS3Url.Should().Be("https://ts-optic.s3.amazonaws.com/userUploads/2025-08-06/407262-sandbox.pdf?X-Amz-Signature=a9f5");
		job.CreatedTs.Should().Be(new DateTimeOffset(2025, 8, 6, 21, 24, 50, TimeSpan.Zero).AddTicks(5261350));
		job.ModifiedTs.Should().Be(new DateTimeOffset(2025, 8, 6, 21, 25, 13, TimeSpan.Zero).AddTicks(3787040));
	}

	[Fact]
	public async Task GetAsyncReportAsync_InProgress_ReadsSucceedAsNull()
	{
		var job = await TestClient.ReadAsync(
			(client, ct) => client.Reports.GetAsyncReportAsync(407262, ct),
			"""{"id": 407262, "succeed": null, "report_type": "actor"}""");

		job.Succeed.Should().BeNull();
		job.SignedS3Url.Should().BeNull();
	}
}
