using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableTags;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class ObservableTagsTests
{
	// The job in the ThreatStream API reference's (Observable Tags in Bulk) asynchronous example, with an ID filled in.
	private const string JobJson = """
		{
			"created_ts": "2022-11-01T21:10:56.008462",
			"id": 3001,
			"modified_ts": "2022-11-01T21:10:56.008475",
			"path": "/api/v2/intelligence/bulk_tagging/",
			"resource_uri": "/api/v1/generic_async_result/3001/",
			"response_status": null,
			"status": "inprogress"
		}
		""";

	// The reference's add example: a private tag on five observables.
	private static readonly ObservableTagRequest AddExample = new() { Ids = [61, 52, 8, 50, 34], Tags = [new ThreatStreamTag { Name = "test", Tlp = Tlp.Red }] };

	// The reference's asynchronous example: add my_tag and remove my_tag2 on the active observables created since a date.
	private static readonly ObservableTagQueryRequest QueryExample = new()
	{
		Query = new ObservableTagQuery { Q = "(status = active) and (created_ts >= 2022-10-01T23:00:00)", Limit = 10000, OrderBy = "-created_ts" },
		RemoveTags = [new ThreatStreamTag { Name = "my_tag2", Tlp = Tlp.Red }],
		Tags = [new ThreatStreamTag { Name = "my_tag", Tlp = Tlp.Red }]
	};

	[Fact]
	public async Task BulkTagAsync_PostsTheIdsAndTags()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableTags.BulkTagAsync(AddExample, ct), "{}");

		call.ShouldBe(HttpMethod.Post, "/api/v2/intelligence/bulk_tagging/", body: """{"ids":[61,52,8,50,34],"tags":[{"name":"test","tlp":"red"}]}""");
	}

	[Fact]
	public async Task BulkTagAsync_SendsTagsToRemove()
	{
		var request = new ObservableTagRequest { Ids = [776, 349], RemoveTags = [new ThreatStreamTag { Name = "test" }, new ThreatStreamTag { Name = "tagB" }] };

		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableTags.BulkTagAsync(request, ct), "{}");

		call.ShouldBe(HttpMethod.Post, "/api/v2/intelligence/bulk_tagging/", body: """{"ids":[776,349],"remove_tags":[{"name":"test"},{"name":"tagB"}]}""");
	}

	[Fact]
	public async Task BulkTagMatchingAsync_PostsTheQueryAsynchronously()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableTags.BulkTagMatchingAsync(QueryExample, ct), JobJson);

		// System.Text.Json escapes ">" as a \u escape (code point 3E) by default; ThreatStream reads both forms as the same string.
		var body = """{"query":{"q":"(status = active) and (created_ts >= 2022-10-01T23:00:00)","limit":10000,"order_by":"-created_ts"},"tags":[{"name":"my_tag","tlp":"red"}],"remove_tags":[{"name":"my_tag2","tlp":"red"}]}""";
		call.ShouldBe(
			HttpMethod.Post,
			"/api/v2/intelligence/bulk_tagging/",
			"?async_result=true",
			body.Replace(">", '\\' + "u003E", StringComparison.Ordinal));
	}

	[Fact]
	public async Task BulkTagMatchingAsync_SendsOnlyTheQueryWhenThatIsAllThereIs()
	{
		var request = new ObservableTagQueryRequest { Query = new ObservableTagQuery { Q = "itype = mal_ip" } };

		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableTags.BulkTagMatchingAsync(request, ct), JobJson);

		call.Body.Should().Be("""{"query":{"q":"itype = mal_ip"}}""");
	}

	[Fact]
	public async Task BulkTagMatchingAsync_MapsEveryField()
	{
		var job = (await TestClient.ReadAsync((client, ct) => client.ObservableTags.BulkTagMatchingAsync(QueryExample, ct), JobJson))!;

		job.Id.Should().Be(3001);
		job.ResourceUri.Should().Be("/api/v1/generic_async_result/3001/");
		job.Status.Should().Be("inprogress");
		job.ResponseStatus.Should().BeNull();
		job.Path.Should().Be("/api/v2/intelligence/bulk_tagging/");
		job.CreatedTs.Should().Be(new DateTimeOffset(2022, 11, 1, 21, 10, 56, TimeSpan.Zero).AddTicks(84620));
		job.ModifiedTs.Should().Be(new DateTimeOffset(2022, 11, 1, 21, 10, 56, TimeSpan.Zero).AddTicks(84750));
	}

	[Fact]
	public async Task BulkTagMatchingAsync_MapsAFinishedJob()
	{
		var job = (await TestClient.ReadAsync(
			(client, ct) => client.ObservableTags.BulkTagMatchingAsync(QueryExample, ct),
			"""{"id": 3001, "response_status": "200", "status": "done"}"""))!;

		job.Status.Should().Be("done");
		job.ResponseStatus.Should().Be(200);
	}

	[Fact]
	public Task BulkTagAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ObservableTags.BulkTagAsync(AddExample, ct), "POST", "api/v2/intelligence/bulk_tagging/");

	[Fact]
	public Task BulkTagMatchingAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ObservableTags.BulkTagMatchingAsync(QueryExample, ct), "POST", "api/v2/intelligence/bulk_tagging/");

	[Fact]
	public Task BulkTagMatchingAsync_JobAlreadyRunning_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ObservableTags.BulkTagMatchingAsync(QueryExample, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "Another bulk tagging job is in progress"}""",
			"Another bulk tagging job is in progress");
}
