using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class AppStoreHistoryTests
{
	// The reference's example entry, plus the email it documents for actions by your own users.
	private const string PageJson = """
		{
			"meta": {"limit": 20, "offset": 0, "total_count": 1},
			"objects": [
				{
					"edit_type": "Accepted Paid",
					"email": "analyst@example.com",
					"id": 929,
					"name": "Cofense Intelligence",
					"resource_uri": "/api/v1/appstorehistory/929/",
					"timestamp": "2020-06-05 20:32:56.511717+00:00"
				}
			]
		}
		""";

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AppStoreHistory.ListAsync(new ListOptions { Limit = 10, Offset = 10 }, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/appstorehistory/", "?limit=10&offset=10");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.AppStoreHistory.ListAsync(null, ct), PageJson);

		var entry = page.Objects.Should().ContainSingle().Subject;
		entry.Id.Should().Be(929);
		entry.ResourceUri.Should().Be("/api/v1/appstorehistory/929/");
		entry.EditType.Should().Be("Accepted Paid");
		entry.Email.Should().Be("analyst@example.com");
		entry.Name.Should().Be("Cofense Intelligence");
		entry.Timestamp.Should().Be(new DateTimeOffset(2020, 6, 5, 20, 32, 56, TimeSpan.Zero).AddTicks(5117170));
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.AppStoreHistory.ListAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "Forbidden"}""",
			"Forbidden");
}
