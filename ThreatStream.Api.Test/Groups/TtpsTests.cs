using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsBTestKit;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class TtpsTests
{
	private const string Type = "ttp";

	// The reference documents the attributes but gives no example response; this follows its attribute table.
	private static readonly string TtpJson = Entity(1774, Type, """
		"actors": [{"id": 1, "name": "Example Actor"}]
		""");

	[Fact]
	public Task ListAsync_SendsGetWithFilters()
		=> ListShouldSendAsync((client, options, ct) => client.Ttps.ListAsync(options, ct), Type);

	[Fact]
	public async Task CreateAsync_SendsTheDocumentedExample()
	{
		var request = new TtpCreateRequest
		{
			Name = "dummy test ttp",
			Tlp = Tlp.Red,
			Tags = ["big ttp", "dummy tag"],
			Intelligence = [165546],
			Signatures = [10],
			PublicationStatus = ThreatModelPublicationStatus.Published
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Ttps.CreateAsync(request, ct), TtpJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/ttp/", body: """{"name":"dummy test ttp","tags":["big ttp","dummy tag"],"tlp":"red","publication_status":"published","intelligence":[165546],"signatures":[10]}""");
	}

	[Fact]
	public Task GetAsync_SendsGet()
		=> GetShouldSendAsync((client, id, options, ct) => client.Ttps.GetAsync(id, options, ct), Type, TtpJson);

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var ttp = await TestClient.ReadAsync((client, ct) => client.Ttps.GetAsync(1774, null, ct), TtpJson);

		ttp.ShouldHaveEntityFields(1774, Type);
		ttp.Actors.Should().ContainSingle().Which.Name.Should().Be("Example Actor");
	}

	[Fact]
	public async Task UpdateAsync_SendsEveryField()
	{
		var request = new TtpUpdateRequest
		{
			Name = "another dummy test ttp",
			Description = "d",
			Actors = [1],
			Tlp = Tlp.Amber,
			IsPublic = true
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Ttps.UpdateAsync(1774, request, ct), TtpJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/ttp/1774/", body: """{"actors":[1],"name":"another dummy test ttp","description":"d","tlp":"amber","is_public":true}""");
	}

	[Fact]
	public Task ListAssociatedWithValueAsync_SendsTheValue()
		=> AssociatedWithValueShouldSendAsync((client, value, options, ct) => client.Ttps.ListAssociatedWithValueAsync(value, options, ct), Type);

	[Fact]
	public Task ListAssociatedWithIdsAsync_SendsTheIds()
		=> AssociatedWithIdsShouldSendAsync((client, ids, options, ct) => client.Ttps.ListAssociatedWithIdsAsync(ids, options, ct), Type);

	[Fact]
	public Task ListLinkedAsync_SendsTheEntityType()
		=> LinkedShouldSendAsync((client, id, entityType, options, ct) => client.Ttps.ListLinkedAsync(id, entityType, options, ct), Type);

	[Fact]
	public Task ListIntelligenceAsync_SendsGet()
		=> IntelligenceShouldSendAsync((client, id, options, ct) => client.Ttps.ListIntelligenceAsync(id, options, ct), Type);

	[Fact]
	public async Task Writes_AreRefusedWhenReadOnly()
	{
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Ttps.CreateAsync(new TtpCreateRequest { Name = "x" }, ct), "POST", "api/v1/ttp/");
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Ttps.UpdateAsync(1774, new TtpUpdateRequest { Name = "x" }, ct), "PATCH", "api/v1/ttp/1774/");
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.Ttps.GetAsync(1774, null, ct));

	[Fact]
	public void Property_ReturnsTheSameInstance()
	{
		using var client = TestClient.Create(new StubHandler());

		client.Ttps.Should().BeSameAs(client.Ttps);
	}
}
