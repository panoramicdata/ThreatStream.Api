using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;

namespace ThreatStream.Api.Test.Support;

/// <summary>Shared responses and assertions for the intelligence, import, snapshot and sandbox tests.</summary>
internal static class IntelligenceTestKit
{
	/// <summary>
	/// An observable as <c>api/v2/intelligence/</c> returns it, built from the reference's attribute table and examples
	/// (with an organisation and user anonymised), including quoted numbers and an unmodelled attribute.
	/// </summary>
	public const string ObservableJson = """
		{
			"asn": "3786",
			"can_add_public_tags": true,
			"classification": "private",
			"confidence": "90",
			"country": "IN",
			"created_by": "analyst@example.com",
			"created_ts": "2015-04-13T23:00:00.123Z",
			"expiration_ts": "2015-07-12T23:00:00",
			"feed_id": 0,
			"id": "11202475",
			"import_session_id": 346928,
			"ip": "203.0.113.2",
			"is_anonymous": false,
			"is_editable": false,
			"is_public": "1",
			"itype": "apt_ip",
			"latitude": 28.6667,
			"longitude": 77.2167,
			"meta": {"detail": "energetic-bear", "detail2": "imported by user 668", "maltype": "zeus", "severity": "very-high", "registrant_name": "x"},
			"modified_ts": "2015-04-14T01:02:03",
			"org": "Example Telecom",
			"owner_organization_id": 2,
			"rdns": "host.example.net",
			"resource_uri": "/api/v2/intelligence/11202475/",
			"retina_confidence": -1,
			"severity": "very-high",
			"source": "Analyst",
			"source_created": "2015-04-12T00:00:00",
			"source_modified": "2015-04-13T00:00:00",
			"source_reported_confidence": 90,
			"srcip": "203.0.113.2",
			"status": "active",
			"subtype": null,
			"tags": [{"id": "abc1", "name": "energetic-bear", "tlp": "red", "org_id": 2}],
			"threat_type": "apt",
			"threatscore": 90,
			"tlp": "amber",
			"trusted_circle_ids": [13, 20],
			"type": "ip",
			"update_id": "2237854",
			"uuid": "0a5b2a6e-8f6d-4f0c-9d3e-0123456789ab",
			"value": "203.0.113.2"
		}
		""";

	/// <summary>A page holding <see cref="ObservableJson"/>.</summary>
	public static readonly string ObservablePage = $$"""{"meta":{"limit":20,"offset":0,"next":null,"total_count":1},"objects":[{{ObservableJson}}]}""";

	/// <summary>The import job a clone or an import with approval answers (from the reference's examples).</summary>
	public const string ImportJobJson = """{"job_id": "6194c7c0-53ae-41fc-a6f0-dc3d174f02e2", "success": true, "import_session_id": "482"}""";

	/// <summary>Asserts every modelled field of <see cref="ObservableJson"/>.</summary>
	public static void ShouldBeTheExampleObservable(Observable observable)
	{
		observable.Id.Should().Be(11202475);
		observable.ResourceUri.Should().Be("/api/v2/intelligence/11202475/");
		observable.Value.Should().Be("203.0.113.2");
		observable.Type.Should().Be("ip");
		observable.Itype.Should().Be("apt_ip");
		observable.ThreatType.Should().Be("apt");
		observable.Status.Should().Be("active");
		observable.Classification.Should().Be("private");
		observable.Confidence.Should().Be(90);
		observable.RetinaConfidence.Should().Be(-1);
		observable.SourceReportedConfidence.Should().Be(90);
		observable.Severity.Should().Be("very-high");
		observable.Tlp.Should().Be(Tlp.Amber);
		observable.Meta!.Detail.Should().Be("energetic-bear");
		observable.Meta.Detail2.Should().Be("imported by user 668");
		observable.Meta.Maltype.Should().Be("zeus");
		observable.Meta.Severity.Should().Be("very-high");
		observable.Meta.AdditionalProperties.Should().ContainKey("registrant_name");
		var tag = observable.Tags.Should().ContainSingle().Subject;
		(tag.Id, tag.Name, tag.Tlp, tag.OrgId).Should().Be(("abc1", "energetic-bear", Tlp.Red, 2L));
		observable.Subtype.Should().BeNull();
		observable.Asn.Should().Be("3786");
		observable.Country.Should().Be("IN");
		observable.Ip.Should().Be("203.0.113.2");
		observable.SourceIp.Should().Be("203.0.113.2");
		observable.Org.Should().Be("Example Telecom");
		observable.Rdns.Should().Be("host.example.net");
		observable.Latitude.Should().Be(28.6667);
		observable.Longitude.Should().Be(77.2167);
		observable.CreatedTs.Should().Be(new DateTimeOffset(2015, 4, 13, 23, 0, 0, 123, TimeSpan.Zero));
		observable.ModifiedTs.Should().Be(new DateTimeOffset(2015, 4, 14, 1, 2, 3, TimeSpan.Zero));
		observable.ExpirationTs.Should().Be(new DateTimeOffset(2015, 7, 12, 23, 0, 0, TimeSpan.Zero));
		observable.SourceCreated.Should().Be(new DateTimeOffset(2015, 4, 12, 0, 0, 0, TimeSpan.Zero));
		observable.SourceModified.Should().Be(new DateTimeOffset(2015, 4, 13, 0, 0, 0, TimeSpan.Zero));
		observable.Source.Should().Be("Analyst");
		observable.CreatedBy.Should().Be("analyst@example.com");
		observable.FeedId.Should().Be(0);
		observable.ImportSessionId.Should().Be(346928);
		observable.OwnerOrganizationId.Should().Be(2);
		observable.TrustedCircleIds.Should().Equal(13, 20);
		observable.IsPublic.Should().BeTrue();
		observable.IsAnonymous.Should().BeFalse();
		observable.IsEditable.Should().BeFalse();
		observable.CanAddPublicTags.Should().BeTrue();
		observable.UpdateId.Should().Be(2237854);
		observable.Uuid.Should().Be("0a5b2a6e-8f6d-4f0c-9d3e-0123456789ab");
		observable.AdditionalProperties["threatscore"].GetInt32().Should().Be(90);
	}

	/// <summary>The parts of a recorded multipart body as (name, file name, content type, text) tuples.</summary>
	public static IEnumerable<(string? Name, string? FileName, string? ContentType, string Text)> PartsOf(RecordedCall call)
	{
		call.ContentType.Should().Be("multipart/form-data");
		return call.Parts.Select(p => (p.Name, p.FileName, p.ContentType, p.Text));
	}

	/// <summary>A form field part, as the multipart encoder sends one.</summary>
	public static (string? Name, string? FileName, string? ContentType, string Text) Field(string name, string text) => (name, null, null, text);
}
