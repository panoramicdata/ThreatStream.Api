using System.Net;
using System.Text;
using ThreatStream.Api.Models.Whitelist;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public partial class WhitelistTests
{
	// An entry with every field in the ThreatStream API reference's (Whitelist) response table, using the add example's values.
	private const string EntryJson = """
		{
			"created_ts": "2024-05-06T07:08:09",
			"id": 1201,
			"modified_ts": "2024-05-07T08:09:10.5",
			"notes": "example domain",
			"resource_uri": "/api/v1/orgwhitelist/1201/",
			"value": "example44.org",
			"value_type": "domain"
		}
		""";

	private const string PageJson = $$"""
		{
			"meta": {"limit": 10, "next": "/api/v1/orgwhitelist/?format=json&limit=10&offset=10", "offset": 0, "previous": null, "total_count": 31},
			"objects": [{{EntryJson}}, {"id": 1202, "value": "Mozilla/5.0 (scanner)", "value_type": "user-agent"}]
		}
		""";

	// The CSV file of the reference's upload example, also the shape of its CSV export.
	private const string Csv = """
		value_type,value,notes
		domain,*foo.wildcarddomain.com,some notes on foo
		ip,8.8.8.8,notes on ip address
		""";

	[Fact]
	public async Task ListAsync_SendsGetAsJson()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.ListAsync(null, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/orgwhitelist/", "?format=json");
	}

	[Fact]
	public async Task ListAsync_SendsNotesAndPaging()
	{
		var options = new WhitelistListOptions { ShowNote = false, Limit = 0 };

		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.ListAsync(options, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/orgwhitelist/", "?format=json&showNote=false&limit=0");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Whitelist.ListAsync(null, ct), PageJson);

		page.Meta!.TotalCount.Should().Be(31);
		page.Objects.Should().HaveCount(2);
		ShouldBeTheExampleEntry(page.Objects[0]);
		page.Objects[1].ValueType.Should().Be(WhitelistValueType.UserAgent);
		page.Objects[1].Notes.Should().BeNull();
	}

	[Fact]
	public async Task ExportCsvAsync_SendsGetAsCsv()
	{
		var options = new WhitelistListOptions { ShowNote = true };

		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.ExportCsvAsync(options, ct), Csv);

		call.ShouldBe(HttpMethod.Get, "/api/v1/orgwhitelist/", "?format=csv&showNote=true");
	}

	[Fact]
	public async Task ExportCsvAsync_ReturnsTheText()
	{
		var stub = new StubHandler();
		stub.EnqueueFile(Encoding.UTF8.GetBytes(Csv), "text/csv", "whitelist.csv");
		using var client = TestClient.Create(stub);

		var csv = await client.Whitelist.ExportCsvAsync(null, TestContext.Current.CancellationToken);

		csv.Should().Be(Csv);
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Whitelist.ListAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"error": "Org Admin required"}""",
			"Org Admin required");

	private static void ShouldBeTheExampleEntry(WhitelistEntry entry)
	{
		entry.Id.Should().Be(1201);
		entry.ResourceUri.Should().Be("/api/v1/orgwhitelist/1201/");
		entry.ValueType.Should().Be(WhitelistValueType.Domain);
		entry.Value.Should().Be("example44.org");
		entry.Notes.Should().Be("example domain");
		entry.CreatedTs.Should().Be(new DateTimeOffset(2024, 5, 6, 7, 8, 9, TimeSpan.Zero));
		entry.ModifiedTs.Should().Be(new DateTimeOffset(2024, 5, 7, 8, 9, 10, 500, TimeSpan.Zero));
		entry.AdditionalProperties.Should().BeEmpty();
	}
}
