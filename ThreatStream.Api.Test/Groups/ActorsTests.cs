using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class ActorsTests
{
	private static readonly string ActorJson = Entity(223, "actor", """
		"aliases": ["Fancy Bear", {"id": 9, "name": "APT28"}],
		"is_team": "true",
		"soph_type": {"id": 3, "value": "Expert", "display_name": "Expert", "resource_uri": "/api/v1/actorsophisticationtype/3/"},
		"soph_desc": "Custom tooling",
		"start_date": "2014-10-02T20:44:35",
		"victims": [13, {"id": 15, "value": "Insurance", "resource_uri": "/api/v1/victimtype/15/"}],
		"campaigns": [1062],
		"incidents": [79]
		""");

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.ListAsync(null, ct), Page(ActorJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/");
	}

	[Fact]
	public async Task ListAsync_SendsEveryFilter()
	{
		var options = new ThreatModelListOptions
		{
			Limit = 10,
			Offset = 20,
			OrderBy = "-modified_ts",
			IdIn = [223, 224],
			Name = "test actor",
			NameContains = "test",
			PublicationStatus = ThreatModelPublicationStatus.PendingReview,
			ModifiedFrom = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
			ModifiedBefore = new DateTimeOffset(2026, 1, 3, 4, 4, 5, TimeSpan.FromHours(1))
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.ListAsync(options, ct), EmptyPage);

		call.ShouldBe(
			HttpMethod.Get,
			"/api/v1/actor/",
			"?id__in=223%2C224&name=test%20actor&name__contains=test&publication_status=pending_review&modified_ts__gte=2026-01-02T03%3A04%3A05Z&modified_ts__lt=2026-01-03T03%3A04%3A05Z&limit=10&offset=20&order_by=-modified_ts");
	}

	[Fact]
	public async Task ListAsync_MapsThePage()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Actors.ListAsync(null, ct), Page(ActorJson));

		page.Meta!.TotalCount.Should().Be(1);
		page.Objects.Should().ContainSingle().Which.Id.Should().Be(223);
	}

	private static async Task CreateShouldSendAsync(ActorCreateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.CreateAsync(request, ct), ActorJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/actor/", body: body);
	}

	[Fact]
	public Task CreateAsync_SendsPostWithJsonBody()
		=> CreateShouldSendAsync(
			new ActorCreateRequest
			{
				Name = "test actor",
				Tlp = Tlp.Red,
				Tags = ["bad actor", "dummy tag"],
				Victims = [13, 15],
				PublicationStatus = ThreatModelPublicationStatus.Published
			},
			"""{"name":"test actor","victims":[13,15],"tags":["bad actor","dummy tag"],"tlp":"red","publication_status":"published"}""");

	[Fact]
	public Task CreateAsync_SendsEveryAttribute()
		=> CreateShouldSendAsync(
			new ActorCreateRequest
			{
				Name = "test actor",
				Description = "d",
				Tags = ["t"],
				TagsV2 = [new ThreatStreamTag { Name = "p", Tlp = Tlp.Red }],
				Tlp = Tlp.Green,
				IsPublic = false,
				Circles = [101],
				PublicationStatus = ThreatModelPublicationStatus.New,
				SourceCreated = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
				SourceModified = new DateTimeOffset(2014, 10, 3, 20, 44, 35, TimeSpan.Zero),
				Intelligence = [165546],
				Signatures = [5],
				Ttps = [63],
				Aliases = "APT28",
				IsTeam = true,
				SophisticationType = 3,
				SophisticationDescription = "s",
				StartDate = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
				Victims = [13],
				Campaigns = [1062],
				Incidents = [79]
			},
			"""{"name":"test actor","aliases":"APT28","is_team":true,"soph_type":3,"soph_desc":"s","start_date":"2014-10-02T20:44:35Z","victims":[13],"campaigns":[1062],"incidents":[79],"description":"d","tags":["t"],"tags_v2":[{"name":"p","tlp":"red"}],"tlp":"green","is_public":false,"circles":[101],"publication_status":"new","source_created":"2014-10-02T20:44:35Z","source_modified":"2014-10-03T20:44:35Z","intelligence":[165546],"signatures":[5],"ttps":[63]}""");

	[Fact]
	public async Task GetAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.GetAsync(223, null, ct), ActorJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/223/");
	}

	[Fact]
	public async Task GetAsync_SendsTheOnPremOptions()
	{
		var options = new ThreatModelDetailOptions { SkipAssociations = true, SkipIntelligence = false, RemoteApi = true };

		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.GetAsync(253, options, ct), ActorJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/253/", "?skip_associations=true&skip_intelligence=false&remote_api=true");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var actor = await TestClient.ReadAsync((client, ct) => client.Actors.GetAsync(223, null, ct), ActorJson);

		actor.ShouldHaveEntityFields(223, "actor");
		actor.Aliases.Select(a => (a.Id, a.Name)).Should().Equal((null, "Fancy Bear"), (9L, "APT28"));
		actor.IsTeam.Should().BeTrue();
		actor.SophisticationType!.Value.Should().Be("Expert");
		actor.SophisticationType.DisplayName.Should().Be("Expert");
		actor.SophisticationDescription.Should().Be("Custom tooling");
		actor.StartDate.Should().Be(new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero));
		actor.Victims.Select(v => (v.Id, v.Value)).Should().Equal((13L, null), (15L, "Insurance"));
		actor.Campaigns.Should().ContainSingle().Which.Id.Should().Be(1062);
		actor.Incidents.Should().ContainSingle().Which.Id.Should().Be(79);
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithJsonBody()
	{
		var request = new ActorUpdateRequest { Name = "another test actor", Tlp = Tlp.Amber, Victims = [13] };

		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.UpdateAsync(223, request, ct), ActorJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/actor/223/", body: """{"victims":[13],"name":"another test actor","tlp":"amber"}""");
	}

	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Actors.CreateAsync(new ActorCreateRequest { Name = "x" }, ct), "POST", "/api/v1/actor/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Actors.UpdateAsync(223, new ActorUpdateRequest { Name = "x" }, ct), "PATCH", "/api/v1/actor/223/");

	[Fact]
	public async Task ListAssociatedWithValueAsync_SendsTheValue()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.ListAssociatedWithValueAsync("google.com", null, ct), Page(ActorJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/associated_with_intelligence/", "?value=google.com");
	}

	[Fact]
	public async Task ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.ListAssociatedWithIdsAsync([1, 2, 3], Paging, ct), Page(ActorJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/associated_with_intelligence/", "?ids=1%2C2%2C3&limit=10&offset=20");
	}

	[Fact]
	public async Task ListLinkedAsync_SendsTheEntityTypeInThePath()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.ListLinkedAsync(234673, ThreatModelType.Malware, Paging, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/234673/malware/", PagingQuery);
	}

	[Fact]
	public async Task ListIntelligenceAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Actors.ListIntelligenceAsync(223, null, ct), Page(ObservableJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/actor/223/intelligence/");
	}

	[Fact]
	public async Task ListIntelligenceAsync_MapsTheObservables()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Actors.ListIntelligenceAsync(223, null, ct), Page(ObservableJson));

		page.ShouldBeTheExampleObservable();
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.Actors.GetAsync(1, null, ct));
}
