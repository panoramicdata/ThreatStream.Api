using ThreatStream.Api.Models.Sandbox;

namespace ThreatStream.Api.IntegrationTest.Sandbox;

/// <summary>Sandbox detonations are only searched here; submissions are never run live (docs/TESTING.md).</summary>
[Collection(ThreatStreamTestGroup.Name)]
public class SandboxIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task SearchAsync_ReturnsDetonations()
	{
		var page = await fixture.Client.SandboxSubmissions.SearchAsync(new SandboxSearchOptions { Limit = 5 }, TestContext.Current.CancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Count.Should().BeLessThanOrEqualTo(5);
	}
}
