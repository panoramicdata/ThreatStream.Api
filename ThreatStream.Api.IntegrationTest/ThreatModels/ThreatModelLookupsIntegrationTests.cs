using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.IntegrationTest.ThreatModels;

/// <summary>
/// Read-only checks of the lookup lists the threat model types refer to by ID. These endpoints are inferred from the API
/// reference (named, never documented), so these tests are the first to show their real shape.
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class ThreatModelLookupsIntegrationTests(ThreatStreamFixture fixture)
{
	private static CancellationToken Token => TestContext.Current.CancellationToken;

	[Fact]
	public async Task VictimTypes_ListAsync_ReturnsValues()
		=> ShouldBeLookups(await fixture.Client.VictimTypes.ListAsync(null, Token));

	[Fact]
	public async Task ActorSophisticationTypes_ListAsync_ReturnsValues()
		=> ShouldBeLookups(await fixture.Client.ActorSophisticationTypes.ListAsync(null, Token));

	[Fact]
	public async Task CampaignStatuses_ListAsync_ReturnsValues()
		=> ShouldBeLookups(await fixture.Client.CampaignStatuses.ListAsync(null, Token));

	[Fact]
	public async Task IncidentStatusTypes_ListAsync_ReturnsValues()
		=> ShouldBeLookups(await fixture.Client.IncidentStatusTypes.ListAsync(null, Token));

	private static void ShouldBeLookups(ThreatStreamPage<ThreatModelReference> page)
	{
		page.Objects.Should().NotBeEmpty("every deployment has built-in values");
		page.Objects.Should().OnlyContain(v => v.Id > 0 && (v.Value ?? v.DisplayName ?? v.Name) != null);
	}
}
