using System.Text.Json;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Feeds;
using ThreatStream.Api.Models.Rules;
using ThreatStream.Api.Models.Workgroups;

namespace ThreatStream.Api.Test.Core;

/// <summary>References to other objects, which ThreatStream sends as objects, bare IDs, API paths or names.</summary>
public class ThreatStreamReferenceTests
{
	private static T Read<T>(string json) => JsonSerializer.Deserialize<T>(json, ThreatStreamJson.Options)!;

	[Theory]
	[InlineData("13", 13L, null, null)]
	[InlineData("\"15\"", 15L, null, null)]
	[InlineData("\"Health Care\"", null, "Health Care", null)]
	[InlineData("\"/api/v1/organization/1/\"", null, null, "/api/v1/organization/1/")]
	public void ReadsABareIdPathOrName(string json, long? id, string? name, string? resourceUri)
	{
		var reference = Read<ThreatStreamReference>(json);

		(reference.Id, reference.Name, reference.ResourceUri).Should().Be((id, name, resourceUri));
	}

	[Fact]
	public void ReadsAnObject_KeepingUnmodelledProperties()
	{
		var reference = Read<ThreatStreamReference>("""{"id": "7", "name": "n", "value": "v", "display_name": "d", "resource_uri": "/api/v1/x/7/", "email": "e"}""");

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
	public void RejectsOtherShapes(string json)
	{
		var act = () => Read<ThreatStreamReference>(json);

		act.Should().Throw<JsonException>().WithMessage("Cannot read a reference from a JSON *");
	}

	[Fact]
	public void WritesAnObject()
	{
		var reference = Read<ThreatStreamReference>("""{"id": 7, "value": "v", "email": "e"}""");

		JsonSerializer.Serialize(reference, ThreatStreamJson.Options).Should().Be("""{"id":7,"value":"v","email":"e"}""");
	}

	[Fact]
	public void OrganisationsOfEveryCategory_ReadAnyForm()
	{
		Read<Feed>("""{"id": 1, "organization": "/api/v1/organization/9/"}""").Organization!.ResourceUri.Should().Be("/api/v1/organization/9/");
		Read<Rule>("""{"id": 1, "organization": 9, "investigation": {"id": 4, "name": "i"}}""").Organization!.Id.Should().Be(9);
		Read<Workgroup>("""{"id": 1, "owner_org": "Example Org"}""").OwnerOrg!.Name.Should().Be("Example Org");
	}
}
