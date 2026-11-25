namespace ThreatStream.Api.IntegrationTest.Confidence;

[Collection(ThreatStreamTestGroup.Name)]
public class ConfidenceIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task GetTrendAsync_ReturnsScoresWithinRange()
	{
		// A public resolver: ThreatStream may or may not hold intelligence for it, so the scores may be null.
		var trend = await fixture.Client.Confidence.GetTrendAsync("8.8.8.8", TestContext.Current.CancellationToken);

		foreach (var score in new[] { trend.MinConfidence, trend.AverageConfidence, trend.MaxConfidence })
		{
			score.GetValueOrDefault().Should().BeInRange(0, 100);
		}

		if (trend.MinConfidence is { } min && trend.MaxConfidence is { } max)
		{
			min.Should().BeLessThanOrEqualTo(max);
		}
	}

	[Fact]
	public async Task WrongApiKey_RaisesUnauthorized()
	{
		using var client = new ThreatStreamClient(fixture.CreateOptions(o => o.ApiKey = "definitely-not-the-key"));

		var act = () => client.Confidence.GetTrendAsync("8.8.8.8", TestContext.Current.CancellationToken);

		(await act.Should().ThrowAsync<ThreatStreamApiException>()).Which.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Unauthorized, System.Net.HttpStatusCode.Forbidden);
	}
}
