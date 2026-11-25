using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;

namespace ThreatStream.Api.IntegrationTest.Intelligence;

[Collection(ThreatStreamTestGroup.Name)]
public class IntelligenceIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task Observables_ListAsync_ReturnsActiveObservables()
	{
		var page = await fixture.Client.Observables.ListAsync(
			new ObservableListOptions { Status = "active", Limit = 5 },
			TestContext.Current.CancellationToken);

		page.Objects.Count.Should().BeLessThanOrEqualTo(5);
		page.Objects.Should().AllSatisfy(o =>
		{
			o.Id.Should().BePositive();
			o.Value.Should().NotBeNullOrEmpty();
			o.Status.Should().Be("active");
		});
	}

	[Fact]
	public async Task SearchFilters_ListAsync_ReturnsSavedSearches()
	{
		var page = await fixture.Client.SearchFilters.ListAsync(new ListOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Should().AllSatisfy(f => f.Id.Should().BePositive());
	}
}
