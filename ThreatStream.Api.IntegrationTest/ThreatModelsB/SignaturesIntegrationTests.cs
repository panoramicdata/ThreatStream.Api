using ThreatStream.Api.Models;

namespace ThreatStream.Api.IntegrationTest.ThreatModelsB;

[Collection(ThreatStreamTestGroup.Name)]
public class SignaturesIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public Task ListAsync_ThenGetAsync_ReadTheSameEntity()
		=> ThreatModelsBReads.ListThenGetAsync(
			(options, ct) => fixture.Client.Signatures.ListAsync(options, ct),
			(id, ct) => fixture.Client.Signatures.GetAsync(id, null, ct),
			"signature");

	[Fact]
	public async Task ListTypesAsync_IncludesSnortAndYara()
	{
		// Inferred endpoint: the reference names it but does not document it.
		var page = await fixture.Client.Signatures.ListTypesAsync(new ListOptions { Limit = 0 }, TestContext.Current.CancellationToken);

		page.Objects.Select(t => t.Name).Should().Contain(["Snort", "YARA"]);
		page.Objects.Should().OnlyContain(t => t.Id > 0);
	}
}
