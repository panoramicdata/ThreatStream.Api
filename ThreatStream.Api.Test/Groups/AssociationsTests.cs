using System.Net;
using ThreatStream.Api.Models.Associations;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public class AssociationsTests
{
	private static readonly AssociationRequest Observables = new() { Ids = [54, 67] };

	private static readonly AssociationRequest Signature = new() { Ids = [123] };

	[Fact]
	public async Task BulkAddAsync_PostsTheIds()
	{
		// The reference's example: associate threat bulletin 744 with observables 54 and 67.
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.Associations.BulkAddAsync(AssociationEntityType.TipReport, 744, AssociationEntityType.Intelligence, Observables, ct),
			"{}");

		call.ShouldBe(HttpMethod.Post, "/api/v1/tipreport/744/intelligence/bulk_add/", body: """{"ids":[54,67]}""");
	}

	[Fact]
	public async Task BulkRemoveAsync_PostsTheIds()
	{
		// The reference's example: dissociate TTP 67 from signature 123.
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.Associations.BulkRemoveAsync(AssociationEntityType.Ttp, 67, AssociationEntityType.Signature, Signature, ct),
			"{}");

		call.ShouldBe(HttpMethod.Post, "/api/v1/ttp/67/signature/bulk_remove/", body: """{"ids":[123]}""");
	}

	[Theory]
	[InlineData(AssociationEntityType.Actor, "actor")]
	[InlineData(AssociationEntityType.AttackPattern, "attackpattern")]
	[InlineData(AssociationEntityType.Campaign, "campaign")]
	[InlineData(AssociationEntityType.CourseOfAction, "courseofaction")]
	[InlineData(AssociationEntityType.Identity, "identity")]
	[InlineData(AssociationEntityType.Infrastructure, "infrastructure")]
	[InlineData(AssociationEntityType.IntrusionSet, "intrusionset")]
	[InlineData(AssociationEntityType.Incident, "incident")]
	[InlineData(AssociationEntityType.Malware, "malware")]
	[InlineData(AssociationEntityType.Signature, "signature")]
	[InlineData(AssociationEntityType.TipReport, "tipreport")]
	[InlineData(AssociationEntityType.Tool, "tool")]
	[InlineData(AssociationEntityType.Ttp, "ttp")]
	[InlineData(AssociationEntityType.Vulnerability, "vulnerability")]
	public async Task BulkAddAsync_SpellsEveryEntityType(AssociationEntityType type, string wire)
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.Associations.BulkAddAsync(type, 1, type, Signature, ct),
			"{}");

		call.ShouldBe(HttpMethod.Post, $"/api/v1/{wire}/1/{wire}/bulk_add/", body: """{"ids":[123]}""");
	}

	[Fact]
	public Task BulkAddAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.Associations.BulkAddAsync(AssociationEntityType.TipReport, 744, AssociationEntityType.Intelligence, Observables, ct),
			"POST");

	[Fact]
	public Task BulkRemoveAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync(
			(client, ct) => client.Associations.BulkRemoveAsync(AssociationEntityType.Ttp, 67, AssociationEntityType.Signature, Signature, ct),
			"POST");

	[Fact]
	public Task BulkAddAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Associations.BulkAddAsync(AssociationEntityType.TipReport, 744, AssociationEntityType.Intelligence, Observables, ct),
			HttpStatusCode.NotFound,
			"""{"detail": "Not found."}""",
			"Not found.");
}
