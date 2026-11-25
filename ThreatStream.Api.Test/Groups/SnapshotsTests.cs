using System.Net;
using ThreatStream.Api.Models.Snapshots;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class SnapshotsTests
{
	// The reference's status output, with its placeholders filled in, plus the list attributes it documents.
	private const string SnapshotJson = """
		{
			"id": 2,
			"resource_uri": "/api/v1/snapshot/2/",
			"location": "https://api.threatstream.com/api/v1/snapshot/2/",
			"snapshot_ts": "2026-01-02T03:04:05",
			"sha256sum": "abc123",
			"status": "inprogress",
			"files": [
				{"download_url": "https://downloads.example/1.json", "sha256": "e3b0c442", "total_count": "250000"},
				{"download_url": "https://downloads.example/2.json", "sha256": "98ea6e4f", "total_count": 1200}
			]
		}
		""";

	private static SnapshotCreateRequest Request() => new()
	{
		IntelVersion = 1,
		CustomConfig = new SnapshotConfig
		{
			Filter = "state=\"active\"",
			MaxResults = 50000,
			Whitelist = new Dictionary<string, IReadOnlyList<string>> { ["itype"] = ["mal_ip", "mal_domain"] },
			Blacklist = new Dictionary<string, IReadOnlyList<string>> { ["country"] = ["US"] },
			FieldWhitelist = ["srcip", "domain"],
			FieldBlacklist = ["asn", "country"],
			ChunkSize = "1MB",
			ChunkIocCount = 1000,
			Format = SnapshotFormat.JsonV2,
			UseOrgWhitelist = true
		}
	};

	[Fact]
	public async Task ListAsync_SendsGetWithTheFilters()
	{
		var options = new SnapshotListOptions
		{
			IdAfter = 1,
			StartedSince = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
			StartedBefore = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
			Sha256Sum = "abc123",
			OrderBy = "-id"
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Snapshots.ListAsync(options, ct), EmptyPage);

		call.ShouldBe(
			HttpMethod.Get,
			"/api/v1/snapshot/",
			"?id__gt=1&snapshot_ts__gte=2026-01-01T00%3A00%3A00Z&snapshot_ts__lt=2026-02-01T00%3A00%3A00Z&sha256sum=abc123&order_by=-id");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Snapshots.ListAsync(null, ct), $$"""{"meta":{},"objects":[{{SnapshotJson}}]}""");

		ShouldBeTheExample(page.Objects.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task CreateAsync_SendsPostWithTheConfig()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Snapshots.CreateAsync(Request(), ct), SnapshotJson);

		call.ShouldBe(
			HttpMethod.Post,
			"/api/v1/snapshot/",
			body: "{\"intel_version\":1,\"custom_config\":{\"filter\":\"state=\\u0022active\\u0022\",\"max_results\":50000,"
				+ "\"whitelist\":{\"itype\":[\"mal_ip\",\"mal_domain\"]},\"blacklist\":{\"country\":[\"US\"]},"
				+ "\"field_whitelist\":[\"srcip\",\"domain\"],\"field_blacklist\":[\"asn\",\"country\"],\"chunk_size\":\"1MB\","
				+ "\"chunk_ioc_count\":1000,\"format\":\"json_v2\",\"use_org_whitelist\":true}}");
	}

	[Fact]
	public async Task CreateAsync_EmptyBody_ReadsAsNull()
	{
		var snapshot = await TestClient.ReadAsync((client, ct) => client.Snapshots.CreateAsync(Request(), ct), string.Empty);

		snapshot.Should().BeNull();
	}

	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Snapshots.CreateAsync(Request(), ct), "POST", "api/v1/snapshot/");

	[Fact]
	public Task CreateAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Snapshots.CreateAsync(Request(), ct),
			HttpStatusCode.TooManyRequests,
			"""{"message": "Too many snapshots in progress"}""",
			"Too many snapshots in progress");

	[Fact]
	public async Task GetAsync_SendsGetToTheSnapshot()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Snapshots.GetAsync(2, ct), SnapshotJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/snapshot/2/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
		=> ShouldBeTheExample(await TestClient.ReadAsync((client, ct) => client.Snapshots.GetAsync(2, ct), SnapshotJson));

	private static void ShouldBeTheExample(Snapshot snapshot)
	{
		snapshot.Id.Should().Be(2);
		snapshot.ResourceUri.Should().Be("/api/v1/snapshot/2/");
		snapshot.Location.Should().Be("https://api.threatstream.com/api/v1/snapshot/2/");
		snapshot.SnapshotTs.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
		snapshot.Sha256Sum.Should().Be("abc123");
		snapshot.Status.Should().Be("inprogress");
		snapshot.Files.Select(f => (f.DownloadUrl, f.Sha256, f.TotalCount)).Should().Equal(
			("https://downloads.example/1.json", "e3b0c442", 250000L),
			("https://downloads.example/2.json", "98ea6e4f", 1200L));
	}
}
