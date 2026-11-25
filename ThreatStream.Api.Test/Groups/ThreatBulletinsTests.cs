using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsBTestKit;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class ThreatBulletinsTests
{
	private const string Type = "tipreport";

	// The reference documents the attributes but gives no example response; this follows its attribute table, with the
	// deprecated attributes as it says ThreatStream sends them.
	private static readonly string ThreatBulletinJson = Entity(11, Type, """
		"body": "this is a new threat bulletin",
		"body_content_type": "richtext",
		"status": "review_requested",
		"assignee_org_id": "42",
		"assignee_user_id": 7,
		"owner_org_id": 42,
		"source": "USCERT",
		"attachments": [{"id": 5, "name": "report.pdf"}],
		"import_sessions": [301],
		"sandbox_reports": ["77"],
		"comments": [],
		"history": [],
		"threat_actor": null,
		"ttp": null,
		"vulnerability": null
		""");

	[Fact]
	public Task ListAsync_SendsGetWithFilters()
		=> ListShouldSendAsync((client, options, ct) => client.ThreatBulletins.ListAsync(options, ct), Type);

	private static async Task CreateShouldSendAsync(ThreatBulletinCreateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatBulletins.CreateAsync(request, ct), ThreatBulletinJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/tipreport/", body: body);
	}

	private static async Task UpdateShouldSendAsync(ThreatBulletinUpdateRequest request, string body)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatBulletins.UpdateAsync(11, request, ct), ThreatBulletinJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/tipreport/11/", body: body);
	}

	[Fact]
	public Task CreateAsync_SendsTheDocumentedExample()
		=> CreateShouldSendAsync(
			new ThreatBulletinCreateRequest { Body = "this is a new threat bulletin", Name = "New Threat Bulletin" },
			"""{"name":"New Threat Bulletin","body":"this is a new threat bulletin"}""");

	[Fact]
	public Task CreateAsync_SendsPrivateTags()
		=> CreateShouldSendAsync(
			new ThreatBulletinCreateRequest
			{
				Name = "New Threat Bulletin with Associations",
				TagsV2 = [new ThreatStreamTag { Name = "my_private_tag", Tlp = Tlp.Red }]
			},
			"""{"name":"New Threat Bulletin with Associations","tags_v2":[{"name":"my_private_tag","tlp":"red"}]}""");

	[Fact]
	public Task GetAsync_SendsGet()
		=> GetShouldSendAsync((client, id, options, ct) => client.ThreatBulletins.GetAsync(id, options, ct), Type, ThreatBulletinJson);

	[Fact]
	public Task UpdateAsync_SendsTheDocumentedExample()
		=> UpdateShouldSendAsync(new ThreatBulletinUpdateRequest { Name = "Updated Threat Bulletin Name" }, """{"name":"Updated Threat Bulletin Name"}""");

	[Fact]
	public Task ListAssociatedWithValueAsync_SendsTheValue()
		=> AssociatedWithValueShouldSendAsync((client, value, options, ct) => client.ThreatBulletins.ListAssociatedWithValueAsync(value, options, ct), Type);

	[Fact]
	public Task ListAssociatedWithIdsAsync_SendsTheIds()
		=> AssociatedWithIdsShouldSendAsync((client, ids, options, ct) => client.ThreatBulletins.ListAssociatedWithIdsAsync(ids, options, ct), Type);

	[Fact]
	public Task ListLinkedAsync_SendsTheEntityType()
		=> LinkedShouldSendAsync((client, id, entityType, options, ct) => client.ThreatBulletins.ListLinkedAsync(id, entityType, options, ct), Type);

	[Fact]
	public Task ListIntelligenceAsync_SendsGet()
		=> IntelligenceShouldSendAsync((client, id, options, ct) => client.ThreatBulletins.ListIntelligenceAsync(id, options, ct), Type);

	[Fact]
	public Task GetAsync_NotFound_RaisesThreatStreamApiException()
		=> ShouldFailWhenNotFoundAsync((client, ct) => client.ThreatBulletins.GetAsync(11, null, ct));

	[Fact]
	public void Property_ReturnsTheSameInstance()
	{
		using var client = TestClient.Create(new StubHandler());

		client.ThreatBulletins.Should().BeSameAs(client.ThreatBulletins);
	}
}
