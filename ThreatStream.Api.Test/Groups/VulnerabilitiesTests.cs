using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsBTestKit;

namespace ThreatStream.Api.Test.Groups;

public class VulnerabilitiesTests
{
	private const string Type = "vulnerability";

	// The reference documents the attributes but gives no example response; this follows its attribute table.
	private static readonly string VulnerabilityJson = $$"""
		{
			{{EntityFields(15, Type)}},
			"is_system": true,
			"source": "NVD",
			"remote_api": false
		}
		""";

	[Fact]
	public Task ListAsync_SendsGetWithFilters()
		=> ListShouldSendAsync((client, options, ct) => client.Vulnerabilities.ListAsync(options, ct), Type);

	[Fact]
	public async Task CreateAsync_SendsTheDocumentedExample()
	{
		var request = new VulnerabilityCreateRequest
		{
			Name = "dummy test vulnerability",
			Tlp = Tlp.Red,
			Tags = ["big vulnerability", "dummy tag"],
			PublicationStatus = ThreatModelPublicationStatus.Published,
			Definition = "the definition of this vulnerability"
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Vulnerabilities.CreateAsync(request, ct), VulnerabilityJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/vulnerability/", body: """{"name":"dummy test vulnerability","publication_status":"published","definition":"the definition of this vulnerability","tags":["big vulnerability","dummy tag"],"tlp":"red"}""");
	}

	[Fact]
	public Task GetAsync_SendsGet()
		=> GetShouldSendAsync((client, id, options, ct) => client.Vulnerabilities.GetAsync(id, options, ct), Type, VulnerabilityJson);

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var vulnerability = await TestClient.ReadAsync((client, ct) => client.Vulnerabilities.GetAsync(15, null, ct), VulnerabilityJson);

		vulnerability.ShouldHaveEntityFields(15, Type);
		vulnerability.IsSystem.Should().BeTrue();
		vulnerability.Source.Should().Be("NVD");
		vulnerability.RemoteApi.Should().BeFalse();
	}

	[Fact]
	public async Task UpdateAsync_SendsTheDocumentedExample()
	{
		var request = new VulnerabilityUpdateRequest { Name = "another dummy test vulnerability", Description = "d", Tags = ["December"] };

		var call = await TestClient.CaptureAsync((client, ct) => client.Vulnerabilities.UpdateAsync(15, request, ct), VulnerabilityJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/vulnerability/15/", body: """{"name":"another dummy test vulnerability","description":"d","tags":["December"]}""");
	}

	[Fact]
	public Task ListAssociatedWithIntelligenceAsync_SendsTheValue()
		=> AssociatedByValueShouldSendAsync((client, value, options, ct) => client.Vulnerabilities.ListAssociatedWithIntelligenceAsync(value, options, ct), Type);

	[Fact]
	public Task ListAssociatedWithIntelligenceByIdsAsync_SendsTheIds()
		=> AssociatedByIdsShouldSendAsync((client, ids, options, ct) => client.Vulnerabilities.ListAssociatedWithIntelligenceByIdsAsync(ids, options, ct), Type);

	[Fact]
	public Task ListLinkedEntitiesAsync_SendsTheEntityType()
		=> LinkedEntitiesShouldSendAsync((client, id, entityType, options, ct) => client.Vulnerabilities.ListLinkedEntitiesAsync(id, entityType, options, ct), Type);

	[Fact]
	public Task ListIntelligenceAsync_SendsGet()
		=> IntelligenceShouldSendAsync((client, id, options, ct) => client.Vulnerabilities.ListIntelligenceAsync(id, options, ct), Type);

	[Fact]
	public async Task Writes_AreRefusedWhenReadOnly()
	{
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Vulnerabilities.CreateAsync(new VulnerabilityCreateRequest { Name = "x" }, ct));
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Vulnerabilities.UpdateAsync(15, new VulnerabilityUpdateRequest { Name = "x" }, ct));
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> NotFoundShouldFailAsync((client, ct) => client.Vulnerabilities.GetAsync(15, null, ct));

	[Fact]
	public void Property_ReturnsTheSameInstance()
	{
		using var client = TestClient.Create(new StubHandler());

		client.Vulnerabilities.Should().BeSameAs(client.Vulnerabilities);
	}
}
