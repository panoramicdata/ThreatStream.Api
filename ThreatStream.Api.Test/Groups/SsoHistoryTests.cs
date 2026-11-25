using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class SsoHistoryTests
{
	// The reference's example entry, with the user anonymised.
	private const string PageJson = """
		{
			"meta": {"limit": 20, "offset": 0, "total_count": 1},
			"objects": [
				{
					"action": "Login Attempt",
					"deployment_type": "OnPrem",
					"id": 1000000019,
					"idp": "adfs-onprem",
					"resource_uri": "/api/v1/ssohistory/1000000019/",
					"ts": "2020-10-01T10:07:23.916639",
					"user_email": "analyst@example.com"
				}
			]
		}
		""";

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SsoHistory.ListAsync(new ListOptions { OrderBy = "-ts" }, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/ssohistory/", "?order_by=-ts");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.SsoHistory.ListAsync(null, ct), PageJson);

		var entry = page.Objects.Should().ContainSingle().Subject;
		entry.Id.Should().Be(1000000019);
		entry.ResourceUri.Should().Be("/api/v1/ssohistory/1000000019/");
		entry.Action.Should().Be("Login Attempt");
		entry.DeploymentType.Should().Be("OnPrem");
		entry.Idp.Should().Be("adfs-onprem");
		entry.UserEmail.Should().Be("analyst@example.com");
		entry.Ts.Should().Be(new DateTimeOffset(2020, 10, 1, 10, 7, 23, TimeSpan.Zero).AddTicks(9166390));
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.SsoHistory.ListAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "Org Admin permission required"}""",
			"Org Admin permission required");
}
