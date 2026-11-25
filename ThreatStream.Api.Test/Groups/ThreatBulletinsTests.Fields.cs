using System.Text;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class ThreatBulletinsTests
{
	private const string AttachmentJson = """{"id": 5, "filename": "report.pdf", "s3_url": "https://s3.example/report.pdf"}""";

	private static ThreatBulletinAttachmentRequest Attachment => new()
	{
		Attachment = new ThreatStreamFile("report.pdf", Encoding.UTF8.GetBytes("%PDF-1.7"), "application/pdf"),
		FileName = "report.pdf"
	};

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var bulletin = await TestClient.ReadAsync((client, ct) => client.ThreatBulletins.GetAsync(11, null, ct), ThreatBulletinJson);

		bulletin.ShouldHaveEntityFields(11, Type);
		bulletin.Body.Should().Be("this is a new threat bulletin");
		bulletin.BodyContentType.Should().Be(ThreatModelBodyContentType.RichText);
		bulletin.Status.Should().Be(ThreatModelPublicationStatus.ReviewRequested);
		bulletin.AssigneeOrgId.Should().Be(42);
		bulletin.AssigneeUserId.Should().Be(7);
		bulletin.OwnerOrgId.Should().Be(42);
		bulletin.Source.Should().Be("USCERT");
		bulletin.Attachments.Should().ContainSingle().Which.Name.Should().Be("report.pdf");
		bulletin.ImportSessions.Should().ContainSingle().Which.Id.Should().Be(301);
		bulletin.SandboxReports.Should().ContainSingle().Which.Id.Should().Be(77);
		bulletin.AdditionalProperties.Keys.Should().Contain(["comments", "history", "threat_actor", "ttp", "vulnerability"]);
	}

	[Fact]
	public async Task GetAsync_UnknownStatus_ReadsAsUnknown()
	{
		var bulletin = await TestClient.ReadAsync(
			(client, ct) => client.ThreatBulletins.GetAsync(11, null, ct),
			"""{"id": 11, "status": "archived", "body_content_type": "html", "attachments": null}""");

		bulletin.Status.Should().Be(ThreatModelPublicationStatus.Unknown);
		bulletin.BodyContentType.Should().Be(ThreatModelBodyContentType.Unknown);
		bulletin.Attachments.Should().BeEmpty();
	}

	[Fact]
	public Task UpdateAsync_SendsEveryField()
		=> UpdateShouldSendAsync(
			new ThreatBulletinUpdateRequest
			{
				Body = "b",
				BodyContentType = ThreatModelBodyContentType.Markdown,
				Status = ThreatModelPublicationStatus.Published,
				AssigneeOrgId = 42,
				AssigneeUserId = 7,
				IsAnonymous = false,
				Source = "USCERT",
				ImportSessions = [301],
				SandboxReports = [77]
			},
			"{\"body\":\"b\",\"body_content_type\":\"markdown\",\"status\":\"published\","
				+ "\"assignee_org_id\":42,\"assignee_user_id\":7,\"is_anonymous\":false,\"source\":\"USCERT\","
				+ "\"import_sessions\":[301],\"sandbox_reports\":[77]}");

	[Fact]
	public async Task AddAttachmentAsync_SendsTheFileAndItsName()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatBulletins.AddAttachmentAsync(11, Attachment, ct), AttachmentJson);

		call.ShouldBe(HttpMethod.Post, "/api/v1/tipreport/11/attachment/");
		IntelligenceTestKit.PartsOf(call).Should().Equal(
			("attachment", "report.pdf", "application/pdf", "%PDF-1.7"),
			("filename", null, null, "report.pdf"));
	}

	[Fact]
	public async Task AddAttachmentAsync_MapsTheResponse()
	{
		var attachment = (await TestClient.ReadAsync((client, ct) => client.ThreatBulletins.AddAttachmentAsync(11, Attachment, ct), AttachmentJson))!;

		attachment.Id.Should().Be(5);
		attachment.FileName.Should().Be("report.pdf");
		attachment.AdditionalProperties["s3_url"].GetString().Should().Be("https://s3.example/report.pdf");
	}

	[Fact]
	public async Task Writes_AreRefusedWhenReadOnly()
	{
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatBulletins.CreateAsync(new ThreatBulletinCreateRequest { Name = "x" }, ct), "POST", "api/v1/tipreport/");
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatBulletins.UpdateAsync(11, new ThreatBulletinUpdateRequest { Name = "x" }, ct), "PATCH", "api/v1/tipreport/11/");
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatBulletins.AddAttachmentAsync(11, Attachment, ct), "POST", "api/v1/tipreport/11/attachment/");
	}
}
