using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UsersRoles;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAdministrationTestKit;
using static ThreatStream.Api.Test.Support.UsersRolesTestKit;

namespace ThreatStream.Api.Test.Groups;

public class SsoPermissionsTests
{
	[Fact]
	public async Task ListAsync_SendsGetForTheIdentityProvider()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SsoPermissions.ListAsync(159, null, ct), MappingsPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/ssopermission/", "?identity_provider_id=159");
	}

	[Fact]
	public async Task ListAsync_SendsPaging()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.SsoPermissions.ListAsync(159, new ListOptions { Limit = 0 }, ct),
			MappingsPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/ssopermission/", "?identity_provider_id=159&limit=0");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.SsoPermissions.ListAsync(159, null, ct), MappingsPageJson);

		page.Objects.Should().ContainSingle().Which.ShouldBeTheExampleMapping();
	}

	[Fact]
	public async Task CreateAsync_SendsPostWithGroupProviderAndRoles()
	{
		var request = new SsoGroupMappingCreateRequest { Group = "SecAdminGroup", IdpId = 9, Roles = [7] };

		var call = await TestClient.CaptureAsync((client, ct) => client.SsoPermissions.CreateAsync(request, ct), MappingJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/ssopermission/", body: """{"group":"SecAdminGroup","idp_id":9,"roles":[7]}""");
	}

	[Fact]
	public async Task CreateAsync_MapsTheNewMapping()
	{
		var request = new SsoGroupMappingCreateRequest { Group = "8x9x9f9t9w9sdf9pspdfg", IdpId = 159, Roles = [4567, 3238] };

		var mapping = await TestClient.ReadAsync((client, ct) => client.SsoPermissions.CreateAsync(request, ct), MappingJson);

		mapping.ShouldBeTheExampleMapping();
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithProviderAndRoles()
	{
		var request = new SsoGroupMappingUpdateRequest { IdpId = 9, Roles = [7, 3001] };

		var call = await TestClient.CaptureAsync((client, ct) => client.SsoPermissions.UpdateAsync(1692, request, ct), MappingJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/ssopermission/1692/", body: """{"idp_id":9,"roles":[7,3001]}""");
	}

	[Fact]
	public async Task UpdateAsync_SendsANewGroupName()
	{
		var request = new SsoGroupMappingUpdateRequest { Group = "SecAdminGroup", IdpId = 7, Roles = [6] };

		var call = await TestClient.CaptureAsync((client, ct) => client.SsoPermissions.UpdateAsync(1692, request, ct), MappingJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/ssopermission/1692/", body: """{"group":"SecAdminGroup","idp_id":7,"roles":[6]}""");
	}

	[Fact]
	public async Task UpdateAsync_MapsTheUpdatedMapping()
	{
		var mapping = await TestClient.ReadAsync(
			(client, ct) => client.SsoPermissions.UpdateAsync(1692, new SsoGroupMappingUpdateRequest { IdpId = 159 }, ct),
			MappingJson);

		mapping.ShouldBeTheExampleMapping();
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.SsoPermissions.DeleteAsync(1692, ct), string.Empty);

		call.ShouldBe(HttpMethod.Delete, "/api/v1/ssopermission/1692/");
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.SsoPermissions.ListAsync(1, null, ct),
			HttpStatusCode.BadRequest,
			"""{"error": "identity_provider_id is required"}""",
			"identity_provider_id is required");

	[Fact]
	public Task CreateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.SsoPermissions.CreateAsync(new SsoGroupMappingCreateRequest { Group = "g", IdpId = 1, Roles = [1] }, ct),
			"POST",
			"api/v1/ssopermission/");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.SsoPermissions.UpdateAsync(1692, new SsoGroupMappingUpdateRequest { IdpId = 1 }, ct),
			"PATCH",
			"api/v1/ssopermission/1692/");

	[Fact]
	public Task DeleteAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.SsoPermissions.DeleteAsync(1692, ct), "DELETE", "api/v1/ssopermission/1692/");
}
