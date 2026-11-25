using ThreatStream.Api.Models.Feeds;

namespace ThreatStream.Api.IntegrationTest.Feeds;

[Collection(ThreatStreamTestGroup.Name)]
public class FeedsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsFeeds()
	{
		var page = await fixture.Client.Feeds.ListAsync(new FeedListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.Objects.Count.Should().BeLessThanOrEqualTo(5);
		page.Objects.Should().AllSatisfy(f =>
		{
			f.Id.Should().BePositive();
			f.Name.Should().NotBeNullOrEmpty();
		});
	}
}
