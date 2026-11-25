using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class IncidentsTests
{
	private static readonly string IncidentJson = Entity(79, "incident", """
		"status": 2,
		"status_desc": "this is an open incident",
		"start_date": "2014-10-02T20:44:35",
		"end_date": "2014-10-09T20:44:35",
		"victims": [13],
		"actors": [223],
		"sandbox_reports": [{"id": 31, "resource_uri": "/api/v1/submit/31/"}]
		""");

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.ListAsync(new ThreatModelListOptions { Limit = 10 }, ct), Page(IncidentJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/", "?limit=10");
	}

	private static async Task CreateShouldSendAsync(IncidentCreateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.CreateAsync(request, ct), IncidentJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/incident/", body: body);
	}

	[Fact]
	public Task CreateAsync_SendsPostWithJsonBody()
		=> CreateShouldSendAsync(
			new IncidentCreateRequest
			{
				Name = "dummy test incident",
				Tlp = Tlp.Red,
				Tags = ["big incident", "dummy tag"],
				Ttps = [63],
				Status = 2,
				StatusDescription = "this is an open incident"
			},
			"""{"name":"dummy test incident","status":2,"status_desc":"this is an open incident","tags":["big incident","dummy tag"],"tlp":"red","ttps":[63]}""");

	[Fact]
	public Task CreateAsync_SendsEveryOwnAttribute()
		=> CreateShouldSendAsync(
			new IncidentCreateRequest
			{
				Name = "i",
				StartDate = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
				EndDate = new DateTimeOffset(2014, 10, 9, 20, 44, 35, TimeSpan.Zero),
				Victims = [13],
				Actors = [223],
				SandboxReports = [31]
			},
			"""{"name":"i","actors":[223],"victims":[13],"sandbox_reports":[31],"start_date":"2014-10-02T20:44:35Z","end_date":"2014-10-09T20:44:35Z"}""");

	[Fact]
	public async Task GetAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.GetAsync(12345, new ThreatModelDetailOptions { SkipAssociations = true }, ct), IncidentJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/12345/", "?skip_associations=true");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var incident = await TestClient.ReadAsync((client, ct) => client.Incidents.GetAsync(79, null, ct), IncidentJson);

		incident.ShouldHaveEntityFields(79, "incident");
		incident.Status!.Id.Should().Be(2);
		incident.StatusDescription.Should().Be("this is an open incident");
		incident.StartDate.Should().Be(new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero));
		incident.EndDate.Should().Be(new DateTimeOffset(2014, 10, 9, 20, 44, 35, TimeSpan.Zero));
		incident.Victims.Should().ContainSingle().Which.Id.Should().Be(13);
		incident.Actors.Should().ContainSingle().Which.Id.Should().Be(223);
		incident.SandboxReports.Should().ContainSingle().Which.ResourceUri.Should().Be("/api/v1/submit/31/");
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithJsonBody()
	{
		var request = new IncidentUpdateRequest { Name = "another dummy test incident", Tlp = Tlp.Amber, Status = 7, StatusDescription = "this incident is closed" };

		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.UpdateAsync(79, request, ct), IncidentJson);

		call.ShouldBe(
			HttpMethod.Patch,
			"/api/v1/incident/79/",
			body: """{"status":7,"status_desc":"this incident is closed","name":"another dummy test incident","tlp":"amber"}""");
	}

	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Incidents.CreateAsync(new IncidentCreateRequest { Name = "x" }, ct), "POST", "/api/v1/incident/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Incidents.UpdateAsync(79, new IncidentUpdateRequest(), ct), "PATCH", "/api/v1/incident/79/");

	[Fact]
	public async Task ListAssociatedWithValueAsync_SendsTheValue()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.ListAssociatedWithValueAsync("192.0.2.4", null, ct), Page(IncidentJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/associated_with_intelligence/", "?value=192.0.2.4");
	}

	[Fact]
	public async Task ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.ListAssociatedWithIdsAsync([7, 8], null, ct), Page(IncidentJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/associated_with_intelligence/", "?ids=7%2C8");
	}

	[Fact]
	public async Task ListLinkedAsync_SendsTheEntityTypeInThePath()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.ListLinkedAsync(234673, ThreatModelType.ThreatBulletin, null, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/234673/tipreport/");
	}

	[Fact]
	public async Task ListIntelligenceAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.ListIntelligenceAsync(79, null, ct), Page(ObservableJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/79/intelligence/");
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.Incidents.GetAsync(1, null, ct));
}
