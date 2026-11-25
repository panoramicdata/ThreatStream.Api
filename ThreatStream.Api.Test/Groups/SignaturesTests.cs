using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsBTestKit;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class SignaturesTests
{
	private const string Type = "signature";

	// The reference documents the attributes but gives no example response; this follows its attribute table.
	private static readonly string SignatureJson = Entity(10, Type, """
		"notes": "the definition of this CybOX signature",
		"s_type": "CybOX"
		""");

	// Inferred: the reference names api/v1/signature_type/ but documents neither it nor its response.
	private static readonly string SignatureTypesJson = Page("""{"id": 3, "resource_uri": "/api/v1/signature_type/3/", "name": "CybOX"}""");

	[Fact]
	public Task ListAsync_SendsGetWithFilters()
		=> ListShouldSendAsync((client, options, ct) => client.Signatures.ListAsync(options, ct), Type);

	[Fact]
	public async Task CreateAsync_SendsTheDocumentedExample()
	{
		var request = new SignatureCreateRequest
		{
			Name = "dummy test signature",
			Tlp = Tlp.Red,
			SignatureType = 3,
			Tags = ["big signature", "dummy tag"],
			Intelligence = [165546],
			PublicationStatus = ThreatModelPublicationStatus.Published,
			Notes = "the definition of this CybOX signature"
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Signatures.CreateAsync(request, ct), SignatureJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/signature/", body: """{"name":"dummy test signature","s_type":3,"notes":"the definition of this CybOX signature","tags":["big signature","dummy tag"],"tlp":"red","publication_status":"published","intelligence":[165546]}""");
	}

	[Fact]
	public Task GetAsync_SendsGet()
		=> GetShouldSendAsync((client, id, options, ct) => client.Signatures.GetAsync(id, options, ct), Type, SignatureJson);

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var signature = await TestClient.ReadAsync((client, ct) => client.Signatures.GetAsync(10, null, ct), SignatureJson);

		signature.ShouldHaveEntityFields(10, Type);
		signature.Notes.Should().Be("the definition of this CybOX signature");
		signature.SignatureType!.Name.Should().Be("CybOX");
	}

	[Fact]
	public async Task UpdateAsync_SendsOnlyTheChangedFields()
	{
		var request = new SignatureUpdateRequest { Name = "another dummy test signature", SignatureType = 11, Tlp = Tlp.Amber };

		var call = await TestClient.CaptureAsync((client, ct) => client.Signatures.UpdateAsync(10, request, ct), SignatureJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/signature/10/", body: """{"s_type":11,"name":"another dummy test signature","tlp":"amber"}""");
	}

	[Fact]
	public Task ListAssociatedWithValueAsync_SendsTheValue()
		=> AssociatedWithValueShouldSendAsync((client, value, options, ct) => client.Signatures.ListAssociatedWithValueAsync(value, options, ct), Type);

	[Fact]
	public Task ListAssociatedWithIdsAsync_SendsTheIds()
		=> AssociatedWithIdsShouldSendAsync((client, ids, options, ct) => client.Signatures.ListAssociatedWithIdsAsync(ids, options, ct), Type);

	[Fact]
	public Task ListLinkedAsync_SendsTheEntityType()
		=> LinkedShouldSendAsync((client, id, entityType, options, ct) => client.Signatures.ListLinkedAsync(id, entityType, options, ct), Type);

	[Fact]
	public Task ListIntelligenceAsync_SendsGet()
		=> IntelligenceShouldSendAsync((client, id, options, ct) => client.Signatures.ListIntelligenceAsync(id, options, ct), Type);

	[Fact]
	public async Task ListTypesAsync_SendsGet()
	{
		(await TestClient.CaptureAsync((client, ct) => client.Signatures.ListTypesAsync(null, ct), SignatureTypesJson)).ShouldBe(HttpMethod.Get, "/api/v1/signature_type/");
		(await TestClient.CaptureAsync((client, ct) => client.Signatures.ListTypesAsync(Paging, ct), SignatureTypesJson)).ShouldBe(HttpMethod.Get, "/api/v1/signature_type/", PagingQuery);
	}

	[Fact]
	public async Task ListTypesAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Signatures.ListTypesAsync(null, ct), SignatureTypesJson);

		var type = page.Objects.Should().ContainSingle().Subject;
		type.Id.Should().Be(3);
		type.ResourceUri.Should().Be("/api/v1/signature_type/3/");
		type.Name.Should().Be("CybOX");
	}

	[Fact]
	public async Task Writes_AreRefusedWhenReadOnly()
	{
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Signatures.CreateAsync(new SignatureCreateRequest { Name = "x", SignatureType = 3 }, ct), "POST", "api/v1/signature/");
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Signatures.UpdateAsync(10, new SignatureUpdateRequest { Name = "x" }, ct), "PATCH", "api/v1/signature/10/");
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.Signatures.GetAsync(10, null, ct));

	[Fact]
	public void Property_ReturnsTheSameInstance()
	{
		using var client = TestClient.Create(new StubHandler());

		client.Signatures.Should().BeSameAs(client.Signatures);
	}
}
