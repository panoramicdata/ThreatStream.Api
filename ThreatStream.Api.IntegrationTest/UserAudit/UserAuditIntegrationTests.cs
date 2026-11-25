using ThreatStream.Api.Models.UserAudit;

namespace ThreatStream.Api.IntegrationTest.UserAudit;

/// <summary>
/// Reads a one-record UI activity report synchronously (needs Org Admin). Asynchronous report jobs are not started live:
/// each one creates a job on the shared account.
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class UserAuditIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task UserActions_GetReport_ReturnsJson()
	{
		var options = new UserActivityReportOptions
		{
			CreatedFrom = DateTimeOffset.UtcNow.AddDays(-1),
			Format = UserActivityReportFormat.Json,
			Limit = 1,
			OrderBy = "-created_ts"
		};

		var report = await fixture.Client.UserActions.GetReportAsync(options, TestContext.Current.CancellationToken);

		report.TrimStart().Should().MatchRegex(@"^[\[{]");
	}
}
