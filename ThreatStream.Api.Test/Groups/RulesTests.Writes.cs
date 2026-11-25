using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Rules;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public partial class RulesTests
{
	// The reference's create example, without its stray "keyword" field.
	private static readonly RuleCreateRequest ExampleRule = new() { Name = "test_rule4", Keywords = ["key", "foobar"], MatchTips = true };

	private static readonly RuleUpdateRequest NewTag = new() { Tags = [new ThreatStreamTag { Name = "new_tag", Tlp = Tlp.White }] };

	[Fact]
	public async Task CreateAsync_PostsTheRule()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.CreateAsync(ExampleRule, ct), RulesTestKit.RuleJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/rule/", body: """{"name":"test_rule4","keywords":["key","foobar"],"match_tips":true}""");
	}

	[Fact]
	public async Task CreateAsync_SendsEveryField()
	{
		var request = new RuleCreateRequest
		{
			Name = "r",
			Keywords = ["evil.example"],
			MatchObservables = true,
			MatchReportedFiles = false,
			MatchSignatures = true,
			MatchTips = false,
			MatchVulnerabilities = true,
			MatchImpacts = ["actor_ipv6"],
			ExcludeImpacts = ["mal_domain"],
			CreateInvestigation = true,
			ExcludeNotifyOrgWhitelisted = true,
			ExcludeNotifyOwnerOrg = false,
			Tags = [new ThreatStreamTag { Name = "t", Tlp = Tlp.Red }],
			Actors = [1],
			Campaigns = [2],
			Incidents = [3],
			Malware = [4],
			Signatures = [5],
			Tips = [6],
			Ttps = [7],
			Vulnerabilities = [8]
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.CreateAsync(request, ct), RulesTestKit.RuleJson);

		call.ShouldBe(
			HttpMethod.Post,
			"/api/v1/rule/",
			body: """{"name":"r","keywords":["evil.example"],"actors":[1],"campaigns":[2],"incidents":[3],"malware":[4],"signatures":[5],"tips":[6],"ttps":[7],"vulnerabilities":[8],"match_observables":true,"match_reportedfiles":false,"match_signatures":true,"match_tips":false,"match_vulnerabilities":true,"match_impacts":["actor_ipv6"],"exclude_impacts":["mal_domain"],"create_investigation":true,"exclude_notify_org_whitelisted":true,"exclude_notify_owner_org":false,"tags":[{"name":"t","tlp":"red"}]}""");
	}

	[Fact]
	public async Task CreateAsync_MapsTheRule()
	{
		var rule = await TestClient.ReadAsync((client, ct) => client.Rules.CreateAsync(ExampleRule, ct), RulesTestKit.RuleJson);

		ShouldBeTheExampleRule(rule!);
	}

	[Fact]
	public async Task UpdateAsync_PatchesTheRule()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.UpdateAsync(56, NewTag, ct), RulesTestKit.RuleJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/rule/56/", body: """{"tags":[{"name":"new_tag","tlp":"white"}]}""");
	}

	[Fact]
	public async Task UpdateAsync_SendsNameAndKeywords()
	{
		var request = new RuleUpdateRequest { Name = "renamed", Keywords = ["foobar"] };

		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.UpdateAsync(56, request, ct), RulesTestKit.RuleJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/rule/56/", body: """{"name":"renamed","keywords":["foobar"]}""");
	}

	[Fact]
	public async Task UpdateAsync_AcceptedWithoutABody_ReturnsNull()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.Accepted, string.Empty);
		using var client = TestClient.Create(stub);

		var rule = await client.Rules.UpdateAsync(56, NewTag, TestContext.Current.CancellationToken);

		rule.Should().BeNull();
	}

	[Fact]
	public async Task DeleteAsync_SendsDeleteWithAnIdFilter()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.DeleteAsync(210, ct), string.Empty);

		call.ShouldBe(HttpMethod.Delete, "/api/v1/rule/", "?id__in=210");
	}

	[Fact]
	public Task CreateAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Rules.CreateAsync(ExampleRule, ct), "POST", "api/v1/rule/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Rules.UpdateAsync(56, NewTag, ct), "PATCH", "api/v1/rule/56/");

	[Fact]
	public Task DeleteAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Rules.DeleteAsync(210, ct), "DELETE", "api/v1/rule/");

	[Fact]
	public Task CreateAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Rules.CreateAsync(ExampleRule, ct),
			HttpStatusCode.BadRequest,
			"""{"error": "Organizations can configure up to 300 rules"}""",
			"Organizations can configure up to 300 rules");
}
