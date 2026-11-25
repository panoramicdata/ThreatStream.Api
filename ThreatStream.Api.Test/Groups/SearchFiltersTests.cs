using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class SearchFiltersTests
{
	// The example in the ThreatStream API reference (Search Filter), with the user anonymised.
	private const string PageJson = """
		{
			"meta": {"limit": 20, "next": null, "offset": 0, "previous": null, "total_count": 1},
			"objects": [
				{
					"created_by_user": {"id": "7", "email": "analyst@example.com"},
					"created_ts": "2020-04-28T23:52:53.612664",
					"es_query": {"query": {}},
					"id": "3303",
					"is_editable": true,
					"modified_ts": "2020-04-28T23:52:53.612664",
					"name": "subtype_test_severity",
					"query": "(subtype=\"md5\") and (status=\"active\") and (created_ts>=2020-03-25T21:54:48) and (severity=\"high\")",
					"resource_uri": "/api/v1/search_filter/3303/"
				}
			]
		}
		""";

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SearchFilters.ListAsync(new ListOptions { Limit = 50 }, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/search_filter/", "?limit=50");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.SearchFilters.ListAsync(null, ct), PageJson);

		var filter = page.Objects.Should().ContainSingle().Subject;
		filter.Id.Should().Be(3303);
		filter.ResourceUri.Should().Be("/api/v1/search_filter/3303/");
		filter.Name.Should().Be("subtype_test_severity");
		filter.Query.Should().Be("""(subtype="md5") and (status="active") and (created_ts>=2020-03-25T21:54:48) and (severity="high")""");
		filter.IsEditable.Should().BeTrue();
		filter.CreatedTs.Should().Be(new DateTimeOffset(2020, 4, 28, 23, 52, 53, TimeSpan.Zero).AddTicks(6126640));
		filter.ModifiedTs.Should().Be(filter.CreatedTs);
		filter.AdditionalProperties.Keys.Should().BeEquivalentTo("created_by_user", "es_query");
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.SearchFilters.ListAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "Forbidden"}""",
			"Forbidden");
}
