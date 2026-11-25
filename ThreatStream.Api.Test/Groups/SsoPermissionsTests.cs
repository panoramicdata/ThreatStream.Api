using System.Net;
using ThreatStream.Api.Interfaces;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UsersRoles;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAdministrationTestKit;
using static ThreatStream.Api.Test.Support.UsersRolesTestKit;

namespace ThreatStream.Api.Test.Groups;

public class SsoPermissionsTests
{
	private const string Mappings = "/api/v1/ssopermission/";
	private const string Mapping1692 = "/api/v1/ssopermission/1692/";

	private static async Task ShouldSendAsync(
		Func<ISsoPermissions, CancellationToken, Task> operation,
		string response,
		HttpMethod method,
		string path,
		string query = RecordedCallAssertions.NoQuery,
		string? body = null)
	{
		var call = await TestClient.CaptureAsync((client, ct) => operation(client.SsoPermissions, ct), response);

		call.ShouldBe(method, path, query, body);
	}

	private static async Task ShouldMapTheExampleMappingAsync(Func<ISsoPermissions, CancellationToken, Task<SsoGroupMapping?>> operation)
		=> (await TestClient.ReadAsync((client, ct) => operation(client.SsoPermissions, ct), MappingJson)).ShouldBeTheExampleMapping();

	[Fact]
	public Task ListAsync_SendsGetForTheIdentityProvider()
		=> ShouldSendAsync((sso, ct) => sso.ListAsync(159, null, ct), MappingsPageJson, HttpMethod.Get, Mappings, "?identity_provider_id=159");

	[Fact]
	public Task ListAsync_SendsPaging()
		=> ShouldSendAsync(
			(sso, ct) => sso.ListAsync(159, new ListOptions { Limit = 0 }, ct),
			MappingsPageJson,
			HttpMethod.Get,
			Mappings,
			"?identity_provider_id=159&limit=0");

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.SsoPermissions.ListAsync(159, null, ct), MappingsPageJson);

		page.Objects.Should().ContainSingle().Which.ShouldBeTheExampleMapping();
	}

	[Fact]
	public Task CreateAsync_SendsPostWithGroupProviderAndRoles()
		=> ShouldSendAsync(
			(sso, ct) => sso.CreateAsync(new SsoGroupMappingCreateRequest { Group = "SecAdminGroup", IdpId = 9, Roles = [7] }, ct),
			MappingJson,
			HttpMethod.Post,
			Mappings,
			body: """{"group":"SecAdminGroup","idp_id":9,"roles":[7]}""");

	[Fact]
	public Task CreateAsync_MapsTheNewMapping()
		=> ShouldMapTheExampleMappingAsync((sso, ct) => sso.CreateAsync(
			new SsoGroupMappingCreateRequest { Group = "8x9x9f9t9w9sdf9pspdfg", IdpId = 159, Roles = [4567, 3238] },
			ct));

	[Fact]
	public Task UpdateAsync_SendsPatchWithProviderAndRoles()
		=> ShouldSendAsync(
			(sso, ct) => sso.UpdateAsync(1692, new SsoGroupMappingUpdateRequest { IdpId = 9, Roles = [7, 3001] }, ct),
			MappingJson,
			HttpMethod.Patch,
			Mapping1692,
			body: """{"idp_id":9,"roles":[7,3001]}""");

	[Fact]
	public Task UpdateAsync_SendsANewGroupName()
		=> ShouldSendAsync(
			(sso, ct) => sso.UpdateAsync(1692, new SsoGroupMappingUpdateRequest { Group = "SecAdminGroup", IdpId = 7, Roles = [6] }, ct),
			MappingJson,
			HttpMethod.Patch,
			Mapping1692,
			body: """{"group":"SecAdminGroup","idp_id":7,"roles":[6]}""");

	[Fact]
	public Task UpdateAsync_MapsTheUpdatedMapping()
		=> ShouldMapTheExampleMappingAsync((sso, ct) => sso.UpdateAsync(1692, new SsoGroupMappingUpdateRequest { IdpId = 159 }, ct));

	[Fact]
	public Task DeleteAsync_SendsDelete()
		=> ShouldSendAsync((sso, ct) => sso.DeleteAsync(1692, ct), string.Empty, HttpMethod.Delete, Mapping1692);

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
