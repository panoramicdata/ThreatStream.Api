using System.Net;
using ThreatStream.Api.Models.Feeds;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class FeedsTests
{
	// A feed built from the reference's attribute table (it gives no example), with internal attributes left unmodelled.
	private const string PageJson = """
		{
			"meta": {"limit": 20, "offset": 0, "total_count": 1},
			"objects": [
				{
					"active": true,
					"circles": [13],
					"classification": "private",
					"confidence": "80",
					"config": [],
					"created_ts": "2019-03-01T10:00:00",
					"description": "Botnet C2 servers",
					"expiry_time_in_days": 90,
					"id": 19,
					"interval": 3600,
					"itype": "c2_ip",
					"last_imported": "2026-01-02T03:04:05",
					"last_scraped": "2026-01-02T03:00:00",
					"last_updated": "2025-12-01T00:00:00",
					"name": "Example C2 Feed",
					"organization": {"id": 2, "name": "Example Org", "resource_uri": "/api/v1/userorganization/2/"},
					"resource_uri": "/api/v1/feed/19/",
					"source_confidence_weight": 30,
					"state": "success",
					"tags": ["c2"],
					"url": "https://feeds.example/c2.txt",
					"engine": "internal"
				}
			]
		}
		""";

	[Fact]
	public async Task ListAsync_SendsGetWithTheFilters()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.Feeds.ListAsync(new FeedListOptions { Id = 19, Name = "Example C2 Feed", Itype = "c2_ip", ExpiryTimeInDays = 90, Limit = 100 }, ct),
			EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/feed/", "?id=19&name=Example%20C2%20Feed&itype=c2_ip&expiry_time_in_days=90&limit=100");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Feeds.ListAsync(null, ct), PageJson);

		var feed = page.Objects.Should().ContainSingle().Subject;
		feed.Id.Should().Be(19);
		feed.ResourceUri.Should().Be("/api/v1/feed/19/");
		feed.Name.Should().Be("Example C2 Feed");
		feed.Description.Should().Be("Botnet C2 servers");
		feed.Active.Should().BeTrue();
		feed.State.Should().Be("success");
		feed.Classification.Should().Be("private");
		feed.Itype.Should().Be("c2_ip");
		feed.Confidence.Should().Be(80);
		feed.SourceConfidenceWeight.Should().Be(30);
		feed.ExpiryTimeInDays.Should().Be(90);
		feed.Interval.Should().Be("3600");
		feed.Url.Should().Be("https://feeds.example/c2.txt");
		feed.Organization!.Id.Should().Be(2);
		feed.Organization.Name.Should().Be("Example Org");
		feed.Organization.ResourceUri.Should().Be("/api/v1/userorganization/2/");
		feed.CreatedTs.Should().Be(new DateTimeOffset(2019, 3, 1, 10, 0, 0, TimeSpan.Zero));
		feed.LastImported.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
		feed.LastScraped.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 0, 0, TimeSpan.Zero));
		feed.LastUpdated.Should().Be(new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero));
		feed.AdditionalProperties.Keys.Should().BeEquivalentTo("circles", "config", "tags", "engine");
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Feeds.ListAsync(null, ct),
			HttpStatusCode.Unauthorized,
			"""{"message": "Invalid credentials"}""",
			"Invalid credentials");
}
