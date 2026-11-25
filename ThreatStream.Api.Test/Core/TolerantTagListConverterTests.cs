using System.Text.Json;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Models.Rules;
using ThreatStream.Api.Models.ThreatModelSearch;

namespace ThreatStream.Api.Test.Core;

/// <summary>Tag lists, which ThreatStream sends as plain names, tag objects or a mix, on every model that has one.</summary>
public class TolerantTagListConverterTests
{
	private const string NamesOnly = """{"tags": ["apt", "phishing"]}""";

	private const string Objects = """{"tags": [{"id": "q1", "name": "apt", "tlp": "red", "org_id": "42"}, {"name": "phishing"}]}""";

	private const string Mixed = """{"tags": ["apt", null, {"name": "phishing", "tlp": "amber"}]}""";

	[Theory]
	[InlineData(NamesOnly)]
	[InlineData(Objects)]
	[InlineData(Mixed)]
	public void EveryModelWithTags_ReadsNamesAndObjects(string json)
	{
		Names(Read<ThreatModelSearchResult>(json).Tags).Should().Equal("apt", "phishing");
		Names(Read<Observable>(json).Tags).Should().Equal("apt", "phishing");
		Names(Read<Rule>(json).Tags).Should().Equal("apt", "phishing");
	}

	[Fact]
	public void NamesOnly_SetOnlyTheName()
	{
		var tag = Read<ThreatModelSearchResult>(NamesOnly).Tags[0];

		(tag.Name, tag.Id, tag.Tlp, tag.OrgId).Should().Be(("apt", null, null, null));
	}

	[Fact]
	public void Objects_KeepEveryField()
	{
		var tag = Read<ThreatModelSearchResult>(Objects).Tags[0];

		(tag.Name, tag.Id, tag.Tlp, tag.OrgId).Should().Be(("apt", "q1", Tlp.Red, 42L));
	}

	[Fact]
	public void Null_KeepsTheEmptyList()
		=> Read<ThreatModelSearchResult>("""{"tags": null}""").Tags.Should().BeEmpty();

	[Theory]
	[InlineData("""{"tags": "apt"}""", "Cannot read tags from a JSON String.")]
	[InlineData("""{"tags": {"name": "apt"}}""", "Cannot read tags from a JSON StartObject.")]
	public void NotAList_Throws(string json, string message)
	{
		var act = () => Read<ThreatModelSearchResult>(json);

		act.Should().Throw<JsonException>().WithMessage(message);
	}

	[Fact]
	public void Write_SendsTagObjects()
	{
		IReadOnlyList<ThreatStreamTag> tags = [new() { Name = "apt", Tlp = Tlp.Red }, new() { Name = "phishing" }];

		JsonSerializer.Serialize(tags, ThreatStreamJson.Options).Should().Be("""[{"name":"apt","tlp":"red"},{"name":"phishing"}]""");
	}

	private static T Read<T>(string json) => JsonSerializer.Deserialize<T>(json, ThreatStreamJson.Options)!;

	private static IEnumerable<string> Names(IReadOnlyList<ThreatStreamTag> tags) => tags.Select(t => t.Name);
}
