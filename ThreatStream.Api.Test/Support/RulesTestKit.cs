namespace ThreatStream.Api.Test.Support;

/// <summary>Responses shared by the rules tests.</summary>
internal static class RulesTestKit
{
	/// <summary>
	/// A rule with every field the ThreatStream API reference (Rules) lists in its response table, as the create example
	/// would return it once matched; organization, user and investigation names are made up.
	/// </summary>
	public const string RuleJson = """
		{
			"create_investigation": true,
			"created_ts": "2024-03-01T10:15:30.123456",
			"exclude_notify_org_whitelisted": true,
			"exclude_notify_owner_org": false,
			"has_associations": true,
			"id": 56,
			"investigation": {"id": 901, "name": "test_rule4 investigation", "resource_uri": "/api/v1/investigation/901/"},
			"keywords": ["key", "foobar"],
			"match_actors": false,
			"match_observables": true,
			"match_reportedfiles": false,
			"match_signatures": true,
			"match_tips": true,
			"match_vulnerabilities": false,
			"matches": "17",
			"modified_ts": "2024-03-02T11:00:00",
			"name": "test_rule4",
			"org_id": 7,
			"org_shared": false,
			"organization": {"id": "7", "name": "Example Org", "resource_uri": "/api/v1/userorganization/7/"},
			"remote_api": false,
			"resource_uri": "/api/v1/rule/56/",
			"tags": [{"name": "new_tag", "tlp": "white"}],
			"user": "analyst@example.com",
			"user_id": 42
		}
		""";

	/// <summary>A page holding <see cref="RuleJson"/>.</summary>
	public const string PageJson = $$"""
		{
			"meta": {"limit": 20, "next": null, "offset": 0, "previous": null, "total_count": 1},
			"objects": [{{RuleJson}}]
		}
		""";
}
