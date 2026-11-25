using System.Text.Json;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

/// <summary>The shared threat model types: references, tags, the associated-entity model and the enums.</summary>
public class ThreatModelsTests
{
	private static T Read<T>(string json) => JsonSerializer.Deserialize<T>(json, ThreatStreamJson.Options)!;

	[Theory]
	[InlineData("13", 13L, null)]
	[InlineData("\"15\"", 15L, null)]
	[InlineData("\"Health Care\"", null, "Health Care")]
	public void Reference_ReadsABareIdOrName(string json, long? id, string? name)
	{
		var reference = Read<ThreatModelReference>(json);

		reference.Id.Should().Be(id);
		reference.Name.Should().Be(name);
	}

	[Fact]
	public void Reference_ReadsAnObject_KeepingUnmodelledProperties()
	{
		var reference = Read<ThreatModelReference>("""{"id": "7", "name": "n", "value": "v", "display_name": "d", "resource_uri": "/api/v1/x/7/", "email": "e"}""");

		reference.Id.Should().Be(7);
		reference.Name.Should().Be("n");
		reference.Value.Should().Be("v");
		reference.DisplayName.Should().Be("d");
		reference.ResourceUri.Should().Be("/api/v1/x/7/");
		reference.AdditionalProperties["email"].GetString().Should().Be("e");
	}

	[Theory]
	[InlineData("true")]
	[InlineData("[1]")]
	public void Reference_RejectsOtherShapes(string json)
	{
		var act = () => Read<ThreatModelReference>(json);

		act.Should().Throw<JsonException>().WithMessage("Cannot read a threat model reference*");
	}

	[Fact]
	public void Reference_WritesAnObject()
	{
		var reference = Read<ThreatModelReference>("""{"id": 7, "value": "v", "email": "e"}""");

		JsonSerializer.Serialize(reference, ThreatStreamJson.Options).Should().Be("""{"id":7,"value":"v","email":"e"}""");
	}

	[Fact]
	public void Tags_RejectAnythingButAList()
	{
		var act = () => Read<ThreatModelAssociatedEntity>("""{"id": 1, "tags": "apt"}""");

		act.Should().Throw<JsonException>().WithMessage("Cannot read tags*");
	}

	[Fact]
	public void Tags_NullKeepsTheEmptyList()
		=> Read<ThreatModelAssociatedEntity>("""{"id": 1, "tags": null}""").Tags.Should().BeEmpty();

	[Fact]
	public void Tags_WriteAsTagObjects()
	{
		var entity = Read<ThreatModelAssociatedEntity>("""{"id": 1, "tags": ["apt"], "tags_v2": [{"name": "p", "tlp": "red"}]}""");

		var json = JsonSerializer.Serialize(entity, ThreatStreamJson.Options);

		json.Should().Contain("""
			"tags":[{"name":"apt"}],"tags_v2":[{"name":"p","tlp":"red"}]
			""".Trim());
	}

	private static readonly string[] PublicationStatuses = ["new", "pending_review", "review_requested", "reviewed", "published", "archived"];

	[Fact]
	public void PublicationStatus_ReadsEveryValue_AndUnknownOnes()
		=> PublicationStatuses
			.Select(s => Read<ThreatModelPublicationStatus>($"\"{s}\""))
			.Should().Equal(
				ThreatModelPublicationStatus.New,
				ThreatModelPublicationStatus.PendingReview,
				ThreatModelPublicationStatus.ReviewRequested,
				ThreatModelPublicationStatus.Reviewed,
				ThreatModelPublicationStatus.Published,
				ThreatModelPublicationStatus.Unknown);

	[Fact]
	public async Task ListLinkedAsync_MapsTheSharedAttributes_AndKeepsTheRest()
	{
		var json = Page(Entity(234674, "malware", """
			"is_family": true,
			"malware_types": ["screen-capture"]
			"""));

		var page = await TestClient.ReadAsync((client, ct) => client.Actors.ListLinkedAsync(234673, ThreatModelType.Malware, null, ct), json);

		var entity = page.Objects.Should().ContainSingle().Subject;
		entity.ShouldHaveEntityFields(234674, "malware");
		entity.AdditionalProperties["is_family"].GetBoolean().Should().BeTrue();
	}

	[Fact]
	public async Task ListLinkedAsync_SendsEveryTypeByItsPathName()
	{
		string[] expected = ["actor", "attackpattern", "campaign", "incident", "malware", "signature", "tipreport", "ttp", "vulnerability"];
		var types = Enum.GetValues<ThreatModelType>().Where(t => t != ThreatModelType.Unknown).ToList();

		var paths = new List<string>();
		foreach (var type in types)
		{
			var call = await TestClient.CaptureAsync((client, ct) => client.Campaigns.ListLinkedAsync(1, type, null, ct), EmptyPage);
			paths.Add(call.Uri.AbsolutePath);
		}

		paths.Should().Equal(expected.Select(e => $"/api/v1/campaign/1/{e}/"));
	}
}
