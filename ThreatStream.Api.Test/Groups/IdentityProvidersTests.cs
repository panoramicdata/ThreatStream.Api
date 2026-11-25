using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UsersRolesTestKit;

namespace ThreatStream.Api.Test.Groups;

public class IdentityProvidersTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.IdentityProviders.ListAsync(null, ct), IdentityProvidersPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/identity-provider/");
	}

	[Fact]
	public async Task ListAsync_SendsPaging()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.IdentityProviders.ListAsync(new ListOptions { Limit = 5 }, ct),
			IdentityProvidersPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/identity-provider/", "?limit=5");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.IdentityProviders.ListAsync(null, ct), IdentityProvidersPageJson);

		var provider = page.Objects.Should().ContainSingle().Subject;
		provider.Id.Should().Be(159);
		provider.ResourceUri.Should().Be("/api/v1/identity-provider/159/");
		provider.AdditionalProperties["name"].GetString().Should().Be("Entra ID");
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.IdentityProviders.ListAsync(null, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "Users & Roles Management is not enabled"}""",
			"Users & Roles Management is not enabled");
}
