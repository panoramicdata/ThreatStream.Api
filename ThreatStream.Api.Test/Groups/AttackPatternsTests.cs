using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class AttackPatternsTests
{
	private static readonly string AttackPatternJson = Entity(27, "attackpattern", """
		"aliases": ["Spearphishing Attachment"],
		"assignee_user": 7,
		"body": "Adversaries send **attachments**.",
		"body_content_type": "markdown",
		"can_add_public_tags": true,
		"embedded_content_type": "",
		"embedded_content_url": null,
		"external_references": [{"source_name": "mitre-attack", "external_id": "T1566.001", "url": "https://attack.mitre.org/techniques/T1566/001"}],
		"is_mitre": "1",
		"kill_chain_phases": {"lockheed-martin": ["delivery", "exploitation"]},
		"killchainstages": ["delivery"],
		"votes": [3, 1],
		"actors": [223],
		"attackpatterns": [28],
		"campaigns": [1062],
		"courseofaction": [5],
		"identities": [6],
		"incidents": [79],
		"infrastructure": [8],
		"intelligence_initiatives": [{"id": 9, "name": "Initiative"}],
		"intrusionsets": [10],
		"malwares": [11],
		"tipreports": [12],
		"tools": [13],
		"vulnerability": [14]
		""");

	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.ListAsync(new ThreatModelListOptions { Limit = 10 }, ct), Page(AttackPatternJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/attackpattern/", "?limit=10");
	}

	private static async Task CreateShouldSendAsync(AttackPatternCreateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.CreateAsync(request, ct), AttackPatternJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/attackpattern/", body: body);
	}

	[Fact]
	public Task CreateAsync_SendsPostWithJsonBody()
		=> CreateShouldSendAsync(
			new AttackPatternCreateRequest { Name = "dummy test attack pattern", Tlp = Tlp.Red, Tags = ["attack pattern", "example tag"], Ttps = [63] },
			"""{"name":"dummy test attack pattern","tags":["attack pattern","example tag"],"tlp":"red","ttps":[63]}""");

	[Fact]
	public Task CreateAsync_SendsEveryOwnAttribute()
		=> CreateShouldSendAsync(
			new AttackPatternCreateRequest
			{
				Name = "p",
				Aliases = "a",
				AssigneeUser = 7,
				Body = "b",
				BodyContentType = ThreatModelBodyContentType.RichText,
				KillChainStages = ["delivery"],
				Actors = [1],
				Campaigns = [2],
				CoursesOfAction = [3],
				Identities = [4],
				Incidents = [5],
				Infrastructure = [6],
				IntrusionSets = [7],
				ThreatBulletins = [8],
				Vulnerabilities = [9]
			},
			"""{"name":"p","aliases":"a","assignee_user":7,"body":"b","body_content_type":"richtext","killchainstages":["delivery"],"actors":[1],"campaigns":[2],"courseofaction":[3],"identities":[4],"incidents":[5],"infrastructure":[6],"intrusionsets":[7],"tipreports":[8],"vulnerability":[9]}""");

	[Fact]
	public async Task GetAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.GetAsync(27, new ThreatModelDetailOptions { RemoteApi = true }, ct), AttackPatternJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/attackpattern/27/", "?remote_api=true");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var pattern = await TestClient.ReadAsync((client, ct) => client.AttackPatterns.GetAsync(27, null, ct), AttackPatternJson);

		pattern.ShouldHaveEntityFields(27, "attackpattern");
		pattern.Aliases.Should().ContainSingle().Which.Name.Should().Be("Spearphishing Attachment");
		pattern.AssigneeUser!.Id.Should().Be(7);
		pattern.Body.Should().Be("Adversaries send **attachments**.");
		pattern.BodyContentType.Should().Be(ThreatModelBodyContentType.Markdown);
		pattern.CanAddPublicTags.Should().BeTrue();
		pattern.EmbeddedContentType.Should().BeEmpty();
		pattern.EmbeddedContentUrl.Should().BeNull();
		pattern.ExternalReferences.Should().ContainSingle().Which.AdditionalProperties["external_id"].GetString().Should().Be("T1566.001");
		pattern.IsMitre.Should().BeTrue();
		pattern.KillChainPhases!.Value.GetProperty("lockheed-martin")[1].GetString().Should().Be("exploitation");
		pattern.KillChainStages.Should().ContainSingle().Which.Name.Should().Be("delivery");
		pattern.Votes!.Value.GetArrayLength().Should().Be(2);
		new[]
		{
			pattern.Actors, pattern.AttackPatterns, pattern.Campaigns, pattern.CoursesOfAction, pattern.Identities, pattern.Incidents,
			pattern.Infrastructure, pattern.IntelligenceInitiatives, pattern.IntrusionSets, pattern.Malware, pattern.ThreatBulletins,
			pattern.Tools, pattern.Vulnerabilities
		}.Select(list => list.Single().Id).Should().Equal(223, 28, 1062, 5, 6, 79, 8, 9, 10, 11, 12, 13, 14);
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithJsonBody()
	{
		var request = new AttackPatternUpdateRequest { Name = "another dummy test attack pattern", Tlp = Tlp.Amber };

		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.UpdateAsync(27, request, ct), AttackPatternJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/attackpattern/27/", body: """{"name":"another dummy test attack pattern","tlp":"amber"}""");
	}

	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.AttackPatterns.CreateAsync(new AttackPatternCreateRequest { Name = "x" }, ct), "POST", "/api/v1/attackpattern/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.AttackPatterns.UpdateAsync(27, new AttackPatternUpdateRequest(), ct), "PATCH", "/api/v1/attackpattern/27/");

	[Fact]
	public async Task ListAssociatedWithValueAsync_SendsTheValue()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.ListAssociatedWithValueAsync("google.com", Paging, ct), Page(AttackPatternJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/attackpattern/associated_with_intelligence/", "?value=google.com&limit=10&offset=20");
	}

	[Fact]
	public async Task ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.ListAssociatedWithIdsAsync([165546], null, ct), Page(AttackPatternJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/attackpattern/associated_with_intelligence/", "?ids=165546");
	}

	[Fact]
	public async Task ListLinkedAsync_SendsTheEntityTypeInThePath()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.ListLinkedAsync(234673, ThreatModelType.Vulnerability, null, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/attackpattern/234673/vulnerability/");
	}

	[Fact]
	public async Task ListIntelligenceAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.AttackPatterns.ListIntelligenceAsync(27, Paging, ct), Page(ObservableJson));

		call.ShouldBe(HttpMethod.Get, "/api/v1/attackpattern/27/intelligence/", PagingQuery);
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.AttackPatterns.GetAsync(1, null, ct));
}
