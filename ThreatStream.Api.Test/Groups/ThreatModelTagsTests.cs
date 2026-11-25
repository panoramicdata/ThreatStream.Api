using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class ThreatModelTagsTests
{
	// The ThreatStream API reference's (Threat Model Entity Tags in Bulk) example: tag three actors and remove tag4.
	private static readonly BulkTagRequest Example = new()
	{
		Ids = [655, 2784, 2785],
		Tags =
		[
			new ThreatStreamTag { Name = "private_tag1", Tlp = Tlp.Red },
			new ThreatStreamTag { Name = "public_tag", Tlp = Tlp.White },
			new ThreatStreamTag { Name = "private_tag3", Tlp = Tlp.Red }
		],
		RemoveTags = [new ThreatStreamTag { Name = "tag4" }]
	};

	private static readonly BulkTagRequest AddOnly = new() { Ids = [1], Tags = [new ThreatStreamTag { Name = "t" }] };

	[Fact]
	public async Task BulkTagAsync_PostsTheIdsAndTags()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatModelTags.BulkTagAsync(ThreatModelType.Actor, Example, ct), "{}");

		call.ShouldBe(
			HttpMethod.Post,
			"/api/v1/actor/bulk_tagging/",
			body: """{"ids":[655,2784,2785],"tags":[{"name":"private_tag1","tlp":"red"},{"name":"public_tag","tlp":"white"},{"name":"private_tag3","tlp":"red"}],"remove_tags":[{"name":"tag4"}]}""");
	}

	[Theory]
	[InlineData(ThreatModelType.Actor, "actor")]
	[InlineData(ThreatModelType.AttackPattern, "attackpattern")]
	[InlineData(ThreatModelType.Campaign, "campaign")]
	[InlineData(ThreatModelType.Incident, "incident")]
	[InlineData(ThreatModelType.Malware, "malware")]
	[InlineData(ThreatModelType.Signature, "signature")]
	[InlineData(ThreatModelType.ThreatBulletin, "tipreport")]
	[InlineData(ThreatModelType.Ttp, "ttp")]
	[InlineData(ThreatModelType.Vulnerability, "vulnerability")]
	public async Task BulkTagAsync_SpellsEveryEntityType(ThreatModelType type, string wire)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatModelTags.BulkTagAsync(type, AddOnly, ct), "{}");

		call.ShouldBe(HttpMethod.Post, $"/api/v1/{wire}/bulk_tagging/", body: """{"ids":[1],"tags":[{"name":"t"}]}""");
	}

	[Fact]
	public Task BulkTagAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatModelTags.BulkTagAsync(ThreatModelType.Actor, Example, ct), "POST", "api/v1/actor/bulk_tagging/");

	[Fact]
	public Task BulkTagAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ThreatModelTags.BulkTagAsync(ThreatModelType.Actor, Example, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "Too many tags to remove"}""",
			"Too many tags to remove");
}
