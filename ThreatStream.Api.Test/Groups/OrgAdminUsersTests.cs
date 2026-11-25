using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.UserAdministration;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.UserAdministrationTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class OrgAdminUsersTests
{
	[Fact]
	public async Task ListAsync_SendsGet()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.OrgAdminUsers.ListAsync(null, ct), UsersPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/orgadmin/");
	}

	[Fact]
	public async Task ListAsync_SendsPaging()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.OrgAdminUsers.ListAsync(new ListOptions { Limit = 50, Offset = 100, OrderBy = "email" }, ct),
			UsersPageJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/orgadmin/", "?limit=50&offset=100&order_by=email");
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.OrgAdminUsers.ListAsync(null, ct), UsersPageJson);

		page.Meta!.TotalCount.Should().Be(1);
		page.Objects.Should().ContainSingle().Which.ShouldBeTheExampleUser();
	}

	[Fact]
	public async Task GetAsync_SendsGetForTheUser()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.OrgAdminUsers.GetAsync(725, ct), UserJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/orgadmin/725/");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var user = await TestClient.ReadAsync((client, ct) => client.OrgAdminUsers.GetAsync(725, ct), UserJson);

		user.ShouldBeTheExampleUser();
	}

	[Fact]
	public async Task GetAsync_ToString_DoesNotRevealTheApiKey()
	{
		var user = await TestClient.ReadAsync((client, ct) => client.OrgAdminUsers.GetAsync(725, ct), UserJson);

		user.ToString().Should().NotContain(FakeApiKey);
	}

	[Fact]
	public async Task GetAsync_MinimalUser_HasNoRoles()
	{
		var user = await TestClient.ReadAsync((client, ct) => client.OrgAdminUsers.GetAsync(5, ct), """{"user_id": 5, "roles": null}""");

		user.UserId.Should().Be(5);
		user.Roles.Should().BeEmpty();
		user.ApiKey.Should().BeNull();
	}

	private static async Task CreateShouldSendAsync(OrganizationUserCreateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.OrgAdminUsers.CreateAsync(request, ct), UserJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/orgadmin/", body: body);
	}

	private static async Task UpdateShouldSendAsync(OrganizationUserUpdateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.OrgAdminUsers.UpdateAsync(145, request, ct), UserJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/orgadmin/145/", body: body);
	}

	[Fact]
	public Task CreateAsync_SendsPostWithPrivileges()
		=> CreateShouldSendAsync(
			// The reference's example: a non-Read Only user with every privilege.
			new OrganizationUserCreateRequest
			{
				Email = "newuser@example.com",
				IsOrgAdmin = true,
				CanApproveIntel = true,
				CanShareIntelligence = true,
				CanSeeApiKey = true,
				CanImportToTaxiiInbox = true
			},
			"""{"email":"newuser@example.com","is_org_admin":true,"can_approve_intel":true,"can_share_intelligence":true,"can_see_api_key":true,"can_import_to_taxii_inbox":true}""");

	[Fact]
	public Task CreateAsync_SendsEveryField()
		=> CreateShouldSendAsync(
			new OrganizationUserCreateRequest
			{
				Email = "reader@example.com",
				IsOrgAdmin = false,
				IsReadOnly = true,
				CanApproveIntel = false,
				CanShareIntelligence = false,
				CanSeeApiKey = false,
				CanImportToTaxiiInbox = false,
				CanUseChat = true,
				IsTfaExempt = false,
				Roles = [7, 9]
			},
			"""{"email":"reader@example.com","is_org_admin":false,"is_readonly":true,"can_approve_intel":false,"can_share_intelligence":false,"can_see_api_key":false,"can_import_to_taxii_inbox":false,"can_use_chat":true,"is_tfa_exempt":false,"roles":[7,9]}""");

	[Fact]
	public async Task CreateAsync_WithRoles_MapsTheNewUser()
	{
		var request = new OrganizationUserCreateRequest { Email = "test@example.com", Roles = [12, 3155] };

		var user = await TestClient.ReadAsync((client, ct) => client.OrgAdminUsers.CreateAsync(request, ct), UserJson);

		user.ShouldBeTheExampleUser();
	}

	[Fact]
	public Task UpdateAsync_SendsPatchWithOnlyTheChanges()
		=> UpdateShouldSendAsync(new OrganizationUserUpdateRequest { IsOrgAdmin = true }, """{"is_org_admin":true}""");

	[Fact]
	public Task UpdateAsync_SendsMustChangePasswordAndRoles()
		=> UpdateShouldSendAsync(
			new OrganizationUserUpdateRequest { MustChangePassword = true, IsReadOnly = true, Roles = [] },
			"""{"must_change_password":true,"is_readonly":true,"roles":[]}""");

	[Fact]
	public async Task UpdateAsync_MapsTheUpdatedUser()
	{
		var user = await TestClient.ReadAsync(
			(client, ct) => client.OrgAdminUsers.UpdateAsync(725, new OrganizationUserUpdateRequest { IsTfaExempt = true }, ct),
			UserJson);

		user.ShouldBeTheExampleUser();
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.OrgAdminUsers.DeleteAsync(348, ct), string.Empty);

		call.ShouldBe(HttpMethod.Delete, "/api/v1/orgadmin/348/");
	}

	[Fact]
	public async Task UnlockAsync_SendsPatchWithoutABody()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.OrgAdminUsers.UnlockAsync(123, ct), string.Empty);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/orgadmin/123/unlock/");
	}

	[Fact]
	public Task GetAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.OrgAdminUsers.GetAsync(725, ct),
			HttpStatusCode.Forbidden,
			"""{"message": "You must be an organization administrator"}""",
			"You must be an organization administrator");
}
