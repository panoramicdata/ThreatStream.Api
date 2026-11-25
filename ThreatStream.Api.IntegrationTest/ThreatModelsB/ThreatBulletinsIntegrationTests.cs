namespace ThreatStream.Api.IntegrationTest.ThreatModelsB;

[Collection(ThreatStreamTestGroup.Name)]
public class ThreatBulletinsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public Task ListAsync_ThenGetAsync_ReadTheSameEntity()
		=> ThreatModelsBReads.ListThenGetAsync(
			(options, ct) => fixture.Client.ThreatBulletins.ListAsync(options, ct),
			(id, ct) => fixture.Client.ThreatBulletins.GetAsync(id, null, ct),
			"tipreport");
}
