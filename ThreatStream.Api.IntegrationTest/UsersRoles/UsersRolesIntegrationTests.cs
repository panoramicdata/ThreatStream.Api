using System.Text.Json;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.IntegrationTest.UsersRoles;

/// <summary>
/// Reads roles, permissions, identity providers and SSO security group mappings (needs Org Admin and the Users &amp; Roles
/// Management feature). Role, assignment and mapping changes are never run live.
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class UsersRolesIntegrationTests(ThreatStreamFixture fixture)
{
	[Fact]
	public async Task UserRoles_ListSystemRoles_ReturnsLegacyRoles()
	{
		var page = await fixture.Client.UserRoles.ListAsync(new UserRoleListOptions { Type = UserRoleType.Legacy, Limit = 5 }, TestContext.Current.CancellationToken);

		page.Objects.Should().NotBeEmpty("every organisation has the out-of-the-box system roles");
		page.Objects.Should().OnlyContain(r => r.Type == UserRoleType.Legacy && r.Id > 0);
	}

	[Fact]
	public async Task UserRoles_GetPermissions_ReturnsAResponse()
	{
		var permissions = await fixture.Client.UserRoles.GetPermissionsAsync(TestContext.Current.CancellationToken);

		permissions.ValueKind.Should().BeOneOf(JsonValueKind.Array, JsonValueKind.Object);
	}

	[Fact]
	public async Task IdentityProviders_ThenSsoPermissions_ListTheMappings()
	{
		var cancellationToken = TestContext.Current.CancellationToken;

		var providers = await fixture.Client.IdentityProviders.ListAsync(new ListOptions { Limit = 1 }, cancellationToken);
		if (providers.Objects is not [var provider, ..])
		{
			// SSO is optional: an organisation without an identity provider has no mappings to list.
			providers.Objects.Should().BeEmpty();
			return;
		}

		var mappings = await fixture.Client.SsoPermissions.ListAsync(provider.Id, new ListOptions { Limit = 5 }, cancellationToken);

		mappings.Objects.Should().OnlyContain(m => m.Id > 0);
	}
}
