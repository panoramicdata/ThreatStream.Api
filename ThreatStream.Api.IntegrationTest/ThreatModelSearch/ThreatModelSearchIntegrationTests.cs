using ThreatStream.Api.Models.ThreatModelSearch;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.IntegrationTest.ThreatModelSearch;

[Collection(ThreatStreamTestGroup.Name)]
public class ThreatModelSearchIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task SearchAsync_ByModelType_ReturnsOnlyThatType()
	{
		var page = await fixture.Client.ThreatModelSearch.SearchAsync(
			new ThreatModelSearchOptions { ModelType = [ThreatModelType.Actor], Limit = 5 },
			TestContext.Current.CancellationToken);

		page.Objects.Count.Should().BeLessThanOrEqualTo(5);
		page.Objects.Should().AllSatisfy(r =>
		{
			r.Id.Should().BePositive();
			r.ModelType.Should().Be(ThreatModelType.Actor);
		});
	}
}
