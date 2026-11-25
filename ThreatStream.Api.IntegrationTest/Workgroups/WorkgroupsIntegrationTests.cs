namespace ThreatStream.Api.IntegrationTest.Workgroups;

[Collection(ThreatStreamTestGroup.Name)]
public class WorkgroupsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsAPageOfWorkgroups()
	{
		// An organization may have no workgroups, so only the shape of the page is checked.
		var page = await fixture.Client.Workgroups.ListAsync(new Models.ListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Count.Should().BeLessThanOrEqualTo(5);
		page.Objects.Should().AllSatisfy(w =>
		{
			w.Id.Should().BePositive();
			w.Name.Should().NotBeNullOrWhiteSpace();
		});
	}
}
