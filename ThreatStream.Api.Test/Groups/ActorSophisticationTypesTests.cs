using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class ActorSophisticationTypesTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ActorSophisticationTypes.ListAsync(null, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/actorsophisticationtype/");
	}

	[Fact]
	public async Task ListAsync_MapsTheValues()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.ActorSophisticationTypes.ListAsync(null, ct), Page(LookupJson("actorsophisticationtype", 3, "Expert")));

		page.ShouldBeTheLookup("actorsophisticationtype", 3, "Expert");
	}

	[Fact]
	public Task ListAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.ActorSophisticationTypes.ListAsync(null, ct));
}
