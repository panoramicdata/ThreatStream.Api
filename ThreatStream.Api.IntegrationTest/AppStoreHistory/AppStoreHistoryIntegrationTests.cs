using ThreatStream.Api.Models;

namespace ThreatStream.Api.IntegrationTest.AppStoreHistory;

[Collection(ThreatStreamTestGroup.Name)]
public class AppStoreHistoryIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsActivity()
	{
		var page = await fixture.Client.AppStoreHistory.ListAsync(new ListOptions { Limit = 5 }, TestContext.Current.CancellationToken);


		page.ShouldBeAPageOf(5, a => a.Id.Should().BePositive());

	}
}
