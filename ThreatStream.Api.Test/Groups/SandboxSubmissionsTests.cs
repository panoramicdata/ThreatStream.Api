using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Sandbox;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class SandboxSubmissionsTests
{
	// The reference's example response.
	private const string SubmissionJson = """
		{
			"success": true,
			"reports": {
				"WINDOWS7": {"status": "/api/v1/submit/188/", "detail": "/api/v1/submit/188/report/"}
			}
		}
		""";

	// The reference documents no response attributes for search and status; these are the fields a search matches.
	private const string ReportJson = """
		{
			"id": 188,
			"resource_uri": "/api/v1/submit/188/",
			"vendor": "Joe Sandbox",
			"platform": "WINDOWS7",
			"status": "done",
			"result": {"verdict": "malicious"},
			"notes": "archive_detonation_2025-02-15"
		}
		""";

	private static SandboxSubmitRequest UrlRequest() => new()
	{
		Url = "http://preppybeach.example/wp-includes/images/ipad/",
		Platform = "WINDOWS7",
		Classification = "private",
		UseAnomaliSandbox = true
	};

	[Fact]
	public async Task SubmitAsync_SendsMultipartWithEveryField()
	{
		var request = new SandboxSubmitRequest
		{
			Url = "http://preppybeach.example/",
			File = new ThreatStreamFile("archive.zip", new byte[] { 0x50, 0x4b }, "application/zip"),
			Platform = "WINDOWS10x64",
			Classification = "private",
			UseAnomaliSandbox = false,
			UsePremiumSandbox = true,
			UsePolyswarmSandbox = false,
			UseVmraySandbox = true,
			VmrayMaxJobs = 3,
			PolyswarmScan = false,
			IsQrCode = true,
			FileHasPassword = true,
			FilePassword = "infected",
			ImportIndicators = false,
			Notes = ["Credential-Exposure", "compromised_email"],
			Circles = [13, 14],
			AdditionalFields = { ["extra"] = "x" }
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.SandboxSubmissions.SubmitAsync(request, ct), SubmissionJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/submit/new/");
		PartsOf(call).Should().Equal(
			Field("url", "http://preppybeach.example/"),
			("file", "archive.zip", "application/zip", "PK"),
			Field("platform", "WINDOWS10x64"),
			Field("classification", "private"),
			Field("use_anomali_sandbox", "false"),
			Field("use_premium_sandbox", "true"),
			Field("use_polyswarm_sandbox", "false"),
			Field("use_vmray_sandbox", "true"),
			Field("vmray_max_jobs", "3"),
			Field("polyswarm_scan", "false"),
			Field("is_qrcode", "true"),
			Field("file_has_password", "true"),
			Field("file_password", "infected"),
			Field("import_indicators", "false"),
			Field("notes", "Credential-Exposure,compromised_email"),
			Field("circles", "13,14"),
			Field("extra", "x"));
	}

	[Fact]
	public async Task SubmitAsync_Url_SendsTheDocumentedFields()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SandboxSubmissions.SubmitAsync(UrlRequest(), ct), SubmissionJson);

		PartsOf(call).Should().Equal(
			Field("url", "http://preppybeach.example/wp-includes/images/ipad/"),
			Field("platform", "WINDOWS7"),
			Field("classification", "private"),
			Field("use_anomali_sandbox", "true"));
	}

	[Fact]
	public async Task SubmitAsync_MapsTheReportLinks()
	{
		var submission = await TestClient.ReadAsync((client, ct) => client.SandboxSubmissions.SubmitAsync(UrlRequest(), ct), SubmissionJson);

		submission.Success.Should().BeTrue();
		var links = submission.Reports.Should().ContainSingle().Which;
		links.Key.Should().Be("WINDOWS7");
		links.Value.Status.Should().Be("/api/v1/submit/188/");
		links.Value.Detail.Should().Be("/api/v1/submit/188/report/");
		links.Value.Id.Should().Be(188);
	}

	[Theory]
	[InlineData(null, null)]
	[InlineData("/api/v1/submit/abc/", null)]
	[InlineData("/api/v1/other/188/", null)]
	[InlineData("/api/v1/submit/", null)]
	[InlineData("https://api.threatstream.com/api/v1/submit/42/", 42L)]
	public void SandboxReportLinks_Id_IsReadFromTheStatusPath(string? status, long? expected)
		=> new SandboxReportLinks { Status = status }.Id.Should().Be(expected);

	[Fact]
	public Task SubmitAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.SandboxSubmissions.SubmitAsync(UrlRequest(), ct), "POST", "api/v1/submit/new/");

	[Fact]
	public Task SubmitAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.SandboxSubmissions.SubmitAsync(UrlRequest(), ct),
			HttpStatusCode.BadRequest,
			"""{"message": "No active sandbox integration"}""",
			"No active sandbox integration");

	[Fact]
	public async Task SearchAsync_SendsGetWithTheKeyword()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.SandboxSubmissions.SearchAsync(new SandboxSearchOptions { Query = "archive_detonation_2025-02-15", Limit = 50 }, ct),
			EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/submit/search/", "?q=archive_detonation_2025-02-15&limit=50");
	}

	[Fact]
	public async Task SearchAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.SandboxSubmissions.SearchAsync(null, ct), $$"""{"meta":{},"objects":[{{ReportJson}}]}""");

		ShouldBeTheExampleReport(page.Objects.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetToTheDetonation()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SandboxSubmissions.GetAsync(188, ct), ReportJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/submit/188/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
		=> ShouldBeTheExampleReport(await TestClient.ReadAsync((client, ct) => client.SandboxSubmissions.GetAsync(188, ct), ReportJson));

	[Fact]
	public async Task GetReportAsync_SendsGetToTheReport()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SandboxSubmissions.GetReportAsync(188, ct), "{}");

		call.ShouldBe(HttpMethod.Get, "/api/v1/submit/188/report/");
	}

	[Fact]
	public async Task GetReportAsync_KeepsEveryProperty()
	{
		var report = await TestClient.ReadAsync(
			(client, ct) => client.SandboxSubmissions.GetReportAsync(188, ct),
			"""{"verdict": "malicious", "signatures": [{"name": "Drops PE files"}]}""");

		report.AdditionalProperties["verdict"].GetString().Should().Be("malicious");
		report.AdditionalProperties["signatures"].GetArrayLength().Should().Be(1);
	}

	[Fact]
	public Task GetReportAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.SandboxSubmissions.GetReportAsync(188, ct),
			HttpStatusCode.NotFound,
			"""{"message": "Not found"}""",
			"Not found");

	private static void ShouldBeTheExampleReport(SandboxReport report)
	{
		report.Id.Should().Be(188);
		report.ResourceUri.Should().Be("/api/v1/submit/188/");
		report.Vendor.Should().Be("Joe Sandbox");
		report.Platform.Should().Be("WINDOWS7");
		report.Status.Should().Be("done");
		report.AdditionalProperties.Keys.Should().BeEquivalentTo("result", "notes");
	}
}
