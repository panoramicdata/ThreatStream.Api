using System.Net;
using System.Text.Json;
using ThreatStream.Api.Models.UsersRoles;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UsersRolesTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class UserRolesTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.ListAsync(null, ct), RolesPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/user_role/");
	}

	[Theory]
	[InlineData(UserRoleType.User, "?type=user")]
	[InlineData(UserRoleType.Legacy, "?type=legacy")]
	public async Task ListAsync_SendsTheType(UserRoleType type, string query)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.ListAsync(new UserRoleListOptions { Type = type }, ct), RolesPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/user_role/", query);
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.UserRoles.ListAsync(null, ct), RolesPageJson);

		page.Objects.Should().ContainSingle().Which.ShouldBeTheExampleRole();
	}

	[Fact]
	public async Task ListAsync_UnknownType_ReadsAsUnknown()
	{
		var page = await TestClient.ReadAsync(
			(client, ct) => client.UserRoles.ListAsync(null, ct),
			"""{"objects": [{"id": 1, "name": "x", "type": "future", "permissions": null}]}""");

		page.Objects[0].Type.Should().Be(UserRoleType.Unknown);
		page.Objects[0].Permissions.Should().BeEmpty();
	}

	[Fact]
	public async Task CreateAsync_SendsPostWithNameAndPermissions()
	{
		var request = new UserRoleCreateRequest { Name = "PlatformOrgAdmin", Permissions = ["is_org_admin", "is_tfa_exempt", "can_use_match"] };

		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.CreateAsync(request, ct), RoleJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/user_role/", body: """{"name":"PlatformOrgAdmin","permissions":["is_org_admin","is_tfa_exempt","can_use_match"]}""");
	}

	[Fact]
	public async Task CreateAsync_MapsTheNewRole()
	{
		var request = new UserRoleCreateRequest { Name = "rule_intel_creator", Permissions = ["can_approve_intel", "can_change_rules"] };

		var role = await TestClient.ReadAsync((client, ct) => client.UserRoles.CreateAsync(request, ct), RoleJson);

		role.ShouldBeTheExampleRole();
	}

	[Fact]
	public async Task GetPermissionsAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.GetPermissionsAsync(ct), """["can_approve_intel"]""");

		call.ShouldBe(HttpMethod.Get, "/api/v1/user_role/permissions/");
	}

	[Fact]
	public async Task GetPermissionsAsync_ReturnsTheResponseUnparsed()
	{
		var permissions = await TestClient.ReadAsync(
			(client, ct) => client.UserRoles.GetPermissionsAsync(ct),
			"""{"permissions": ["can_approve_intel", "can_audit", "can_change_rules", "is_org_admin"]}""");

		permissions.GetProperty("permissions").EnumerateArray().Select(p => p.GetString()).Should()
			.Equal("can_approve_intel", "can_audit", "can_change_rules", "is_org_admin");
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithTheChanges()
	{
		var request = new UserRoleUpdateRequest { Name = "new_role_name", Permissions = ["can_approve_intel"] };

		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.UpdateAsync(3011, request, ct), RoleJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/user_role/3011/", body: """{"name":"new_role_name","permissions":["can_approve_intel"]}""");
	}

	[Fact]
	public async Task UpdateAsync_OmitsUnsetFields_AndMapsTheRole()
	{
		var stub = TestClient.Stub(RoleJson);
		using var client = TestClient.Create(stub);

		var role = await client.UserRoles.UpdateAsync(3011, new UserRoleUpdateRequest { Name = "renamed" }, TestContext.Current.CancellationToken);

		stub.Calls.Should().ContainSingle().Which.Body.Should().Be("""{"name":"renamed"}""");
		role.ShouldBeTheExampleRole();
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.DeleteAsync(4, ct), string.Empty);

		call.ShouldBe(HttpMethod.Delete, "/api/v1/user_role/4/");
	}

	[Fact]
	public async Task AssignAsync_SendsPostWithUsersAndRoles()
	{
		var request = new UserRoleAssignmentRequest { Users = [39], Roles = [3009, 3012] };

		var call = await TestClient.CaptureAsync((client, ct) => client.UserRoles.AssignAsync(request, ct), "{}");

		call.ShouldBe(HttpMethod.Post, "/api/v1/user_role/users_set/", body: """{"users":[39],"roles":[3009,3012]}""");
	}

	[Fact]
	public Task DeleteAsync_SystemRole_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.UserRoles.DeleteAsync(12, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "System roles cannot be deleted"}""",
			"System roles cannot be deleted");

	[Fact]
	public async Task GetPermissionsAsync_ArrayResponse_IsReturned()
	{
		var permissions = await TestClient.ReadAsync((client, ct) => client.UserRoles.GetPermissionsAsync(ct), """["is_tfa_exempt"]""");

		permissions.ValueKind.Should().Be(JsonValueKind.Array);
	}
}
