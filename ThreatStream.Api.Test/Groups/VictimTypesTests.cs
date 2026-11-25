using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class VictimTypesTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.VictimTypes.ListAsync(Paging, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/victimtype/", PagingQuery);
	}

	[Fact]
	public async Task ListAsync_MapsTheValues()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.VictimTypes.ListAsync(null, ct), Page(LookupJson("victimtype", 13, "Health Care")));

		page.ShouldBeTheLookup("victimtype", 13, "Health Care");
	}

	[Fact]
	public Task ListAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.VictimTypes.ListAsync(null, ct));
}
