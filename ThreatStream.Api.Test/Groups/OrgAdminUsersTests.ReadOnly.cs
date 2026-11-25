using ThreatStream.Api.Models.UserAdministration;
using static ThreatStream.Api.Test.Support.UserAdministrationTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class OrgAdminUsersTests
{
	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.OrgAdminUsers.CreateAsync(new OrganizationUserCreateRequest { Email = "x@example.com" }, ct),
			"POST",
			"api/v1/orgadmin/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.OrgAdminUsers.UpdateAsync(145, new OrganizationUserUpdateRequest { IsOrgAdmin = true }, ct),
			"PATCH",
			"api/v1/orgadmin/145/");

	[Fact]
	public Task DeleteAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.OrgAdminUsers.DeleteAsync(348, ct), "DELETE", "api/v1/orgadmin/348/");

	[Fact]
	public Task UnlockAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.OrgAdminUsers.UnlockAsync(123, ct), "PATCH", "api/v1/orgadmin/123/unlock/");

	[Fact]
	public Task ListAsync_ReadOnlyClient_IsAllowed()
		=> ReadWhenReadOnlyAsync((client, ct) => client.OrgAdminUsers.ListAsync(null, ct), UsersPageJson);
}
