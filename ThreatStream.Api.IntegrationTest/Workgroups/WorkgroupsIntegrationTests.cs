namespace ThreatStream.Api.IntegrationTest.Workgroups;

[Collection(ThreatStreamTestGroup.Name)]
public class WorkgroupsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsAPageOfWorkgroups()
	{
		// An organization may have no workgroups, so only the shape of the page is checked.
		var page = await fixture.Client.Workgroups.ListAsync(new Models.ListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.ShouldBeAPageOf(5, w =>
		{
			w.Id.Should().BePositive();
			w.Name.Should().NotBeNullOrWhiteSpace();
		});
	}
}
