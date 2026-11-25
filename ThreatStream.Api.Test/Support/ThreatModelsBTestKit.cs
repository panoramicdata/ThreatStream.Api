using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.ThreatModels;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Support;

/// <summary>
/// Request assertions for the malware, signature, threat bulletin, TTP and vulnerability tests, whose read operations have
/// the same shape on every entity type. The shared JSON and entity assertions are <see cref="ThreatModelsTestKit"/>'s.
/// </summary>
internal static class ThreatModelsBTestKit
{
	/// <summary>Every list filter set.</summary>
	public static ThreatModelListOptions AllListOptions => new()
	{
		Limit = 10,
		Offset = 20,
		OrderBy = "-created_ts",
		IdIn = [1, 2],
		Name = "dummy test",
		NameContains = "dummy",
		PublicationStatus = ThreatModelPublicationStatus.Published,
		ModifiedFrom = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero),
		ModifiedBefore = new DateTimeOffset(2014, 10, 3, 0, 0, 0, TimeSpan.Zero)
	};

	/// <summary>The query string <see cref="AllListOptions"/> produces.</summary>
	public const string AllListQuery = "?id__in=1%2C2&name=dummy%20test&name__contains=dummy&publication_status=published"
		+ "&modified_ts__gte=2014-10-02T20%3A44%3A35Z&modified_ts__lt=2014-10-03T00%3A00%3A00Z&limit=10&offset=20&order_by=-created_ts";

	/// <summary>A page of entities linked to another: an actor, with an attribute of its own type.</summary>
	public static readonly string LinkedPage = Page(Entity(234673, "actor", "\"aliases\": \"Fancy Bear\""));

	/// <summary>A page of observables associated with an entity.</summary>
	public static readonly string IntelligencePage = Page(ObservableJson);

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
	public static async Task AssociatedWithValueShouldSendAsync<T>(Func<ThreatStreamClient, string, ListOptions?, CancellationToken, Task<ThreatStreamPage<T>>> lookup, string type)
	{
		var path = $"/api/v1/{type}/associated_with_intelligence/";
		(await TestClient.CaptureAsync((client, ct) => lookup(client, "google.com", null, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, "?value=google.com");
		(await TestClient.CaptureAsync((client, ct) => lookup(client, "http://a.example/?b=c&d", Paging, ct), EmptyPage))
			.ShouldBe(HttpMethod.Get, path, "?value=http%3A%2F%2Fa.example%2F%3Fb%3Dc%26d&limit=10&offset=20");
	}

	/// <summary>Asserts the comma-separated IDs and paging of an associated-with-intelligence lookup by observable IDs.</summary>
	public static async Task AssociatedWithIdsShouldSendAsync<T>(Func<ThreatStreamClient, IReadOnlyList<long>, ListOptions?, CancellationToken, Task<ThreatStreamPage<T>>> lookup, string type)
	{
		var path = $"/api/v1/{type}/associated_with_intelligence/";
		(await TestClient.CaptureAsync((client, ct) => lookup(client, [165546, 165547], null, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, "?ids=165546%2C165547");
		(await TestClient.CaptureAsync((client, ct) => lookup(client, [1], Paging, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, "?ids=1&" + PagingQuery[1..]);
	}

	/// <summary>Asserts the path and paging of a linked-entities list, and that it reads the linked entities.</summary>
	public static async Task LinkedShouldSendAsync(Func<ThreatStreamClient, long, ThreatModelType, ListOptions?, CancellationToken, Task<ThreatStreamPage<ThreatModelAssociatedEntity>>> linked, string type)
	{
		(await TestClient.CaptureAsync((client, ct) => linked(client, 79, ThreatModelType.Actor, null, ct), LinkedPage)).ShouldBe(HttpMethod.Get, $"/api/v1/{type}/79/actor/");
		(await TestClient.CaptureAsync((client, ct) => linked(client, 79, ThreatModelType.ThreatBulletin, Paging, ct), EmptyPage))
			.ShouldBe(HttpMethod.Get, $"/api/v1/{type}/79/tipreport/", PagingQuery);

		var page = await TestClient.ReadAsync((client, ct) => linked(client, 79, ThreatModelType.Actor, null, ct), LinkedPage);
		var entity = page.Objects.Should().ContainSingle().Subject;
		entity.ShouldHaveEntityFields(234673, "actor");
		entity.AdditionalProperties["aliases"].GetString().Should().Be("Fancy Bear");
	}

	/// <summary>Asserts the path and paging of an associated-observables list, and that it reads the observables.</summary>
	public static async Task IntelligenceShouldSendAsync(Func<ThreatStreamClient, long, ListOptions?, CancellationToken, Task<ThreatStreamPage<Observable>>> intelligence, string type)
	{
		var path = $"/api/v1/{type}/79/intelligence/";
		(await TestClient.CaptureAsync((client, ct) => intelligence(client, 79, null, ct), IntelligencePage)).ShouldBe(HttpMethod.Get, path);
		(await TestClient.CaptureAsync((client, ct) => intelligence(client, 79, Paging, ct), EmptyPage)).ShouldBe(HttpMethod.Get, path, PagingQuery);

		(await TestClient.ReadAsync((client, ct) => intelligence(client, 79, null, ct), IntelligencePage)).ShouldBeTheExampleObservable();
	}
}
