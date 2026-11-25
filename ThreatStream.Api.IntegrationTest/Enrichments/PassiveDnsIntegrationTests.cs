using ThreatStream.Api.Models.Enrichments;

namespace ThreatStream.Api.IntegrationTest.Enrichments;

[Collection(ThreatStreamTestGroup.Name)]
public class PassiveDnsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task GetAsync_ReturnsResolutionsOfAWellKnownDomain()
	{
		// Needs a passive DNS provider subscription; a widely resolved domain should have at least one resolution.
		var result = await fixture.Client.PassiveDns.GetAsync(PassiveDnsLookupType.Domain, "example.com", TestContext.Current.CancellationToken);

		result.Results.Should().AllSatisfy(r => r.Domain.Should().NotBeNullOrEmpty());
	}
}
