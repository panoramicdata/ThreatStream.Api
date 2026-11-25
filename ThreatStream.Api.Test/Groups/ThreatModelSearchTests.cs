using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModelSearch;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public class ThreatModelSearchTests
{
	// Two results of different types, built from the reference's attribute table (it gives no example response).
	private const string PageJson = """
		{
			"meta": {"limit": 20, "offset": 0, "next": null, "total_count": 2, "approximate_count": true},
			"objects": [
				{
					"id": "775054",
					"resource_uri": "/api/v1/actor/775054/",
					"model_type": "actor",
					"name": "Fancy Bear",
					"aliases": "APT28",
					"publication_status": "published",
					"tlp": "amber",
					"is_public": true,
					"tags": [{"id": "t1", "name": "russia"}],
					"feed_id": 72,
					"organization_id": "2",
					"owner_user_id": 7,
					"organization": {"id": 2, "name": "Example Org"},
					"created_by": "analyst@example.com",
					"created_ts": "2014-10-02T20:44:35",
					"modified_ts": "2026-01-02T03:04:05",
					"published_ts": "2026-01-02T03:05:00",
					"source_created": "2014-10-01T00:00:00",
					"source_modified": "2025-12-31T00:00:00",
					"uuid": "1d6e1a4e-0000-4000-8000-000000000001"
				},
				{
					"id": 12,
					"resource_uri": "/api/v1/signature/12/",
					"model_type": "signature",
					"name": "Snort rule",
					"signature_type": "Snort",
					"tags": null
				}
			]
		}
		""";

	[Fact]
	public async Task SearchAsync_SendsGetWithEveryFilter()
	{
		var moment = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero);
		var options = new ThreatModelSearchOptions
		{
			Query = "name=\"x\"",
			SearchFilter = 7,
			ModelType = [ThreatModelType.Signature, ThreatModelType.Actor],
			Value = "bear",
			Name = "Fancy Bear",
			NameContains = "Bear",
			Alias = "APT28",
			TagName = "malware,community-threat-briefing",
			Tlp = Tlp.Red,
			IsPublic = false,
			PublicationStatus = ThreatModelPublicationStatus.New,
			SignatureType = "CybOX",
			FeedId = 72,
			OrganizationId = 2,
			OwnerUserId = 7,
			TrustedCircleIds = [20],
			IsEmail = true,
			CreatedSince = moment,
			CreatedBefore = moment,
			ModifiedSince = moment,
			ModifiedBefore = moment,
			PublishedSince = moment,
			StartedSince = moment,
			EndedBefore = moment,
			SourceCreatedSince = moment,
			SourceModifiedSince = moment,
			TargetIndustry = "chemical",
			TargetLocations = "europe",
			SourceLocations = "NL",
			FullCount = 1,
			LocalOnly = true,
			RemoteApi = true,
			Limit = 100
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatModelSearch.SearchAsync(options, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/threat_model_search/", AllFiltersQuery);
	}

	[Fact]
	public async Task SearchAsync_WithoutFilters_SendsNoQuery()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.ThreatModelSearch.SearchAsync(null, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v1/threat_model_search/");
	}

	[Fact]
	public async Task SearchAsync_MapsMixedTypes()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.ThreatModelSearch.SearchAsync(null, ct), PageJson);

		page.Meta!.ApproximateCount.Should().BeTrue();
		page.Objects.Should().HaveCount(2);
		var actor = page.Objects[0];
		actor.Id.Should().Be(775054);
		actor.ResourceUri.Should().Be("/api/v1/actor/775054/");
		actor.ModelType.Should().Be(ThreatModelType.Actor);
		actor.Name.Should().Be("Fancy Bear");
		actor.PublicationStatus.Should().Be(ThreatModelPublicationStatus.Published);
		actor.Tlp.Should().Be(Tlp.Amber);
		actor.IsPublic.Should().BeTrue();
		actor.Tags.Should().ContainSingle().Which.Name.Should().Be("russia");
		actor.FeedId.Should().Be(72);
		actor.OrganizationId.Should().Be(2);
		actor.OwnerUserId.Should().Be(7);
		actor.CreatedBy.Should().Be("analyst@example.com");
		actor.CreatedTs.Should().Be(new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero));
		actor.ModifiedTs.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
		actor.PublishedTs.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 5, 0, TimeSpan.Zero));
		actor.SourceCreated.Should().Be(new DateTimeOffset(2014, 10, 1, 0, 0, 0, TimeSpan.Zero));
		actor.SourceModified.Should().Be(new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero));
		actor.Uuid.Should().Be("1d6e1a4e-0000-4000-8000-000000000001");
		actor.AdditionalProperties.Keys.Should().BeEquivalentTo("aliases", "organization");
		var signature = page.Objects[1];
		signature.ModelType.Should().Be(ThreatModelType.Signature);
		signature.Tags.Should().BeEmpty();
		signature.AdditionalProperties["signature_type"].GetString().Should().Be("Snort");
	}

	[Fact]
	public Task SearchAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.ThreatModelSearch.SearchAsync(null, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "model_type is required with signature$type"}""",
			"model_type is required with signature$type");

	private const string Moment = "2014-10-02T20%3A44%3A35Z";

	private const string AllFiltersQuery = "?q=name%3D%22x%22&search_filter=7&model_type=signature%2Cactor&value=bear&name=Fancy%20Bear"
		+ "&name__contains=Bear&alias=APT28&tags.name=malware%2Ccommunity-threat-briefing&tlp=red&is_public=false&publication_status=new"
		+ "&signature%24type=CybOX&feed_id=72&organization_id=2&owner_user_id=7&trusted_circle_ids=20&is_email=true"
		+ "&created_ts__gte=" + Moment + "&created_ts__lt=" + Moment + "&modified_ts__gte=" + Moment + "&modified_ts__lt=" + Moment
		+ "&published_ts__gte=" + Moment + "&start_date__gte=" + Moment + "&end_date__lt=" + Moment
		+ "&source_created__gte=" + Moment + "&source_modified__gte=" + Moment
		+ "&target_industry=chemical&target_locations=europe&source_locations=NL&full_count=1&local_only=true&remote_api=true&limit=100";
}
