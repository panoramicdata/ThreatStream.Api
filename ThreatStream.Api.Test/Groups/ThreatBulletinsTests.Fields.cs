using System.Text;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.ThreatModelsBTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class ThreatBulletinsTests
{
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
		bulletin.OwnerUserId.Should().Be(8);
		bulletin.IsAnonymous.Should().BeTrue();
		bulletin.Source.Should().Be("USCERT");
		var tag = bulletin.TagsV2.Should().ContainSingle().Subject;
		(tag.Id, tag.Name, tag.Tlp, tag.OrgId).Should().Be(("abc", "my_private_tag", Tlp.Red, 42L));
		bulletin.RemoteApi.Should().BeFalse();
		bulletin.Attachments.Should().ContainSingle().Which.GetProperty("filename").GetString().Should().Be("report.pdf");
		bulletin.ImportSessions.Should().ContainSingle().Which.GetInt64().Should().Be(301);
		bulletin.Intelligence.Should().ContainSingle().Which.GetInt64().Should().Be(165546);
		bulletin.SandboxReports.Should().ContainSingle().Which.GetInt64().Should().Be(77);
		bulletin.AdditionalProperties.Keys.Should().Contain(["comments", "history", "threat_actor", "ttp", "vulnerability"]);
	}

	[Fact]
	public async Task GetAsync_UnknownStatus_ReadsAsUnknown()
	{
		var bulletin = await TestClient.ReadAsync(
			(client, ct) => client.ThreatBulletins.GetAsync(11, null, ct),
			"""{"id": 11, "status": "archived", "body_content_type": "html", "tags_v2": null}""");

		bulletin.Status.Should().Be(ThreatModelPublicationStatus.Unknown);
		bulletin.BodyContentType.Should().Be(ThreatModelBodyContentType.Unknown);
		bulletin.TagsV2.Should().BeEmpty();
	}

	[Fact]
	public async Task UpdateAsync_SendsEveryField()
	{
		var request = new ThreatBulletinUpdateRequest
		{
			Name = "n",
			Body = "b",
			BodyContentType = ThreatModelBodyContentType.Markdown,
			Status = ThreatModelPublicationStatus.Published,
			AssigneeOrgId = 42,
			AssigneeUserId = 7,
			IsAnonymous = false,
			Source = "USCERT",
			TagsV2 = [new ThreatStreamTag { Name = "t" }],
			ImportSessions = [301],
			SandboxReports = [77],
			Intelligence = [165546]
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatBulletins.UpdateAsync(11, request, ct), ThreatBulletinJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/tipreport/11/", body: "{\"name\":\"n\",\"body\":\"b\",\"body_content_type\":\"markdown\",\"status\":\"published\","
			+ "\"assignee_org_id\":42,\"assignee_user_id\":7,\"is_anonymous\":false,\"source\":\"USCERT\",\"tags_v2\":[{\"name\":\"t\"}],"
			+ "\"import_sessions\":[301],\"sandbox_reports\":[77],\"intelligence\":[165546]}");
	}

	[Fact]
	public async Task AddAttachmentAsync_SendsTheFileAndItsName()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatBulletins.AddAttachmentAsync(11, Attachment, ct), """{"id": 5, "filename": "report.pdf", "s3_url": "https://s3.example/report.pdf"}""");

		call.ShouldBe(HttpMethod.Post, "/api/v1/tipreport/11/attachment/");
		call.ContentType.Should().Be("multipart/form-data");
		call.Parts.Select(p => (p.Name, p.FileName, p.ContentType, p.Text)).Should().Equal(
			("attachment", "report.pdf", "application/pdf", "%PDF-1.7"),
			("filename", null, null, "report.pdf"));
	}

	[Fact]
	public async Task AddAttachmentAsync_MapsTheResponse()
	{
		var attachment = await TestClient.ReadAsync((client, ct) => client.ThreatBulletins.AddAttachmentAsync(11, Attachment, ct), """{"id": 5, "filename": "report.pdf", "s3_url": "https://s3.example/report.pdf"}""");

		attachment.Id.Should().Be(5);
		attachment.FileName.Should().Be("report.pdf");
		attachment.AdditionalProperties["s3_url"].GetString().Should().Be("https://s3.example/report.pdf");
	}

	[Fact]
	public async Task Writes_AreRefusedWhenReadOnly()
	{
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatBulletins.CreateAsync(new ThreatBulletinCreateRequest { Name = "x" }, ct));
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatBulletins.UpdateAsync(11, new ThreatBulletinUpdateRequest { Name = "x" }, ct));
		await ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.ThreatBulletins.AddAttachmentAsync(11, Attachment, ct));
	}
}
