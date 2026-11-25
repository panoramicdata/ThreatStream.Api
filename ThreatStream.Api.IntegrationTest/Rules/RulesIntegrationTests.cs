using ThreatStream.Api.Models.Rules;

namespace ThreatStream.Api.IntegrationTest.Rules;

[Collection(ThreatStreamTestGroup.Name)]
public class RulesIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsAPageOfRules()
	{
		// An organization may have no rules, so only the shape of the page is checked.
		var page = await fixture.Client.Rules.ListAsync(new RuleListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.ShouldBeAPageOf(5, r =>
		{
			r.Id.Should().BePositive();
			r.Name.Should().NotBeNullOrWhiteSpace();
		});
	}
}
