using System.Net;
using ThreatStream.Api.Models.Workgroups;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class ObservableWorkgroupsTests
{
	// The response the ThreatStream API reference (Bulk Workgroups API) describes: the workgroups given and success.
	private const string ResultJson = """{"workgroups": [2, 5], "success": true}""";

	private static readonly BulkWorkgroupsRequest Request = new() { Ids = [5873243, 5873244], Workgroups = [2, 5] };

	[Fact]
	public async Task RestrictAsync_PostsTheIdsAndWorkgroups()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ObservableWorkgroups.RestrictAsync(Request, ct), ResultJson);

		call.ShouldBe(HttpMethod.Post, "/api/v2/intelligence/bulk_workgroups/", body: """{"ids":[5873243,5873244],"workgroups":[2,5]}""");
	}

	[Fact]
	public async Task RestrictAsync_MapsEveryField()
	{
		var result = (await TestClient.ReadAsync((client, ct) => client.ObservableWorkgroups.RestrictAsync(Request, ct), ResultJson))!;

		result.Workgroups.Should().Equal(2, 5);
		result.Success.Should().BeTrue();
	}

	[Fact]
	public Task RestrictAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ObservableWorkgroups.RestrictAsync(Request, ct), "POST", "api/v2/intelligence/bulk_workgroups/");

	[Fact]
	public Task RestrictAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ObservableWorkgroups.RestrictAsync(Request, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "Invalid workgroup"}""",
			"Invalid workgroup");
}
