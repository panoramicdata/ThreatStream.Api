namespace ThreatStream.Api.IntegrationTest.ThreatModelsB;

[Collection(ThreatStreamTestGroup.Name)]
public class TtpsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public Task ListAsync_ThenGetAsync_ReadTheSameEntity()
		=> ThreatModelsBReads.ListThenGetAsync(
			(options, ct) => fixture.Client.Ttps.ListAsync(options, ct),
			(id, ct) => fixture.Client.Ttps.GetAsync(id, null, ct),
			"ttp");
}
