using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Rules;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public partial class RulesTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.ListAsync(null, ct), RulesTestKit.PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/rule/");
	}

	[Fact]
	public async Task ListAsync_SendsRemoteApiAndOrdering()
	{
		var options = new RuleListOptions { RemoteApi = true, OrderBy = "-created_ts", Limit = 20 };

		var call = await TestClient.CaptureAsync((client, ct) => client.Rules.ListAsync(options, ct), RulesTestKit.PageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/rule/", "?remote_api=true&limit=20&order_by=-created_ts");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Rules.ListAsync(null, ct), RulesTestKit.PageJson);

		ShouldBeTheExampleRule(page.Objects.Should().ContainSingle().Subject);
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Rules.ListAsync(null, ct),
			HttpStatusCode.Unauthorized,
			"""{"message": "Invalid credentials"}""",
			"Invalid credentials");

	private static void ShouldBeTheExampleRule(Rule rule)
	{
		rule.Id.Should().Be(56);
		rule.ResourceUri.Should().Be("/api/v1/rule/56/");
		rule.Name.Should().Be("test_rule4");
		rule.Keywords.Should().Equal("key", "foobar");
		rule.Matches.Should().Be(17);
		rule.Tags.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Name = "new_tag", Tlp = Tlp.White });
		rule.HasAssociations.Should().BeTrue();
		rule.CreateInvestigation.Should().BeTrue();
		rule.Investigation!.Id.Should().Be(901);
		rule.Investigation.Name.Should().Be("test_rule4 investigation");
		rule.Investigation.ResourceUri.Should().Be("/api/v1/investigation/901/");
		rule.CreatedTs.Should().Be(new DateTimeOffset(2024, 3, 1, 10, 15, 30, TimeSpan.Zero).AddTicks(1234560));
		rule.ModifiedTs.Should().Be(new DateTimeOffset(2024, 3, 2, 11, 0, 0, TimeSpan.Zero));
		rule.OrgId.Should().Be(7);
		rule.Organization!.Id.Should().Be(7);
		rule.Organization.Name.Should().Be("Example Org");
		rule.UserId.Should().Be(42);
		rule.RemoteApi.Should().BeFalse();
		rule.MatchObservables.Should().BeTrue();
		rule.MatchReportedFiles.Should().BeFalse();
		rule.MatchSignatures.Should().BeTrue();
		rule.MatchTips.Should().BeTrue();
		rule.MatchVulnerabilities.Should().BeFalse();
		rule.ExcludeNotifyOrgWhitelisted.Should().BeTrue();
		rule.ExcludeNotifyOwnerOrg.Should().BeFalse();
		rule.AdditionalProperties.Keys.Should().BeEquivalentTo("match_actors", "org_shared", "user");
	}
}
