using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class IncidentStatusTypesTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.IncidentStatusTypes.ListAsync(new ListOptions { Limit = 0 }, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/incidentstatustype/", "?limit=0");
	}

	[Fact]
	public async Task ListAsync_MapsTheValues()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.IncidentStatusTypes.ListAsync(null, ct), Page(LookupJson("incidentstatustype", 2, "Open")));

		page.ShouldBeTheLookup("incidentstatustype", 2, "Open");
	}

	[Fact]
	public Task ListAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.IncidentStatusTypes.ListAsync(null, ct));
}
