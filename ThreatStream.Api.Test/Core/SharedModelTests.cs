using System.Text.Json;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Test.Core;

/// <summary>The shared models every category uses: tags and TLP designations.</summary>
public class SharedModelTests
{
	[Theory]
	[InlineData("red", Tlp.Red)]
	[InlineData("amber", Tlp.Amber)]
	[InlineData("amber+strict", Tlp.AmberStrict)]
	[InlineData("green", Tlp.Green)]
	[InlineData("clear", Tlp.Clear)]
	[InlineData("white", Tlp.White)]
	[InlineData("purple", Tlp.Unknown)]
	public void Tlp_ReadsEveryDesignation(string wire, Tlp expected)
		=> JsonSerializer.Deserialize<Tlp>($"\"{wire}\"", ThreatStreamJson.Options).Should().Be(expected);

	[Fact]
	public void Tag_ReadsWhatThreatStreamSends()
	{
		var tag = JsonSerializer.Deserialize<ThreatStreamTag>(
			"""{"id":"q1w2e3","name":"apt","tlp":"amber+strict","org_id":"42","source_user_id":"7"}""",
			ThreatStreamJson.Options)!;

		tag.Id.Should().Be("q1w2e3");
		tag.Name.Should().Be("apt");
		tag.Tlp.Should().Be(Tlp.AmberStrict);
		tag.OrgId.Should().Be(42);
		tag.AdditionalProperties["source_user_id"].GetString().Should().Be("7");
	}

	[Fact]
	public void Tag_WritesOnlyWhatIsSet()
		=> JsonSerializer.Serialize(new ThreatStreamTag { Name = "apt", Tlp = Tlp.Red }, ThreatStreamJson.Options)
			.Should().Be("""{"name":"apt","tlp":"red"}""");
}
