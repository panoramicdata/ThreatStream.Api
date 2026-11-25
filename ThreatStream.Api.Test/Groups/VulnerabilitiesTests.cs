using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsBTestKit;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public class VulnerabilitiesTests
{
	private const string Type = "vulnerability";

	// The reference documents the attributes but gives no example response; this follows its attribute table.
	private static readonly string VulnerabilityJson = Entity(15, Type, """
		"is_system": true,
		"source": "NVD"
		""");

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

		call.ShouldBe(HttpMethod.Post, "/api/v1/vulnerability/", body: """{"name":"dummy test vulnerability","definition":"the definition of this vulnerability","tags":["big vulnerability","dummy tag"],"tlp":"red","publication_status":"published"}""");
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
	}

	[Fact]
	public async Task UpdateAsync_SendsTheDocumentedExample()
	{
		var request = new VulnerabilityUpdateRequest { Name = "another dummy test vulnerability", Tags = ["December"] };

		var call = await TestClient.CaptureAsync((client, ct) => client.Vulnerabilities.UpdateAsync(15, request, ct), VulnerabilityJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/vulnerability/15/", body: """{"name":"another dummy test vulnerability","tags":["December"]}""");
	}

	[Fact]
	public Task ListAssociatedWithValueAsync_SendsTheValue()
		=> AssociatedWithValueShouldSendAsync((client, value, options, ct) => client.Vulnerabilities.ListAssociatedWithValueAsync(value, options, ct), Type);

	[Fact]
	public Task ListAssociatedWithIdsAsync_SendsTheIds()
		=> AssociatedWithIdsShouldSendAsync((client, ids, options, ct) => client.Vulnerabilities.ListAssociatedWithIdsAsync(ids, options, ct), Type);

	[Fact]
	public Task ListLinkedAsync_SendsTheEntityType()
		=> LinkedShouldSendAsync((client, id, entityType, options, ct) => client.Vulnerabilities.ListLinkedAsync(id, entityType, options, ct), Type);

	[Fact]
	public Task ListIntelligenceAsync_SendsGet()
		=> IntelligenceShouldSendAsync((client, id, options, ct) => client.Vulnerabilities.ListIntelligenceAsync(id, options, ct), Type);

	[Fact]
	public async Task Writes_AreRefusedWhenReadOnly()
	{
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Vulnerabilities.CreateAsync(new VulnerabilityCreateRequest { Name = "x" }, ct), "POST", "api/v1/vulnerability/");
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Vulnerabilities.UpdateAsync(15, new VulnerabilityUpdateRequest { Name = "x" }, ct), "PATCH", "api/v1/vulnerability/15/");
	}

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.Vulnerabilities.GetAsync(15, null, ct));

	[Fact]
	public void Property_ReturnsTheSameInstance()
	{
		using var client = TestClient.Create(new StubHandler());

		client.Vulnerabilities.Should().BeSameAs(client.Vulnerabilities);
	}
}
