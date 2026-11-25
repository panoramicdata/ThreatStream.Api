using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class WorkgroupsTests
{
	// The example response in the ThreatStream API reference (Workgroup API), with the organization anonymised.
	private const string PageJson = """
		{
			"meta": {
				"limit": 20,
				"next": "/api/v1/workgroup/?limit=20&offset=20",
				"offset": 0,
				"previous": null,
				"total_count": 51
			},
			"objects": [
				{
					"avatar_s3_url": null,
					"created_ts": "2016-09-02T20:31:11.211401",
					"id": 2,
					"members": 8,
					"modified_ts": "2016-09-03T08:15:00",
					"name": "Threat Intel Team",
					"owner_org": {
						"id": "2",
						"name": "Example Org",
						"resource_uri": "/api/v1/userorganization/2/"
					},
					"resource_uri": "/api/v1/workgroup/2/"
				}
			]
		}
		""";

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Workgroups.ListAsync(null, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/workgroup/");
	}

	[Fact]
	public async Task ListAsync_SendsPaging()
	{
		var options = new ListOptions { Limit = 50, Offset = 100, OrderBy = "name" };

		var call = await TestClient.CaptureAsync((client, ct) => client.Workgroups.ListAsync(options, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/workgroup/", "?limit=50&offset=100&order_by=name");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Workgroups.ListAsync(null, ct), PageJson);

		page.Meta!.TotalCount.Should().Be(51);
		var workgroup = page.Objects.Should().ContainSingle().Subject;
		workgroup.Id.Should().Be(2);
		workgroup.ResourceUri.Should().Be("/api/v1/workgroup/2/");
		workgroup.Name.Should().Be("Threat Intel Team");
		workgroup.Members.Should().Be(8);
		workgroup.CreatedTs.Should().Be(new DateTimeOffset(2016, 9, 2, 20, 31, 11, TimeSpan.Zero).AddTicks(2114010));
		workgroup.ModifiedTs.Should().Be(new DateTimeOffset(2016, 9, 3, 8, 15, 0, TimeSpan.Zero));
		workgroup.AvatarS3Url.Should().BeNull();
		workgroup.OwnerOrg!.Id.Should().Be(2);
		workgroup.OwnerOrg.Name.Should().Be("Example Org");
		workgroup.OwnerOrg.ResourceUri.Should().Be("/api/v1/userorganization/2/");
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Workgroups.ListAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "Access denied"}""",
			"Access denied");
}
