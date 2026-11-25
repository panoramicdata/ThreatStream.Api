using ThreatStream.Api.Models;

namespace ThreatStream.Api.IntegrationTest.SsoHistory;

/// <summary>The SSO history needs an account with the Org Admin permission.</summary>
[Collection(ThreatStreamTestGroup.Name)]
public class SsoHistoryIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsActivity()
	{
		var page = await fixture.Client.SsoHistory.ListAsync(new ListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Should().AllSatisfy(a => a.Id.Should().BePositive());
	}
}
