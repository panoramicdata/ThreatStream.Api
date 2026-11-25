using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableImports;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class ObservableImportsTests
{
	private static readonly DateTimeOffset Expiry = new(2017, 1, 26, 0, 0, 0, TimeSpan.Zero);

	private static readonly ObservableImportRequest EveryField = new()
	{
		File = new ThreatStreamFile("iocs.csv", "value,itype,tags\n192.0.2.4,brute_ip,phishing\n"u8.ToArray(), "text/csv"),
		DataText = "192.0.2.4",
		Url = "https://feed.example/list.txt",
		DataType = "structured",
		ThreatType = "malware",
		Classification = "private",
		Confidence = 70,
		SourceConfidenceWeight = 100,
		DefaultState = "active",
		ExpirationTs = Expiry,
		Severity = "high",
		Tags = [new ThreatStreamTag { Name = "private_tag", Tlp = Tlp.Red }],
		Tlp = Tlp.Green,
		TrustedCircles = [1, 2],
		RejectBenign = false,
		IsAnonymous = true,
		IntelligenceSource = "unit test",
		RemoteApi = true,
		Actors = [77],
		Campaigns = [78],
		Incidents = [79],
		Malware = [80],
		ThreatBulletins = [81],
		Ttps = [82],
		SourceLocations = ["europe"],
		TargetLocations = ["NL", "GB"],
		TargetIndustry = ["financial-services"],
		DomainMapping = "mal_domain",
		EmailMapping = "mal_email",
		IpMapping = "mal_ip",
		Md5Mapping = "mal_md5",
		UrlMapping = "mal_url",
		AdditionalFields = { ["notes"] = "from a test" }
	};

	[Fact]
	public async Task ImportAsync_SendsMultipartWithEveryField()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableImports.ImportAsync(EveryField, ct), ImportJobJson);

		call.ShouldBe(HttpMethod.Post, "/api/v2/intelligence/import/");
		PartsOf(call).Should().Equal(
			("file", "iocs.csv", "text/csv", "value,itype,tags\n192.0.2.4,brute_ip,phishing\n"),
			Field("datatext", "192.0.2.4"),
			Field("url", "https://feed.example/list.txt"),
			Field("data_type", "structured"),
			Field("threat_type", "malware"),
			Field("classification", "private"),
			Field("confidence", "70"),
			Field("source_confidence_weight", "100"),
			Field("default_state", "active"),
			Field("expiration_ts", "2017-01-26T00:00:00Z"),
			Field("severity", "high"),
			Field("tags", """[{"name":"private_tag","tlp":"red"}]"""),
			Field("tlp", "green"),
			Field("trustedcircles", "1,2"),
			Field("reject_benign", "false"),
			Field("is_anonymous", "true"),
			Field("intelligence_source", "unit test"),
			Field("remote_api", "true"),
			Field("actors", "77"),
			Field("campaigns", "78"),
			Field("tipreports", "81"),
			Field("incidents", "79"),
			Field("malware", "80"),
			Field("ttps", "82"),
			Field("source_locations", "europe"),
			Field("target_locations", "NL,GB"),
			Field("target_industry", "financial-services"),
			Field("domain_mapping", "mal_domain"),
			Field("email_mapping", "mal_email"),
			Field("ip_mapping", "mal_ip"),
			Field("md5_mapping", "mal_md5"),
			Field("url_mapping", "mal_url"),
			Field("notes", "from a test"));
	}

	[Fact]
	public async Task ImportAsync_SendsOnlyTheFieldsSet()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.ObservableImports.ImportAsync(new ObservableImportRequest { DataText = "bad.example", ThreatType = "apt", Confidence = 70 }, ct),
			ImportJobJson);

		PartsOf(call).Should().Equal(Field("datatext", "bad.example"), Field("threat_type", "apt"), Field("confidence", "70"));
	}

	[Fact]
	public async Task ImportAsync_MapsTheImportJob()
	{
		var job = (await TestClient.ReadAsync((client, ct) => client.ObservableImports.ImportAsync(new ObservableImportRequest { DataText = "x" }, ct), ImportJobJson))!;

		job.JobId.Should().Be("6194c7c0-53ae-41fc-a6f0-dc3d174f02e2");
		job.Success.Should().BeTrue();
		job.ImportSessionId.Should().Be(482);
	}

	[Fact]
	public Task ImportAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.ObservableImports.ImportAsync(new ObservableImportRequest { DataText = "x" }, ct),
			"POST",
			"api/v2/intelligence/import/");

	[Fact]
	public Task ImportAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ObservableImports.ImportAsync(new ObservableImportRequest { DataText = "x" }, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "No threat_type or mappings"}""",
			"No threat_type or mappings");

	[Fact]
	public async Task ImportWithoutApprovalAsync_SendsPatchWithMetaAndObjects()
	{
		var request = new ObservableBulkImportRequest
		{
			Meta = new ObservableImportMeta
			{
				AllowUnresolved = true,
				IgnoreWhitelist = false,
				IgnoreUrlHostWhitelists = true,
				Classification = "private",
				Confidence = 60,
				SourceConfidenceWeight = 100,
				ExpirationTs = Expiry,
				Severity = "high",
				SourceFeedId = 5,
				Tags = [new ThreatStreamTag { Name = "public_tag" }],
				TrustedCircles = [13],
				SourceLocations = ["europe"],
				TargetLocations = ["US-FL"],
				TargetIndustry = ["government-regional"],
				RemoteApi = true
			},
			Objects =
			[
				new ObservableImportObject { SourceIp = "192.0.2.4", Itype = "bot_ip", Severity = "high" },
				new ObservableImportObject { Domain = "idfsdszqylwjzq.biz", Itype = "mal_domain" },
				new ObservableImportObject { Url = "http://malicious.example/a.exe", Itype = "mal_url" },
				new ObservableImportObject { Email = "email@domain.example", Itype = "compromised_email", Confidence = 50 },
				new ObservableImportObject { Md5 = "1d37556b8aeb5cb5fbf08cd5b4790075", Itype = "apt_md5", ExpirationTs = Expiry },
				new ObservableImportObject { Value = "203.0.113.198", Itype = "mal_ip" }
			]
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableImports.ImportWithoutApprovalAsync(request, ct), string.Empty);

		call.ShouldBe(HttpMethod.Patch, "/api/v2/intelligence/", body: BulkImportBody);
	}

	[Fact]
	public async Task ImportWithoutApprovalAsync_MinimalObservable_SendsOnlyObjects()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.ObservableImports.ImportWithoutApprovalAsync(new ObservableBulkImportRequest { Objects = [new ObservableImportObject { SourceIp = "203.0.113.134", Itype = "bot_ip" }] }, ct),
			string.Empty);

		call.ShouldBe(HttpMethod.Patch, "/api/v2/intelligence/", body: """{"objects":[{"itype":"bot_ip","srcip":"203.0.113.134"}]}""");
	}

	[Fact]
	public Task ImportWithoutApprovalAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.ObservableImports.ImportWithoutApprovalAsync(new ObservableBulkImportRequest { Objects = [] }, ct),
			"PATCH",
			"api/v2/intelligence/");

	private const string BulkImportBody = "{\"meta\":{\"allow_unresolved\":true,\"ignore_whitelist\":false,\"ignore_url_host_whitelists\":true,"
		+ "\"classification\":\"private\",\"confidence\":60,\"source_confidence_weight\":100,\"expiration_ts\":\"2017-01-26T00:00:00Z\","
		+ "\"severity\":\"high\",\"source_feed_id\":5,\"tags\":[{\"name\":\"public_tag\"}],\"trustedcircles\":[13],"
		+ "\"source_locations\":[\"europe\"],\"target_locations\":[\"US-FL\"],\"target_industry\":[\"government-regional\"],\"remote_api\":true},"
		+ "\"objects\":[{\"itype\":\"bot_ip\",\"srcip\":\"192.0.2.4\",\"severity\":\"high\"},"
		+ "{\"itype\":\"mal_domain\",\"domain\":\"idfsdszqylwjzq.biz\"},"
		+ "{\"itype\":\"mal_url\",\"url\":\"http://malicious.example/a.exe\"},"
		+ "{\"itype\":\"compromised_email\",\"email\":\"email@domain.example\",\"confidence\":50},"
		+ "{\"itype\":\"apt_md5\",\"md5\":\"1d37556b8aeb5cb5fbf08cd5b4790075\",\"expiration_ts\":\"2017-01-26T00:00:00Z\"},"
		+ "{\"itype\":\"mal_ip\",\"value\":\"203.0.113.198\"}]}";
}
