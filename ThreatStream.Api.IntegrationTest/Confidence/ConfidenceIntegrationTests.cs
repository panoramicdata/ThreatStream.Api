using System.Diagnostics.CodeAnalysis;

namespace ThreatStream.Api.IntegrationTest.Confidence;

[Collection(ThreatStreamTestGroup.Name)]
[SuppressMessage("Security", "S1313:Do not hardcode IP addresses", Justification = "A well-known public resolver, looked up as threat intelligence data; nothing connects to it. A documentation-range address would never have intelligence to score.")]
public class ConfidenceIntegrationTests(ThreatStreamFixture fixture)
{
	// A public resolver: ThreatStream may or may not hold intelligence for it, so the scores may be null.
	private const string PublicResolver = "8.8.8.8";

	[Fact]
	public async Task GetTrendAsync_ReturnsScoresWithinRange()
	{
		var trend = await fixture.Client.Confidence.GetTrendAsync(PublicResolver, TestContext.Current.CancellationToken);

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

		var act = () => client.Confidence.GetTrendAsync(PublicResolver, TestContext.Current.CancellationToken);

		(await act.Should().ThrowAsync<ThreatStreamApiException>()).Which.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Unauthorized, System.Net.HttpStatusCode.Forbidden);
	}
}
