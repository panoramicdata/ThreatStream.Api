using System.Globalization;
using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public class PagingTests
{
	private static CancellationToken Ct => TestContext.Current.CancellationToken;

	private const string MetaBeforeNext = "{\"meta\":{\"limit\":2,\"offset\":0,\"next\":";

	private const string MetaAfterNext = ",\"previous\":null,\"total_count\":5,\"took\":12,\"approximate_count\":false},\"objects\":[";

	private static string Page(string? next, params int[] ids)
		=> MetaBeforeNext + (next is null ? "null" : "\"" + next + "\"") + MetaAfterNext + string.Join(',', ids.Select(Item)) + "]}";

	private static string Item(int id)
	{
		var text = id.ToString(CultureInfo.InvariantCulture);
		var updateId = (id * 10).ToString(CultureInfo.InvariantCulture);
		return "{\"id\":" + text + ",\"name\":\"o" + text + "\",\"update_id\":" + updateId + ",\"resource_uri\":\"/api/v1/probe/" + text + "/\"}";
	}

	private static async Task<int> CountAsync<T>(IAsyncEnumerable<T> items)
	{
		var count = 0;
		await foreach (var _ in items)
		{
			count++;
		}

		return count;
	}

	private static async Task<List<long>> EnumerateAsync(ThreatStreamClient client, string first = "api/v1/probe/")
	{
		var probe = client.For<IProbe>();
		var ids = new List<long>();
		await foreach (var item in client.EnumerateAsync(ct => probe.GetAsync(first, ct), Ct))
		{
			ids.Add(item.Id);
		}

		return ids;
	}

	/// <summary>A stub that answers each request with the next of <paramref name="responses"/>, as 200 OK.</summary>
	private static StubHandler Pages(params string[] responses)
	{
		var stub = new StubHandler();
		foreach (var response in responses)
		{
			stub.Enqueue(HttpStatusCode.OK, response);
		}

		return stub;
	}

	/// <summary>Asserts that enumerating stops with <see cref="InvalidOperationException"/> after <paramref name="calls"/> requests.</summary>
	private static async Task EnumerationShouldStopAsync(StubHandler stub, string message, int calls)
	{
		using var client = TestClient.Create(stub);

		var act = () => EnumerateAsync(client);

		await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(message);
		stub.Calls.Should().HaveCount(calls);
	}

	[Fact]
	public async Task EnumerateAsync_FollowsNextUntilTheLastPage()
	{
		var stub = Pages(Page("/api/v1/probe/?limit=2&offset=2", 1, 2), Page("/api/v1/probe/?limit=2&offset=4", 3, 4), Page(null, 5));
		using var client = TestClient.Create(stub);

		var ids = await EnumerateAsync(client);

		ids.Should().Equal(1, 2, 3, 4, 5);
		stub.Calls.Select(c => c.Uri.PathAndQuery).Should().Equal("/api/v1/probe/", "/api/v1/probe/?limit=2&offset=2", "/api/v1/probe/?limit=2&offset=4");
		stub.Calls.Should().AllSatisfy(c => c.Authorization.Should().Be(TestClient.ApiKeyAuthorization));
	}

	[Fact]
	public async Task EnumerateAsync_MapsTheMeta()
	{
		using var client = TestClient.Create(TestClient.Stub(Page(null, 1)));

		var page = await client.For<IProbe>().GetAsync("api/v1/probe/", Ct);

		page.Meta!.Limit.Should().Be(2);
		page.Meta.Offset.Should().Be(0);
		page.Meta.Next.Should().BeNull();
		page.Meta.Previous.Should().BeNull();
		page.Meta.TotalCount.Should().Be(5);
		page.Meta.Took.Should().Be(12);
		page.Meta.ApproximateCount.Should().BeFalse();
		var item = page.Objects.Should().ContainSingle().Subject;
		item.Name.Should().Be("o1");
		item.UpdateId.Should().Be(10);
		item.ResourceUri.Should().Be("/api/v1/probe/1/");
	}

	[Theory]
	[InlineData("""{"objects":[{"id":1}]}""")]
	[InlineData("""{"meta":{"next":""},"objects":[{"id":1}]}""")]
	[InlineData("""{"meta":{"next":"/api/v1/probe/?offset=2"},"objects":[]}""")]
	public async Task EnumerateAsync_StopsWithoutANextLinkOrObjects(string json)
	{
		var stub = TestClient.Stub(json);
		using var client = TestClient.Create(stub);

		var ids = await EnumerateAsync(client);

		ids.Count.Should().BeLessThanOrEqualTo(1);
		stub.Calls.Should().ContainSingle();
	}

	[Fact]
	public async Task EnumerateAsync_KeepsTheBaseAddressPathPrefix()
	{
		var stub = Pages(Page("/api/v1/probe/?offset=2", 1), Page("/ts/api/v1/probe/?offset=3", 2), Page(null, 3));
		using var client = TestClient.Create(stub, o => o.BaseUrl = "https://threatstream.test/ts/");

		await EnumerateAsync(client);

		stub.Calls.Select(c => c.Uri.PathAndQuery).Should().Equal("/ts/api/v1/probe/", "/ts/api/v1/probe/?offset=2", "/ts/api/v1/probe/?offset=3");
	}

	[Theory]
	[InlineData("https://evil.test/api/v1/probe/?offset=2")]
	[InlineData("//evil.test/api/v1/probe/")]
	[InlineData("http://threatstream.test/api/v1/probe/")]
	[InlineData("https://threatstream.test:8443/api/v1/probe/")]
	public Task EnumerateAsync_RefusesALinkToAnotherOrigin(string next)
		=> EnumerationShouldStopAsync(Pages(Page(next, 1)), "*outside the client's base address*", 1);

	[Fact]
	public async Task EnumerateAsync_FollowsAnAbsoluteLinkToTheSameOrigin()
	{
		using var client = TestClient.Create(Pages(Page("https://THREATSTREAM.test/api/v1/probe/?offset=1", 1), Page(null, 2)));

		(await EnumerateAsync(client)).Should().Equal(1, 2);
	}

	[Fact]
	public Task EnumerateAsync_RefusesARepeatedLink()
		=> EnumerationShouldStopAsync(Pages(Page("/api/v1/probe/?offset=2", 1), Page("/api/v1/probe/?offset=2", 2)), "*already followed*", 2);

	[Fact]
	public async Task EnumerateAsync_ANextPageError_RaisesThreatStreamApiException()
	{
		var stub = new StubHandler();
		stub.Enqueue(HttpStatusCode.OK, Page("/api/v1/probe/?offset=2", 1));
		stub.Enqueue(HttpStatusCode.TooManyRequests, """{"error":"Rate limit exceeded"}""");
		using var client = TestClient.Create(stub);

		await TestClient.ShouldFailWithAsync(() => EnumerateAsync(client), HttpStatusCode.TooManyRequests, "Rate limit exceeded");
	}

	[Fact]
	public Task EnumerateAsync_ANullNextPage_RaisesInvalidOperation()
		=> EnumerationShouldStopAsync(Pages(Page("/api/v1/probe/?offset=2", 1), "null"), "*empty page*", 2);

	[Fact]
	public async Task EnumerateAsync_RequiresAFirstPage()
	{
		using var client = TestClient.Create(new StubHandler());

		var act = () => CountAsync(client.EnumerateAsync<ProbeObject>(null!, Ct));

		await act.Should().ThrowAsync<ArgumentNullException>();
	}

	private static ThreatStreamPage<ProbeObject> UpdatePage(params long[] updateIds)
		=> new() { Objects = [.. updateIds.Select(u => new ProbeObject { Id = u, UpdateId = u })] };

	private static async Task<(List<long> Seen, List<long> Requested)> ByUpdateIdAsync(Queue<ThreatStreamPage<ProbeObject>> pages, long after)
	{
		var requested = new List<long>();
		var seen = new List<long>();
		await foreach (var item in ThreatStreamClient.EnumerateByUpdateIdAsync(
			(last, _) =>
			{
				requested.Add(last);
				return Task.FromResult(pages.Dequeue());
			},
			o => o.UpdateId,
			after,
			Ct))
		{
			seen.Add(item.UpdateId);
		}

		return (seen, requested);
	}

	[Fact]
	public async Task EnumerateByUpdateIdAsync_AdvancesByTheHighestUpdateId_UntilAnEmptyPage()
	{
		var pages = new Queue<ThreatStreamPage<ProbeObject>>([UpdatePage(11, 13, 12), UpdatePage(20), UpdatePage()]);

		var (seen, requested) = await ByUpdateIdAsync(pages, 10);

		seen.Should().Equal(11, 13, 12, 20);
		requested.Should().Equal(10, 13, 20);
	}

	[Fact]
	public async Task EnumerateByUpdateIdAsync_APageThatDoesNotAdvance_Throws()
	{
		var pages = new Queue<ThreatStreamPage<ProbeObject>>([UpdatePage(5), UpdatePage(5)]);

		var act = () => ByUpdateIdAsync(pages, 0);

		await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("The page after update_id 5 did not advance*");
	}

	[Fact]
	public async Task EnumerateByUpdateIdAsync_RequiresItsDelegates()
	{
		var none = (Func<long, CancellationToken, Task<ThreatStreamPage<ProbeObject>>>? getPage, Func<ProbeObject, long>? updateIdOf)
			=> CountAsync(ThreatStreamClient.EnumerateByUpdateIdAsync(getPage!, updateIdOf!, 0, Ct));

		await none.Invoking(f => f(null, o => o.UpdateId)).Should().ThrowAsync<ArgumentNullException>();
		await none.Invoking(f => f((_, _) => Task.FromResult(UpdatePage()), null)).Should().ThrowAsync<ArgumentNullException>();
	}
}
