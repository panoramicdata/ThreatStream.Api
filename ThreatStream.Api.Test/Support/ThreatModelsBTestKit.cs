using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.Test.Support;

/// <summary>
/// Shared JSON and request assertions for the malware, signature, threat bulletin, TTP and vulnerability tests, whose
/// read operations have the same shape on every entity type.
/// </summary>
internal static class ThreatModelsBTestKit
{
	public const string EmptyPage = """{"meta": {"limit": 20, "offset": 0, "next": null, "previous": null, "total_count": 0}, "objects": []}""";

	/// <summary>Every list filter set, and the query string they produce.</summary>
	public static ThreatModelListOptions AllListOptions => new()
	{
		Limit = 10,
		Offset = 20,
		OrderBy = "-created_ts",
		Query = "name=dummy",
		Name = "dummy test",
		CreatedTsGte = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
		ModifiedTsGte = new DateTimeOffset(2014, 10, 3, 0, 0, 0, TimeSpan.Zero)
	};

	public const string AllListQuery = "?q=name%3Ddummy&name=dummy%20test&created_ts__gte=2014-10-02T20%3A44%3A35Z&modified_ts__gte=2014-10-03T00%3A00%3A00Z&limit=10&offset=20&order_by=-created_ts";

	public static ListOptions PageOptions => new() { Limit = 0, Offset = 1000 };

	public const string PageQuery = "?limit=0&offset=1000";

	/// <summary>
	/// The attributes every threat model entity shares, as ThreatStream sends them (a numeric and an object circle, an
	/// offset-less timestamp read as UTC), for an entity of <paramref name="type"/> with <paramref name="id"/>.
	/// </summary>
	public static string EntityFields(long id, string type) => $$"""
		"id": {{id}},
		"resource_uri": "/api/v1/{{type}}/{{id}}/",
		"name": "dummy test {{type}}",
		"description": "A {{type}} seen in the wild.",
		"tags": ["big {{type}}", {"id": "t1", "name": "dummy tag"}],
		"tlp": "red",
		"is_public": "false",
		"circles": [12, {"id": 13, "name": "Partners"}],
		"publication_status": "published",
		"source_created": "2014-10-02T20:44:35",
		"source_modified": "2014-10-03T08:00:00Z",
		"created_ts": "2014-10-04T09:30:00",
		"modified_ts": 1412415000
		""";

