using System.Net;
using System.Text.Json;
using ThreatStream.Api.Models.Enrichments;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class EnrichmentsTests
{
	// The ThreatStream API reference's (Intelligence Enrichments) AbuseIPDB example, made valid JSON, with the ISP and domain anonymised.
	private const string AbuseIpDbJson = """
		[
			{
				"status_code": 200,
				"results": {
					"data": {
						"numDistinctUsers": 0,
						"countryName": "USA",
						"domain": "example.us",
						"isp": "Example Telecom",
						"lastReportedAt": null,
						"countryCode": "US",
						"isPublic": true,
						"usageType": null,
						"reports": [],
						"ipVersion": 4,
						"hostnames": [],
						"abuseConfidenceScore": 0,
						"isWhitelisted": null,
						"ipAddress": "198.51.100.101",
						"totalReports": 0
					}
				}
			}
		]
		""";

	// Shodan Database may answer from several endpoints, some without data; Recorded Future answers 201 on success.
	private const string SeveralJson = """
		[
			{"status_code": 200, "results": {"ip_str": "198.51.100.101", "ports": [80, 443]}},
			{"status_code": 404, "results": null},
			{"status_code": "201", "results": "No information available", "service": "x"}
		]
		""";

	private static readonly DirectEnrichmentLookup RecordedFutureIp = new() { Type = EnrichmentValueType.Ip, Value = "198.51.100.101" };

	[Fact]
	public async Task GetIpAsync_SendsGetWithTheServiceName()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Enrichments.GetIpAsync("198.51.100.101", IpEnrichmentService.AbuseIpDb, ct), AbuseIpDbJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/integration_package/direct/ip/198.51.100.101/", "?name=AbuseIPDB");
	}

	[Fact]
	public async Task GetIpAsync_EncodesShodanDatabaseAndIpv6()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Enrichments.GetIpAsync("2001:db8::1", IpEnrichmentService.ShodanDatabase, ct), SeveralJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/integration_package/direct/ip/2001%3Adb8%3A%3A1/", "?name=Shodan%20Database");
	}

	[Fact]
	public async Task GetIpAsync_MapsTheAbuseIpDbAnswer()
	{
		var results = await TestClient.ReadAsync((client, ct) => client.Enrichments.GetIpAsync("198.51.100.101", IpEnrichmentService.AbuseIpDb, ct), AbuseIpDbJson);

		var result = results.Should().ContainSingle().Subject;
		result.StatusCode.Should().Be(200);
		var data = result.Results!.Value.GetProperty("data");
		data.GetProperty("abuseConfidenceScore").GetInt32().Should().Be(0);
		data.GetProperty("ipAddress").GetString().Should().Be("198.51.100.101");
		data.GetProperty("countryCode").GetString().Should().Be("US");
	}

	[Fact]
	public async Task GetIpAsync_MapsSeveralAnswers()
	{
		var results = await TestClient.ReadAsync((client, ct) => client.Enrichments.GetIpAsync("198.51.100.101", IpEnrichmentService.ShodanDatabase, ct), SeveralJson);

		results.Should().HaveCount(3);
		results[0].Results!.Value.GetProperty("ports").GetArrayLength().Should().Be(2);
		results[1].StatusCode.Should().Be(404);
		results[1].Results.Should().BeNull();
		results[2].StatusCode.Should().Be(201);
		results[2].Results!.Value.ValueKind.Should().Be(JsonValueKind.String);
		results[2].AdditionalProperties.Should().ContainKey("service");
	}

	[Fact]
	public async Task LookupAsync_PostsTheLookup()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Enrichments.LookupAsync(RecordedFutureIp, ct), SeveralJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/integration_package/direct/", body: """{"name":"Recorded Future","type":"ip","value":"198.51.100.101"}""");
	}

	[Theory]
	[InlineData(EnrichmentValueType.Url, "url")]
	[InlineData(EnrichmentValueType.Domain, "domain")]
	[InlineData(EnrichmentValueType.Hash, "hash")]
	public async Task LookupAsync_SpellsEveryType(EnrichmentValueType type, string wire)
	{
		var lookup = new DirectEnrichmentLookup { Name = "Other Service", Type = type, Value = "v" };

		var call = await TestClient.CaptureAsync((client, ct) => client.Enrichments.LookupAsync(lookup, ct), SeveralJson);

		call.Body.Should().Be($$"""{"name":"Other Service","type":"{{wire}}","value":"v"}""");
	}

	[Fact]
	public async Task LookupAsync_IsAllowedByAReadOnlyClient()
	{
		var results = await SideEffectTestKit.ReadWhenReadOnlyAsync((client, ct) => client.Enrichments.LookupAsync(RecordedFutureIp, ct), SeveralJson);

		results.Should().HaveCount(3);
	}

	[Fact]
	public async Task LookupWithGetAsync_SendsTheLookupInTheQuery()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Enrichments.LookupWithGetAsync(RecordedFutureIp, ct), SeveralJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/integration_package/direct/", "?name=Recorded%20Future&type=ip&value=198.51.100.101");
	}

	[Fact]
	public Task GetIpAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Enrichments.GetIpAsync("198.51.100.101", IpEnrichmentService.AbuseIpDb, ct),
			HttpStatusCode.Forbidden,
			"""{"error": "Not subscribed"}""",
			"Not subscribed");
}
