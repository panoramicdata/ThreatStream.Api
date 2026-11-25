using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class CampaignStatusesTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.CampaignStatuses.ListAsync(new ListOptions { OrderBy = "value" }, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaignstatus/", "?order_by=value");
	}

	[Fact]
	public async Task ListAsync_MapsTheValues()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.CampaignStatuses.ListAsync(null, ct), Page(LookupJson("campaignstatus", 1, "Ongoing")));

		page.ShouldBeTheLookup("campaignstatus", 1, "Ongoing");
	}

	[Fact]
	public Task ListAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.CampaignStatuses.ListAsync(null, ct));
}
