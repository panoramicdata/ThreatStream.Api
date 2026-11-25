using ThreatStream.Api.Models.UsersRoles;
using static ThreatStream.Api.Test.Support.UserAdministrationTestKit;
using static ThreatStream.Api.Test.Support.UsersRolesTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class UserRolesTests
{
	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.UserRoles.CreateAsync(new UserRoleCreateRequest { Name = "x", Permissions = ["can_audit"] }, ct),
			"POST",
			"api/v1/user_role/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.UserRoles.UpdateAsync(3011, new UserRoleUpdateRequest { Name = "x" }, ct),
			"PATCH",
			"api/v1/user_role/3011/");

	[Fact]
	public Task DeleteAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.UserRoles.DeleteAsync(4, ct), "DELETE", "api/v1/user_role/4/");

	[Fact]
	public Task AssignAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.UserRoles.AssignAsync(new UserRoleAssignmentRequest { Users = [39], Roles = [3009] }, ct),
			"POST",
			"api/v1/user_role/users_set/");

	[Fact]
	public Task ListAsync_ReadOnlyClient_IsAllowed()
		=> ReadWhenReadOnlyAsync((client, ct) => client.UserRoles.ListAsync(null, ct), RolesPageJson);
}
