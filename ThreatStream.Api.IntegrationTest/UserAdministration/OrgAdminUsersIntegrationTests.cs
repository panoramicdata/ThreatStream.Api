using ThreatStream.Api.Models;

namespace ThreatStream.Api.IntegrationTest.UserAdministration;

/// <summary>
/// Reads the organisation's users (needs an Org Admin account). Creating, changing, unlocking and deleting users are
/// never run live. Assertions are made on user IDs only, never on user objects: a failing assertion prints its subject,
/// and a user object can carry an API key and colleagues' details.
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class OrgAdminUsersIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ThenGetAsync_ReturnsTheSameUser()
	{
		var cancellationToken = TestContext.Current.CancellationToken;

		var page = await fixture.Client.OrgAdminUsers.ListAsync(new ListOptions { Limit = 1 }, cancellationToken);
		var userId = page.Objects.Select(u => u.UserId).Should().ContainSingle("the account running the tests is itself a user").Subject;
		var user = await fixture.Client.OrgAdminUsers.GetAsync(userId, cancellationToken);

		userId.Should().BePositive();
		user.UserId.Should().Be(userId);
	}
}
