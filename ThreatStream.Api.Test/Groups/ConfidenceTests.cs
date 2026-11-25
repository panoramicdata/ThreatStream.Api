using System.Net;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class ConfidenceTests
{
	// The example response in the ThreatStream API reference (Confidence), plus an unmodelled property.
	private const string TrendJson = """
		{
			"min_confidence": 66,
			"average_confidence": "69.5",
			"max_confidence": 70,
			"type": "confidence"
		}
		""";

	[Fact]
	public async Task GetTrendAsync_SendsGetWithTheValue()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Confidence.GetTrendAsync("118.172.14.234", ct), TrendJson);

		call.ShouldBe(HttpMethod.Get, "/api/v1/inteldetails/confidence_trend/", "?value=118.172.14.234");
	}

	[Fact]
	public async Task GetTrendAsync_EscapesTheValue()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Confidence.GetTrendAsync("http://bad.example/a?b=c&d", ct), TrendJson);

		call.Uri.Query.Should().Be("?value=http%3A%2F%2Fbad.example%2Fa%3Fb%3Dc%26d");
	}

	[Fact]
	public async Task GetTrendAsync_MapsEveryField()
	{
		var trend = await TestClient.ReadAsync((client, ct) => client.Confidence.GetTrendAsync("118.172.14.234", ct), TrendJson);

		trend.MinConfidence.Should().Be(66);
		trend.AverageConfidence.Should().Be(69.5);
		trend.MaxConfidence.Should().Be(70);
		trend.AdditionalProperties["type"].GetString().Should().Be("confidence");
	}

	[Fact]
	public async Task GetTrendAsync_UnknownObservable_ReadsAsNulls()
	{
		var trend = await TestClient.ReadAsync(
			(client, ct) => client.Confidence.GetTrendAsync("unknown.example", ct),
			"""{"min_confidence": null, "average_confidence": "", "max_confidence": null}""");

		trend.MinConfidence.Should().BeNull();
		trend.AverageConfidence.Should().BeNull();
		trend.MaxConfidence.Should().BeNull();
	}

	[Fact]
	public Task GetTrendAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Confidence.GetTrendAsync("118.172.14.234", ct),
			HttpStatusCode.Unauthorized,
			"""{"message": "Invalid credentials"}""",
			"Invalid credentials");
}
