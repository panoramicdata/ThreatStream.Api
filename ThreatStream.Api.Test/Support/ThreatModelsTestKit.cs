using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Test.Support;

/// <summary>
/// Shared JSON and assertions for the threat model groups (actors, attack patterns, campaigns, incidents and their
/// lookups). The reference shows no response bodies for these types, so the JSON is built from each type's attribute
/// table, with IDs and values from its examples, and mixes the shapes ThreatStream uses for "related" attributes (bare
/// IDs, numeric strings, names and objects).
/// </summary>
internal static class ThreatModelsTestKit
{
	/// <summary>A one-object page in ThreatStream's list envelope.</summary>
	public static string Page(string json) => $$"""
		{
			"meta": {"limit": 20, "offset": 0, "next": null, "previous": null, "total_count": 1},
			"objects": [{{json}}]
		}
		""";

	/// <summary>An entity of <paramref name="type"/> with every shared attribute, followed by the type's own <paramref name="extra"/> attributes.</summary>
	public static string Entity(long id, string type, string extra) => $$"""
		{
			"id": {{id}},
			"resource_uri": "/api/v1/{{type}}/{{id}}/",
			"name": "Example {{type}}",
			"description": "Seen in the wild",
			"tags": ["bad actor", {"id": "q1w2e3", "name": "dummy tag", "tlp": "red", "org_id": 42}, null],
			"tags_v2": [{"id": "q1w2e3", "name": "dummy tag", "tlp": "amber+strict", "org_id": "42"}],
			"tlp": "amber",
			"is_public": "false",
			"circles": [101, {"id": "102", "name": "Partners", "resource_uri": "/api/v1/trustedcircle/102/"}],
			"publication_status": "published",
			"source_created": "2014-10-02T20:44:35",
			"source_modified": "2014-10-03T20:44:35",
			"created_ts": "2014-10-04T20:44:35",
			"modified_ts": "2014-10-05T20:44:35.123456",
			"published_ts": 1412628275,
			"organization_id": "42",
			"organization": {"id": 42, "name": "Example Org", "title": "Example Org", "resource_uri": "/api/v1/userorganization/42/"},
			"owner_user_id": 7,
			"owner_user": {"id": 7, "email": "analyst@example.com", "name": "Analyst", "resource_uri": "/api/v1/user/7/"},
			"feed_id": "0",
			"uuid": "4c5e3a8e-1d2b-4b6f-9a51-0f2a9c7e1d11",
			"is_anonymous": 0,
			"is_cloneable": "yes",
			"starred_by_me": true,
			"starred_total_count": "3",
			"watched_by_me": false,
			"watched_total_count": 2,
			"workgroups": ["SOC"],
			"remote_api": false,
			"intelligence": [165546],
			"signatures": ["5"],
			"ttps": [{"id": 63, "name": "Spear phishing", "resource_uri": "/api/v1/ttp/63/"}],
			"parent": null,
			{{extra}}
		}
		""";

	/// <summary>An observable associated with an entity.</summary>
	public const string ObservableJson = """
		{
			"id": 165546,
			"resource_uri": "/api/v2/intelligence/165546/",
			"value": "bad.example",
			"type": "domain",
			"itype": "mal_domain",
			"threat_type": "malware",
			"status": "active",
			"confidence": "80",
			"tlp": "red",
			"tags": [{"id": "a1", "name": "phishing", "tlp": "red"}],
			"created_ts": "2026-01-02T03:04:05",
			"modified_ts": "2026-01-03T03:04:05Z",
			"source": "Example feed"
		}
		""";

	/// <summary>One value of a lookup list such as <c>api/v1/victimtype/</c>.</summary>
	public static string LookupJson(string path, long id, string value) => $$"""
		{"id": {{id}}, "value": "{{value}}", "display_name": "{{value}}", "resource_uri": "/api/v1/{{path}}/{{id}}/", "description": "From the STIX vocabulary"}
		""";

	/// <summary>An empty page.</summary>
	public const string EmptyPage = """{"meta": {"limit": 20, "offset": 0, "next": null, "previous": null, "total_count": 0}, "objects": []}""";

	/// <summary>The paging options most request tests send.</summary>
	public static ListOptions Paging => new() { Limit = 10, Offset = 20 };

	/// <summary>The query <see cref="Paging"/> produces.</summary>
	public const string PagingQuery = "?limit=10&offset=20";

