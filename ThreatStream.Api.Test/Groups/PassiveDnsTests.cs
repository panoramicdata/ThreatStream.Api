using System.Net;
using ThreatStream.Api.Models.Enrichments;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class PassiveDnsTests
{
	// The reference has no example response; this is the shape ThreatStream integrations read, with documentation addresses.
	private const string ResultJson = """
		{
			"success": true,
			"results": [
				{
					"domain": "example.com",
					"ip": "192.0.2.10",
					"rrtype": "A",
					"source": "Spamhaus",
					"first_seen": "2023-01-02T03:04:05",
					"last_seen": "2024-06-07T08:09:10",
					"count": 12
				}
			]
		}
		""";

	[Fact]
	public async Task GetAsync_SendsGetForADomain()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.PassiveDns.GetAsync(PassiveDnsLookupType.Domain, "example.com", ct), ResultJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/pdns/domain/example.com/");
	}

	[Fact]
	public async Task GetAsync_SendsGetForAnIp()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.PassiveDns.GetAsync(PassiveDnsLookupType.Ip, "192.0.2.10", ct), ResultJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/pdns/ip/192.0.2.10/");
	}

	[Fact]
	public async Task GetAsync_EscapesTheValueAsOneSegment()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.PassiveDns.GetAsync(PassiveDnsLookupType.Domain, "a/b?c", ct), ResultJson);

		call.Uri.AbsolutePath.Should().Be("/api/v1/pdns/domain/a%2Fb%3Fc/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var result = await TestClient.ReadAsync((client, ct) => client.PassiveDns.GetAsync(PassiveDnsLookupType.Domain, "example.com", ct), ResultJson);

		result.Success.Should().BeTrue();
		var record = result.Results.Should().ContainSingle().Subject;
		record.Domain.Should().Be("example.com");
		record.Ip.Should().Be("192.0.2.10");
		record.RrType.Should().Be("A");
		record.Source.Should().Be("Spamhaus");
		record.FirstSeen.Should().Be(new DateTimeOffset(2023, 1, 2, 3, 4, 5, TimeSpan.Zero));
		record.LastSeen.Should().Be(new DateTimeOffset(2024, 6, 7, 8, 9, 10, TimeSpan.Zero));
		record.AdditionalProperties["count"].GetInt32().Should().Be(12);
	}

	[Fact]
	public async Task GetAsync_NothingFound_ReadsAsEmpty()
	{
		var result = await TestClient.ReadAsync(
			(client, ct) => client.PassiveDns.GetAsync(PassiveDnsLookupType.Ip, "192.0.2.99", ct),
			"""{"success": true, "results": null}""");

		result.Results.Should().BeEmpty();
	}

	[Fact]
	public Task GetAsync_RateLimited_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.PassiveDns.GetAsync(PassiveDnsLookupType.Domain, "example.com", ct),
			HttpStatusCode.TooManyRequests,
			"""{"error": "Rate limit exceeded"}""",
			"Rate limit exceeded");
}
