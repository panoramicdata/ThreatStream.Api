using ThreatStream.Api.Models;

namespace ThreatStream.Api.IntegrationTest.UserAdministration;

/// <summary>
/// Reads the organisation's users (needs an Org Admin account). Creating, changing, unlocking and deleting users are
/// never run live. Assertions name single fields only: a failing structural assertion would print a user object, which
/// can carry an API key.
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class OrgAdminUsersIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task ListAsync_ThenGetAsync_ReturnsTheSameUser()
	{
		var cancellationToken = TestContext.Current.CancellationToken;

		var page = await fixture.Client.OrgAdminUsers.ListAsync(new ListOptions { Limit = 1 }, cancellationToken);
		var listed = page.Objects.Should().ContainSingle("the account running the tests is itself a user").Subject;
		var user = await fixture.Client.OrgAdminUsers.GetAsync(listed.UserId, cancellationToken);

		listed.UserId.Should().BePositive();
		user.UserId.Should().Be(listed.UserId);
		user.Email.Should().Be(listed.Email);
	}
}
