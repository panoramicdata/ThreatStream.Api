namespace ThreatStream.Api.IntegrationTest;

/// <summary>
/// Shares one <see cref="ThreatStreamFixture"/> across every integration test class. Collections run one at a time
/// (<c>xunit.runner.json</c>), which also keeps the tests inside ThreatStream's per-user rate limits.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ThreatStreamTestGroup : ICollectionFixture<ThreatStreamFixture>
{
	internal const string Name = "ThreatStream";
}
