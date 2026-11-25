namespace ThreatStream.Api.IntegrationTest.ObservableImports;

/// <summary>
/// Import sessions are read-only here; imports and approvals are never run live (docs/TESTING.md).
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class ImportsIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ImportSessions_GetAsync_UnknownSession_RaisesNotFound()
	{
		var act = () => fixture.Client.ImportSessions.GetAsync(1, TestContext.Current.CancellationToken);

		(await act.Should().ThrowAsync<ThreatStreamApiException>()).Which.StatusCode
			.Should().BeOneOf(System.Net.HttpStatusCode.NotFound, System.Net.HttpStatusCode.Forbidden);
	}
}
