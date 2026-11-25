using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableImports;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class StixImportsTests
{
	private static StixImportRequest Request(string json = """{"type":"bundle"}""") => new() { File = new ThreatStreamFile("bundle.json", System.Text.Encoding.UTF8.GetBytes(json), "application/json") };

	[Fact]
	public async Task ImportAsync_SendsMultipartWithTheFileAndSettings()
	{
		var request = new StixImportRequest
		{
			File = new ThreatStreamFile("indicators.xml", "<stix:STIX_Package/>"u8.ToArray(), "application/xml"),
			Classification = "private",
			Confidence = 60,
			IsAnonymous = false,
			Tags = [new ThreatStreamTag { Name = "my_tag", Tlp = Tlp.Red }],
			TrustedCircles = [1, 2]
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.StixImports.ImportAsync(request, ct), "{}");

		call.ShouldBe(HttpMethod.Post, "/api/v1/stix_import/");
		PartsOf(call).Should().Equal(
			("file", "indicators.xml", "application/xml", "<stix:STIX_Package/>"),
			Field("classification", "private"),
			Field("confidence", "60"),
			Field("is_anonymous", "false"),
			Field("tags", """[{"name":"my_tag","tlp":"red"}]"""),
			Field("trustedcircles", "1,2"));
	}

	[Fact]
	public async Task ImportAsync_FileOnly_SendsOnePart()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.StixImports.ImportAsync(Request(), ct), string.Empty);

		PartsOf(call).Should().Equal(("file", "bundle.json", "application/json", """{"type":"bundle"}"""));
	}

	[Fact]
	public Task ImportAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.StixImports.ImportAsync(Request(), ct), "POST", "api/v1/stix_import/");

	[Fact]
	public Task ImportAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.StixImports.ImportAsync(Request(), ct),
			HttpStatusCode.BadRequest,
			"""{"message": "Invalid STIX"}""",
			"Invalid STIX");
}
