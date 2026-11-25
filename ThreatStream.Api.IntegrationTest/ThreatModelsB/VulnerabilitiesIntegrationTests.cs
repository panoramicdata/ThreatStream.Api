namespace ThreatStream.Api.IntegrationTest.ThreatModelsB;

[Collection(ThreatStreamTestGroup.Name)]
public class VulnerabilitiesIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public Task ListAsync_ThenGetAsync_ReadTheSameEntity()
		=> ThreatModelsBReads.ListThenGetAsync(
			(options, ct) => fixture.Client.Vulnerabilities.ListAsync(options, ct),
			(id, ct) => fixture.Client.Vulnerabilities.GetAsync(id, null, ct),
			"vulnerability");
}