	/// <summary>Asserts the attributes <see cref="EntityFields"/> wrote.</summary>
	public static void ShouldHaveEntityFields(this ThreatModelEntity entity, long id, string type)
	{
		entity.Id.Should().Be(id);
		entity.ResourceUri.Should().Be($"/api/v1/{type}/{id}/");
		entity.Name.Should().Be($"dummy test {type}");
		entity.Description.Should().Be($"A {type} seen in the wild.");
		entity.Tags.Should().Equal($"big {type}", """{"id": "t1", "name": "dummy tag"}""");
		entity.Tlp.Should().Be(Tlp.Red);
		entity.IsPublic.Should().BeFalse();
		entity.Circles.Select(c => c.GetRawText()).Should().Equal("12", """{"id": 13, "name": "Partners"}""");
		entity.PublicationStatus.Should().Be(ThreatModelPublicationStatus.Published);
		entity.SourceCreated.Should().Be(new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero));
		entity.SourceModified.Should().Be(new DateTimeOffset(2014, 10, 3, 8, 0, 0, TimeSpan.Zero));
		entity.CreatedTs.Should().Be(new DateTimeOffset(2014, 10, 4, 9, 30, 0, TimeSpan.Zero));
		entity.ModifiedTs.Should().Be(new DateTimeOffset(2014, 10, 4, 9, 30, 0, TimeSpan.Zero));
	}

	/// <summary>A one-object page.</summary>
	public static string Page(string objectJson) => $$"""
		{
			"meta": {"limit": 20, "offset": 0, "next": "/api/v1/x/?limit=20&offset=20", "previous": null, "total_count": 21},
			"objects": [{{objectJson}}]
		}
		""";

	/// <summary>A page of entities linked to another: an actor, with an attribute of its own type.</summary>
	public static readonly string LinkedPage = Page($$"""{{{EntityFields(234673, "actor")}}, "aliases": "Fancy Bear"}""");

	/// <summary>A page of observables associated with an entity.</summary>
	public static readonly string IntelligencePage = Page("""
		{"id": 165546, "resource_uri": "/api/v2/intelligence/165546/", "value": "bad.example", "itype": "mal_domain", "confidence": 80}
		""");

	/// <summary>Asserts that a list sends no query without options, and every filter with them.</summary>
	public static async Task ListShouldSendAsync<T>(Func<ThreatStreamClient, ThreatModelListOptions?, CancellationToken, Task<ThreatStreamPage<T>>> list, string type)
	{
		var path = $"/api/v1/{type}/";
		(await TestClient.CaptureAsync((client, ct) => list(client, null, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path);
		(await TestClient.CaptureAsync((client, ct) => list(client, AllListOptions, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, AllListQuery);
	}

	/// <summary>Asserts that a detail read sends no query without options, and every OnPrem option with them.</summary>
	public static async Task GetShouldSendAsync<T>(Func<ThreatStreamClient, long, ThreatModelDetailOptions?, CancellationToken, Task<T>> get, string type, string response)
	{
		var options = new ThreatModelDetailOptions { SkipAssociations = true, SkipIntelligence = false, RemoteApi = true };
		(await TestClient.CaptureAsync((client, ct) => get(client, 79, null, ct), response)).ShouldBe(HttpMethod.Get, $"/api/v1/{type}/79/");
		(await TestClient.CaptureAsync((client, ct) => get(client, 79, options, ct), response))
			.ShouldBe(HttpMethod.Get, $"/api/v1/{type}/79/", "?skip_associations=true&skip_intelligence=false&remote_api=true");
	}

	/// <summary>Asserts the escaped value and paging of an associated-with-intelligence lookup by value.</summary>
	public static async Task AssociatedByValueShouldSendAsync<T>(Func<ThreatStreamClient, string, ListOptions?, CancellationToken, Task<ThreatStreamPage<T>>> lookup, string type)
	{
		var path = $"/api/v1/{type}/associated_with_intelligence/";
		(await TestClient.CaptureAsync((client, ct) => lookup(client, "google.com", null, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, "?value=google.com");
		(await TestClient.CaptureAsync((client, ct) => lookup(client, "http://a.example/?b=c&d", PageOptions, ct), EmptyPage))
			.ShouldBe(HttpMethod.Get, path, "?value=http%3A%2F%2Fa.example%2F%3Fb%3Dc%26d&limit=0&offset=1000");
	}

	/// <summary>Asserts the comma-separated IDs and paging of an associated-with-intelligence lookup by observable IDs.</summary>
	public static async Task AssociatedByIdsShouldSendAsync<T>(Func<ThreatStreamClient, IReadOnlyList<long>, ListOptions?, CancellationToken, Task<ThreatStreamPage<T>>> lookup, string type)
	{
		var path = $"/api/v1/{type}/associated_with_intelligence/";
		(await TestClient.CaptureAsync((client, ct) => lookup(client, [165546, 165547], null, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, "?ids=165546%2C165547");
		(await TestClient.CaptureAsync((client, ct) => lookup(client, [1], PageOptions, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, "?ids=1&limit=0&offset=1000");
	}

	/// <summary>Asserts the path and paging of a linked-entities list, and that it reads the linked entities.</summary>
	public static async Task LinkedEntitiesShouldSendAsync(Func<ThreatStreamClient, long, ThreatModelEntityType, ListOptions?, CancellationToken, Task<ThreatStreamPage<ThreatModelLinkedEntity>>> linked, string type)
	{
		(await TestClient.CaptureAsync((client, ct) => linked(client, 79, ThreatModelEntityType.Actor, null, ct), LinkedPage)).ShouldBe(HttpMethod.Get, $"/api/v1/{type}/79/actor/");
		(await TestClient.CaptureAsync((client, ct) => linked(client, 79, ThreatModelEntityType.TipReport, PageOptions, ct), EmptyPage))
			.ShouldBe(HttpMethod.Get, $"/api/v1/{type}/79/tipreport/", PageQuery);

		var page = await TestClient.ReadAsync((client, ct) => linked(client, 79, ThreatModelEntityType.Actor, null, ct), LinkedPage);
		var entity = page.Objects.Should().ContainSingle().Subject;
		entity.ShouldHaveEntityFields(234673, "actor");
		entity.AdditionalProperties["aliases"].GetString().Should().Be("Fancy Bear");
	}

	/// <summary>Asserts the path and paging of an associated-observables list, and that it reads the observables.</summary>
	public static async Task IntelligenceShouldSendAsync(Func<ThreatStreamClient, long, ListOptions?, CancellationToken, Task<ThreatStreamPage<Observable>>> intelligence, string type)
	{
		var path = $"/api/v1/{type}/79/intelligence/";
		(await TestClient.CaptureAsync((client, ct) => intelligence(client, 79, null, ct), IntelligencePage)).ShouldBe(HttpMethod.Get, path);
		(await TestClient.CaptureAsync((client, ct) => intelligence(client, 79, PageOptions, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, PageQuery);

		var page = await TestClient.ReadAsync((client, ct) => intelligence(client, 79, null, ct), IntelligencePage);
		page.Meta!.TotalCount.Should().Be(21);
		var observable = page.Objects.Should().ContainSingle().Subject;
		observable.Id.Should().Be(165546);
		observable.ResourceUri.Should().Be("/api/v2/intelligence/165546/");
		observable.Value.Should().Be("bad.example");
		observable.IType.Should().Be("mal_domain");
		observable.AdditionalProperties["confidence"].GetInt32().Should().Be(80);
	}

	/// <summary>Asserts that a read-only client refuses <paramref name="call"/> without sending anything.</summary>
	public static async Task ShouldBeRefusedWhenReadOnlyAsync(Func<ThreatStreamClient, CancellationToken, Task> call)
	{
		var stub = new StubHandler();
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		var act = () => call(client, TestContext.Current.CancellationToken);

		await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		stub.Calls.Should().BeEmpty();
	}

	/// <summary>Asserts that a 404 for an entity raises <see cref="ThreatStreamApiException"/>.</summary>
	public static Task NotFoundShouldFailAsync(Func<ThreatStreamClient, CancellationToken, Task> call)
		=> TestClient.ShouldFailAsync(call, HttpStatusCode.NotFound, """{"message": "Not found"}""", "Not found");
}
