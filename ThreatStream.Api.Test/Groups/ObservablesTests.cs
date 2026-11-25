using System.Net;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Test.Support;
using static ThreatStream.Api.Test.Support.IntelligenceTestKit;

namespace ThreatStream.Api.Test.Groups;

public partial class ObservablesTests
{
	[Fact]
	public async Task ListAsync_SendsGetWithoutFilters()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Observables.ListAsync(null, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v2/intelligence/");
	}

	[Fact]
	public async Task ListAsync_SendsEveryFilter()
	{
		var since = new DateTimeOffset(2014, 10, 2, 20, 44, 35, TimeSpan.Zero);
		var options = new ObservableListOptions
		{
			Query = "confidence>=90",
			SearchFilter = 398,
			Value = "1.2.3.4",
			ValueContains = "example",
			ValueStartsWith = "http",
			Type = "ip",
			Itype = "mal_ip,c2_ip",
			ThreatType = "malware",
			Status = "active",
			Subtype = "SHA256",
			Tlp = Tlp.Red,
			IsPublic = true,
			TagName = "energetic-bear",
			Severity = "low",
			Maltype = "zeus",
			Detail = "d",
			Detail2 = "d2",
			FeedId = 0,
			ImportSessionId = 482,
			TrustedCircleIds = [1, 2],
			TargetIndustry = "financial-services",
			TargetLocations = "europe",
			SourceLocations = "US-FL",
			Uuid = "u",
			FullCount = 1,
			LocalOnly = true,
			RemoteApi = true,
			MinConfidence = 80,
			MaxConfidence = 99,
			MinSourceReportedConfidence = 50,
			MinRetinaConfidence = 40,
			CreatedSince = since,
			CreatedBefore = since,
			ModifiedSince = since,
			ModifiedBefore = since,
			UpdateIdAfter = 11849,
			SourceCreatedSince = since,
			SourceModifiedSince = since,
			Country = "CN",
			Asn = "3786",
			Ip = "1.2.3.4",
			Org = "o",
			Rdns = "r",
			Limit = 0,
			Offset = 20,
			OrderBy = "update_id"
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Observables.ListAsync(options, ct), EmptyPage);

		call.ShouldBe(HttpMethod.Get, "/api/v2/intelligence/", AllFiltersQuery);
	}

	[Fact]
	public async Task ListAsync_MapsEveryField()
	{
		var page = await TestClient.ReadAsync((client, ct) => client.Observables.ListAsync(null, ct), ObservablePage);

		page.Meta!.TotalCount.Should().Be(1);
		ShouldBeTheExampleObservable(page.Objects.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task ListAsync_MissingValues_ReadAsDefaults()
	{
		var page = await TestClient.ReadAsync(
			(client, ct) => client.Observables.ListAsync(null, ct),
			"""{"meta":{},"objects":[{"id":1,"tags":null,"trusted_circle_ids":null,"meta":null,"tlp":null,"confidence":""}]}""");

		var observable = page.Objects.Should().ContainSingle().Subject;
		observable.Tags.Should().BeEmpty();
		observable.TrustedCircleIds.Should().BeEmpty();
		observable.Meta.Should().BeNull();
		observable.Tlp.Should().BeNull();
		observable.Confidence.Should().BeNull();
		observable.UpdateId.Should().Be(0);
	}

	[Fact]
	public Task ListAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Observables.ListAsync(null, ct),
			HttpStatusCode.TooManyRequests,
			"""{"message": "Rate limit exceeded"}""",
			"Rate limit exceeded");

	[Fact]
	public async Task ListAssociatedWithIntelligenceAsync_SendsTheValueAndIds()
	{
		var call = await TestClient.CaptureAsync(
			(client, ct) => client.Observables.ListAssociatedWithIntelligenceAsync(new ObservableAssociationOptions { Value = "google.com", Ids = [1, 2], Limit = 10 }, ct),
			ObservablePage);

		call.ShouldBe(HttpMethod.Get, "/api/v2/intelligence/associated_with_intelligence/", "?value=google.com&ids=1%2C2&limit=10");
	}

	[Fact]
	public async Task ListAssociatedWithIntelligenceAsync_MapsTheObservables()
	{
		var page = await TestClient.ReadAsync(
			(client, ct) => client.Observables.ListAssociatedWithIntelligenceAsync(new ObservableAssociationOptions { Value = "google.com" }, ct),
			ObservablePage);

		ShouldBeTheExampleObservable(page.Objects.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task CloneAsync_SendsPostToTheObservable()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Observables.CloneAsync(11202475, ct), ImportJobJson);

		call.ShouldBe(HttpMethod.Post, "/api/v2/intelligence/11202475/clone/");
	}

	[Fact]
	public async Task CloneAsync_MapsTheImportJob()
	{
		var job = await TestClient.ReadAsync(
			(client, ct) => client.Observables.CloneAsync(11202475, ct),
			"""{"job_id": "368ab6dc-7955-4fd7-aef9-b8cd5496fcb1", "import_session_id": "346928"}""");

		job.JobId.Should().Be("368ab6dc-7955-4fd7-aef9-b8cd5496fcb1");
		job.ImportSessionId.Should().Be(346928);
		job.Success.Should().BeNull();
	}

	[Fact]
	public Task CloneAsync_ReadOnlyClient_IsRefused()
		=> ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Observables.CloneAsync(1, ct), "POST", "api/v2/intelligence/1/clone/");
}
