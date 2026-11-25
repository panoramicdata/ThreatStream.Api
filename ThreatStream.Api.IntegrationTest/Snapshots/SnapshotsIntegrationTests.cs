using ThreatStream.Api.Models.Snapshots;

namespace ThreatStream.Api.IntegrationTest.Snapshots;

/// <summary>Snapshots are only listed here; creating one is never run live (docs/TESTING.md).</summary>
[Collection(ThreatStreamTestGroup.Name)]
public class SnapshotsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsSnapshots()
	{
		var page = await fixture.Client.Snapshots.ListAsync(new SnapshotListOptions { Limit = 5, OrderBy = "-id" }, TestContext.Current.CancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Should().AllSatisfy(s => s.Id.Should().BePositive());
	}
}
