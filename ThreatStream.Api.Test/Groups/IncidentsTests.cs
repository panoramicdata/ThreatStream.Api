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

	[Fact]
	public async Task CreateAsync_SendsPostWithJsonBody()
	{
		var request = new IncidentCreateRequest
		{
			Name = "dummy test incident",
			Tlp = Tlp.Red,
			Tags = ["big incident", "dummy tag"],
			Ttps = [63],
			Status = 2,
			StatusDescription = "this is an open incident"
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.CreateAsync(request, ct), IncidentJson);

		call.ShouldBe(
			HttpMethod.Post,
			"/api/v1/incident/",
			body: """{"name":"dummy test incident","status":2,"status_desc":"this is an open incident","tags":["big incident","dummy tag"],"tlp":"red","ttps":[63]}""");
	}

	[Fact]
	public async Task CreateAsync_SendsEveryOwnAttribute()
	{
		var request = new IncidentCreateRequest
		{
			Name = "i",
			StartDate = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
			EndDate = new DateTimeOffset(2014, 10, 9, 20, 44, 35, TimeSpan.Zero),
			Victims = [13],
			Actors = [223],
			SandboxReports = [31]
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.CreateAsync(request, ct), IncidentJson);

		call.ShouldBe(
			HttpMethod.Post,
			"/api/v1/incident/",
			body: """{"name":"i","start_date":"2014-10-02T20:44:35Z","end_date":"2014-10-09T20:44:35Z","victims":[13],"actors":[223],"sandbox_reports":[31]}""");
	}

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
		var call = await TestClient.CaptureAsync((client, ct) => client.Incidents.ListAssociatedWithValueAsync("1.2.3.4", null, ct), Page(IncidentJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/incident/associated_with_intelligence/", "?value=1.2.3.4");
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
