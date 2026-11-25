using ThreatStream.Api.Models.UserAdministration;
using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.Test.Support;

/// <summary>Responses and assertions shared by the user administration tests.</summary>
internal static class UserAdministrationTestKit
{
	/// <summary>An obviously fake API key, which must never appear in a user's <see cref="object.ToString"/>.</summary>
	public const string FakeApiKey = "fake-secret-api-key-0123456789";

	/// <summary>
	/// The user in the reference's example (Users and Roles Management, <c>GET api/v1/orgadmin/</c>), with the privileges
	/// from User Administration's attribute table, an API key and an unmodelled property.
	/// </summary>
	public const string UserJson = $$"""
		{
			"avatar_s3_url": "",
			"date_joined": "2025-05-22T19:23:50",
			"date_password_changed": null,
			"email": "test@example.com",
			"is_active": true,
			"is_locked": false,
			"last_access_ts": "2025-06-25T05:57:31.215305",
			"last_login": "2025-06-25T05:55:06.287563",
			"must_change_password": false,
			"name": "Test",
			"next_password_change_ts": "2025-09-01T00:00:00",
			"nickname": "tester",
			"resource_uri": "/api/v1/orgadmin/725/",
			"roles": [
				{ "id": 12, "name": "Organization Administrator", "type": "legacy" },
				{ "id": 3155, "name": "test role again 2", "type": "user" }
			],
			"user_id": "725",
			"is_org_admin": true,
			"is_readonly": false,
			"can_approve_intel": true,
			"can_share_intelligence": "true",
			"can_see_api_key": true,
			"can_import_to_taxii_inbox": false,
			"can_use_chat": false,
			"is_tfa_exempt": 1,
			"api_key": "{{FakeApiKey}}",
			"organization": "/api/v1/organization/1/"
		}
		""";

	/// <summary>A page of users, as <c>GET api/v1/orgadmin/</c> returns it.</summary>
	public const string UsersPageJson = $$"""{"meta": {"limit": 20, "next": null, "offset": 0, "previous": null, "total_count": 1}, "objects": [{{UserJson}}]}""";

	/// <summary>
	/// Asserts that a read-only client refuses <paramref name="call"/> before sending anything, as every administrative
	/// change in these categories must be refused.
	/// </summary>
	public static async Task ShouldBeRefusedWhenReadOnlyAsync(Func<ThreatStreamClient, CancellationToken, Task> call, string method, string path)
	{
		var stub = new StubHandler();
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		var act = () => call(client, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		thrown.Which.Method.Should().Be(method);
		thrown.Which.Path.Should().Be(TestClient.BaseUrl + path);
		stub.Calls.Should().BeEmpty();
	}

	/// <summary>Asserts that a read-only client still sends <paramref name="call"/>, a read.</summary>
	public static async Task ShouldBeAllowedWhenReadOnlyAsync(Func<ThreatStreamClient, CancellationToken, Task> call, string response)
	{
		var stub = TestClient.Stub(response);
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		await call(client, TestContext.Current.CancellationToken);

		stub.Calls.Should().ContainSingle().Which.Method.Should().Be(HttpMethod.Get);
	}

	/// <summary>Asserts every modelled field of <see cref="UserJson"/>.</summary>
	public static void ShouldBeTheExampleUser(this OrganizationUser user)
	{
		user.UserId.Should().Be(725);
		user.ResourceUri.Should().Be("/api/v1/orgadmin/725/");
		user.Email.Should().Be("test@example.com");
		user.Name.Should().Be("Test");
		user.Nickname.Should().Be("tester");
		user.AvatarS3Url.Should().BeEmpty();
		user.ApiKey.Should().Be(FakeApiKey);
		user.IsActive.Should().BeTrue();
		user.IsLocked.Should().BeFalse();
		user.MustChangePassword.Should().BeFalse();
		user.DateJoined.Should().Be(new DateTimeOffset(2025, 5, 22, 19, 23, 50, TimeSpan.Zero));
		user.DatePasswordChanged.Should().BeNull();
		user.NextPasswordChangeTs.Should().Be(new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero));
		user.LastAccessTs.Should().Be(new DateTimeOffset(2025, 6, 25, 5, 57, 31, TimeSpan.Zero).AddTicks(2153050));
		user.LastLogin.Should().Be(new DateTimeOffset(2025, 6, 25, 5, 55, 6, TimeSpan.Zero).AddTicks(2875630));
		user.ShouldHaveTheExamplePrivileges();
		user.Roles.Should().HaveCount(2);
		user.Roles[0].ShouldBeRole(12, "Organization Administrator", UserRoleType.Legacy);
		user.Roles[1].ShouldBeRole(3155, "test role again 2", UserRoleType.User);
		user.AdditionalProperties["organization"].GetString().Should().Be("/api/v1/organization/1/");
	}

	/// <summary>Asserts a role reference's ID, name and type.</summary>
	public static void ShouldBeRole(this UserRoleReference role, long id, string name, UserRoleType type)
	{
		role.Id.Should().Be(id);
		role.Name.Should().Be(name);
		role.Type.Should().Be(type);
	}

	private static void ShouldHaveTheExamplePrivileges(this OrganizationUser user)
	{
		user.IsOrgAdmin.Should().BeTrue();
		user.IsReadOnly.Should().BeFalse();
		user.CanApproveIntel.Should().BeTrue();
		user.CanShareIntelligence.Should().BeTrue();
		user.CanSeeApiKey.Should().BeTrue();
		user.CanImportToTaxiiInbox.Should().BeFalse();
		user.CanUseChat.Should().BeFalse();
		user.IsTfaExempt.Should().BeTrue();
	}
}
