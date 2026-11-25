using ThreatStream.Api.Models.Whitelist;

namespace ThreatStream.Api.IntegrationTest.Whitelist;

[Collection(ThreatStreamTestGroup.Name)]
public class WhitelistIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ReturnsAPageOfEntries()
	{
		// The whitelist may be empty, so only the shape of the page is checked.
		var page = await fixture.Client.Whitelist.ListAsync(new WhitelistListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Count.Should().BeLessThanOrEqualTo(5);
		page.Objects.Should().AllSatisfy(e =>
		{
			e.Id.Should().BePositive();
			e.Value.Should().NotBeNullOrEmpty();
		});
	}

	[Fact]
	public async Task ExportCsvAsync_ReturnsText()
	{
		var csv = await fixture.Client.Whitelist.ExportCsvAsync(new WhitelistListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		csv.Should().NotBeNull();
	}
}
