using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class CampaignsTests
{
	private static readonly string CampaignJson = Entity(1062, "campaign", """
		"status": {"id": 1, "display_name": "Ongoing", "resource_uri": "/api/v1/campaignstatus/1/"},
		"start_date": "2014-10-02T20:44:35",
		"end_date": "",
		"victims": ["13"],
		"actors": [{"id": 223, "name": "test actor", "resource_uri": "/api/v1/actor/223/"}],
		"incidents": [79]
		""");

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.ListAsync(new ThreatModelListOptions { Limit = 10 }, ct), Page(CampaignJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaign/", "?limit=10");
	}

	private static async Task CreateShouldSendAsync(CampaignCreateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.CreateAsync(request, ct), CampaignJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/campaign/", body: body);
	}

	[Fact]
	public Task CreateAsync_SendsPostWithJsonBody()
		=> CreateShouldSendAsync(
			new CampaignCreateRequest
			{
				Name = "dummy test campaign",
				Tlp = Tlp.Red,
				Tags = ["big campaign", "dummy tag"],
				Intelligence = [165546],
				PublicationStatus = ThreatModelPublicationStatus.Published
			},
			"""{"name":"dummy test campaign","tags":["big campaign","dummy tag"],"tlp":"red","publication_status":"published","intelligence":[165546]}""");

	[Fact]
	public Task CreateAsync_SendsEveryOwnAttribute()
		=> CreateShouldSendAsync(
			new CampaignCreateRequest
			{
				Name = "c",
				Status = 1,
				StartDate = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
				EndDate = new DateTimeOffset(2014, 11, 2, 20, 44, 35, TimeSpan.Zero),
				Victims = [13],
				Actors = [223],
				Incidents = [79]
			},
			"""{"name":"c","status":1,"start_date":"2014-10-02T20:44:35Z","end_date":"2014-11-02T20:44:35Z","victims":[13],"actors":[223],"incidents":[79]}""");

	[Fact]
	public async Task GetAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.GetAsync(1062, null, ct), CampaignJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaign/1062/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var campaign = await TestClient.ReadAsync((client, ct) => client.Campaigns.GetAsync(1062, null, ct), CampaignJson);

		campaign.ShouldHaveEntityFields(1062, "campaign");
		campaign.Status!.DisplayName.Should().Be("Ongoing");
		campaign.StartDate.Should().Be(new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero));
		campaign.EndDate.Should().BeNull();
		campaign.Victims.Should().ContainSingle().Which.Id.Should().Be(13);
		campaign.Actors.Should().ContainSingle().Which.ResourceUri.Should().Be("/api/v1/actor/223/");
		campaign.Incidents.Should().ContainSingle().Which.Id.Should().Be(79);
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithJsonBody()
	{
		var request = new CampaignUpdateRequest { Name = "another dummy test campaign", Tlp = Tlp.Amber };

		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.UpdateAsync(1062, request, ct), CampaignJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/campaign/1062/", body: """{"name":"another dummy test campaign","tlp":"amber"}""");
	}

	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Campaigns.CreateAsync(new CampaignCreateRequest { Name = "x" }, ct), "POST", "/api/v1/campaign/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Campaigns.UpdateAsync(1062, new CampaignUpdateRequest(), ct), "PATCH", "/api/v1/campaign/1062/");

	[Fact]
	public async Task ListAssociatedWithValueAsync_SendsTheValue()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.ListAssociatedWithValueAsync("http://bad.example/a?b=c", null, ct), Page(CampaignJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaign/associated_with_intelligence/", "?value=http%3A%2F%2Fbad.example%2Fa%3Fb%3Dc");
	}

	[Fact]
	public async Task ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.ListAssociatedWithIdsAsync([4, 5], null, ct), Page(CampaignJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaign/associated_with_intelligence/", "?ids=4%2C5");
	}

	[Fact]
	public async Task ListLinkedAsync_SendsTheEntityTypeInThePath()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.ListLinkedAsync(234673, ThreatModelType.Actor, null, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaign/234673/actor/");
	}

	[Fact]
	public async Task ListIntelligenceAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.ListIntelligenceAsync(1062, null, ct), Page(ObservableJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/campaign/1062/intelligence/");
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.Campaigns.GetAsync(1, null, ct));
}