	/// <summary>Asserts every shared attribute <see cref="Entity"/> wrote.</summary>
	public static void ShouldHaveEntityFields(this ThreatModelEntity entity, long id, string type)
	{
		entity.Id.Should().Be(id);
		entity.ResourceUri.Should().Be($"/api/v1/{type}/{id}/");
		entity.Name.Should().Be($"Example {type}");
		entity.Description.Should().Be("Seen in the wild");
		entity.Tags.Select(t => (t.Name, t.Tlp, t.Id, t.OrgId)).Should().Equal(("bad actor", null, null, null), ("dummy tag", Tlp.Red, "q1w2e3", 42L));
		entity.TagsV2.Should().ContainSingle().Which.Tlp.Should().Be(Tlp.AmberStrict);
		entity.Tlp.Should().Be(Tlp.Amber);
		entity.IsPublic.Should().BeFalse();
		entity.Circles.Select(c => (c.Id, c.Name, c.ResourceUri)).Should().Equal((101L, null, null), (102L, "Partners", "/api/v1/trustedcircle/102/"));
		entity.PublicationStatus.Should().Be(ThreatModelPublicationStatus.Published);
		entity.SourceCreated.Should().Be(new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero));
		entity.SourceModified.Should().Be(new DateTimeOffset(2014, 10, 3, 20, 44, 35, TimeSpan.Zero));
		entity.CreatedTs.Should().Be(new DateTimeOffset(2014, 10, 4, 20, 44, 35, TimeSpan.Zero));
		entity.ModifiedTs.Should().Be(new DateTimeOffset(2014, 10, 5, 20, 44, 35, TimeSpan.Zero).AddTicks(1234560));
		entity.PublishedTs.Should().Be(DateTimeOffset.FromUnixTimeSeconds(1412628275));
		entity.OrganizationId.Should().Be(42);
		entity.Organization!.Name.Should().Be("Example Org");
		entity.Organization.AdditionalProperties["title"].GetString().Should().Be("Example Org");
		entity.OwnerUserId.Should().Be(7);
		entity.OwnerUser!.Id.Should().Be(7);
		entity.OwnerUser.AdditionalProperties["email"].GetString().Should().Be("analyst@example.com");
		entity.FeedId.Should().Be(0);
		entity.Uuid.Should().Be("4c5e3a8e-1d2b-4b6f-9a51-0f2a9c7e1d11");
		entity.IsAnonymous.Should().BeFalse();
		entity.IsCloneable.Should().Be("yes");
		entity.StarredByMe.Should().BeTrue();
		entity.StarredTotalCount.Should().Be(3);
		entity.WatchedByMe.Should().BeFalse();
		entity.WatchedTotalCount.Should().Be(2);
		entity.Workgroups.Should().ContainSingle().Which.Name.Should().Be("SOC");
		entity.RemoteApi.Should().BeFalse();
		entity.Intelligence.Should().ContainSingle().Which.Id.Should().Be(165546);
		entity.Signatures.Should().ContainSingle().Which.Id.Should().Be(5);
		entity.Ttps.Should().ContainSingle().Which.Name.Should().Be("Spear phishing");
		entity.AdditionalProperties.Should().ContainKey("parent");
	}

	/// <summary>Asserts every attribute of <see cref="ObservableJson"/>.</summary>
	public static void ShouldBeTheExampleObservable(this ThreatStreamPage<ThreatModelObservable> page)
	{
		var observable = page.Objects.Should().ContainSingle().Subject;
		observable.Id.Should().Be(165546);
		observable.ResourceUri.Should().Be("/api/v2/intelligence/165546/");
		observable.Value.Should().Be("bad.example");
		observable.Type.Should().Be("domain");
		observable.IType.Should().Be("mal_domain");
		observable.ThreatType.Should().Be("malware");
		observable.Status.Should().Be("active");
		observable.Confidence.Should().Be(80);
		observable.Tlp.Should().Be(Tlp.Red);
		observable.Tags.Should().ContainSingle().Which.Name.Should().Be("phishing");
		observable.CreatedTs.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
		observable.ModifiedTs.Should().Be(new DateTimeOffset(2026, 1, 3, 3, 4, 5, TimeSpan.Zero));
		observable.AdditionalProperties["source"].GetString().Should().Be("Example feed");
	}

	/// <summary>Asserts every attribute of a page holding one <see cref="LookupJson"/> value.</summary>
	public static void ShouldBeTheLookup(this ThreatStreamPage<ThreatModelReference> page, string path, long id, string value)
	{
		var lookup = page.Objects.Should().ContainSingle().Subject;
		lookup.Id.Should().Be(id);
		lookup.Value.Should().Be(value);
		lookup.DisplayName.Should().Be(value);
		lookup.ResourceUri.Should().Be($"/api/v1/{path}/{id}/");
		lookup.AdditionalProperties["description"].GetString().Should().Be("From the STIX vocabulary");
	}

	/// <summary>
	/// Asserts that a read-only client refuses <paramref name="call"/> (a create or update) before sending anything.
	/// </summary>
	public static async Task ShouldBeRefusedWhenReadOnlyAsync(Func<ThreatStreamClient, CancellationToken, Task> call, string method, string path)
	{
		var stub = new StubHandler();
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		var act = () => call(client, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		thrown.Which.Method.Should().Be(method);
		thrown.Which.Message.Should().Contain(path);
		stub.Calls.Should().BeEmpty();
	}

	/// <summary>Asserts that <paramref name="call"/> raises <see cref="ThreatStreamApiException"/> for a 404.</summary>
	public static Task ShouldFailWhenNotFoundAsync(Func<ThreatStreamClient, CancellationToken, Task> call)
		=> TestClient.ShouldFailAsync(call, HttpStatusCode.NotFound, """{"message": "Not found"}""", "Not found");
}
