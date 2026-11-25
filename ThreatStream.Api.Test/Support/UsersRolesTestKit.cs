using ThreatStream.Api.Models.UsersRoles;

namespace ThreatStream.Api.Test.Support;

/// <summary>Responses and assertions shared by the users and roles tests.</summary>
internal static class UsersRolesTestKit
{
	/// <summary>The role in the reference's example (Users and Roles Management, <c>GET api/v1/user_role/</c>).</summary>
	public const string RoleJson = """
		{
			"created_ts": "2024-12-12T21:07:51.909567",
			"id": 3011,
			"match_filter": null,
			"max_search_back": null,
			"modified_ts": "2024-12-12T21:15:49.207885",
			"name": "rule_intel_creator",
			"permissions": [
				"can_approve_intel",
				"can_change_rules"
			],
			"resource_uri": "/api/v1/user_role/3011/",
			"type": "user"
		}
		""";

	/// <summary>A page of roles.</summary>
	public const string RolesPageJson = $$"""{"meta": {"limit": 20, "next": null, "offset": 0, "total_count": 1}, "objects": [{{RoleJson}}]}""";

	/// <summary>The SSO security group mapping in the reference's example, with its missing comma restored.</summary>
	public const string MappingJson = """
		{
			"group": "8x9x9f9t9w9sdf9pspdfg",
			"id": 1692,
			"identity_provider": "/api/v1/identity-provider/112/",
			"idp_id": 159,
			"resource_uri": "/api/v1/ssopermission/1692/",
			"roles": [
				{ "id": 4567, "name": "Test", "type": "user" },
				{ "id": 3238, "name": "AnomaliRole_azure", "type": "user" }
			]
		}
		""";

	/// <summary>A page of SSO security group mappings.</summary>
	public const string MappingsPageJson = $$"""{"meta": {"limit": 20, "next": null, "offset": 0, "total_count": 1}, "objects": [{{MappingJson}}]}""";

	/// <summary>An identity provider; the reference documents no fields, so all but the ID and path are unmodelled.</summary>
	public const string IdentityProvidersPageJson = """
		{
			"meta": {"limit": 20, "next": null, "offset": 0, "total_count": 1},
			"objects": [{"id": 159, "name": "Entra ID", "resource_uri": "/api/v1/identity-provider/159/"}]
		}
		""";

	/// <summary>Asserts every modelled field of <see cref="RoleJson"/>.</summary>
	public static void ShouldBeTheExampleRole(this UserRole? role)
	{
		role.Should().NotBeNull();
		role.Id.Should().Be(3011);
		role.ResourceUri.Should().Be("/api/v1/user_role/3011/");
		role.Name.Should().Be("rule_intel_creator");
		role.Type.Should().Be(UserRoleType.User);
		role.Permissions.Should().Equal("can_approve_intel", "can_change_rules");
		role.CreatedTs.Should().Be(new DateTimeOffset(2024, 12, 12, 21, 7, 51, TimeSpan.Zero).AddTicks(9095670));
		role.ModifiedTs.Should().Be(new DateTimeOffset(2024, 12, 12, 21, 15, 49, TimeSpan.Zero).AddTicks(2078850));
		role.AdditionalProperties["match_filter"].ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
		role.AdditionalProperties.Should().ContainKey("max_search_back");
	}

	/// <summary>Asserts every modelled field of <see cref="MappingJson"/>.</summary>
	public static void ShouldBeTheExampleMapping(this SsoGroupMapping? mapping)
	{
		mapping.Should().NotBeNull();
		mapping.Id.Should().Be(1692);
		mapping.ResourceUri.Should().Be("/api/v1/ssopermission/1692/");
		mapping.Group.Should().Be("8x9x9f9t9w9sdf9pspdfg");
		mapping.IdentityProvider.Should().Be("/api/v1/identity-provider/112/");
		mapping.IdpId.Should().Be(159);
		mapping.Roles.Should().HaveCount(2);
		mapping.Roles[0].ShouldBeRole(4567, "Test", UserRoleType.User);
		mapping.Roles[1].ShouldBeRole(3238, "AnomaliRole_azure", UserRoleType.User);
	}
}
