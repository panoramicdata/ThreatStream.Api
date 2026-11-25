using System.Net;
using ThreatStream.Api.Models.Intelligence;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public partial class ObservablesTests
{
	private const string Moment = "2014-10-02T20%3A44%3A35Z";

	private const string AllFiltersQuery = "?q=confidence%3E%3D90&search_filter=398&value=1.2.3.4&value__contains=example&value__startswith=http"
		+ "&type=ip&itype=mal_ip%2Cc2_ip&threat_type=malware&status=active&subtype=SHA256&tlp=red&is_public=true"
		+ "&tags.name=energetic-bear&meta.severity=low&meta.maltype=zeus&meta.detail=d&meta.detail2=d2&feed_id=0&import_session_id=482"
		+ "&trusted_circle_ids=1%2C2&target_industry=financial-services&target_locations=europe&source_locations=US-FL&uuid=u"
		+ "&full_count=1&local_only=true&remote_api=true&confidence__gte=80&confidence__lte=99&source_reported_confidence__gte=50"
		+ "&retina_confidence__gte=40&created_ts__gte=" + Moment + "&created_ts__lt=" + Moment + "&modified_ts__gte=" + Moment
		+ "&modified_ts__lt=" + Moment + "&update_id__gt=11849&source_created__gte=" + Moment + "&source_modified__gte=" + Moment
		+ "&country=CN&asn=3786&ip=1.2.3.4&org=o&rdns=r&limit=0&offset=20&order_by=update_id";

	private static string Page(string next, params long[] updateIds)
		=> $$"""{"meta":{"next":{{next}}},"objects":[{{string.Join(',', updateIds.Select(u => $$"""{"id":{{u}},"update_id":{{u}}}"""))}}]}""";

	private static async Task<(List<Observable> Read, List<RecordedCall> Calls)> EnumerateAsync(
		Func<ThreatStreamClient, CancellationToken, IAsyncEnumerable<Observable>> enumerate,
		params string[] pages)
	{
		var stub = new StubHandler();
		foreach (var page in pages)
		{
			stub.Enqueue(HttpStatusCode.OK, page);
		}

		using var client = TestClient.Create(stub);
		var read = new List<Observable>();
		await foreach (var observable in enumerate(client, TestContext.Current.CancellationToken))
		{
			read.Add(observable);
		}

		return (read, stub.Calls);
	}

	[Fact]
	public async Task EnumerateObservablesAsync_FollowsTheNextLinks()
	{
		var options = new ObservableListOptions { Status = "active", Limit = 2 };

		var (read, calls) = await EnumerateAsync(
			(client, ct) => client.EnumerateObservablesAsync(options, ct),
			Page("\"/api/v2/intelligence/?limit=2&offset=2&status=active\"", 1, 2),
			Page("null", 3));

		read.Select(o => o.Id).Should().Equal(1, 2, 3);
		calls.Select(c => c.Uri.PathAndQuery).Should().Equal(
			"/api/v2/intelligence/?status=active&limit=2",
			"/api/v2/intelligence/?limit=2&offset=2&status=active");
	}

	[Fact]
	public async Task EnumerateObservablesByUpdateIdAsync_WithoutOptions_PagesByUpdateId()
	{
		var (read, calls) = await EnumerateAsync(
			(client, ct) => client.EnumerateObservablesByUpdateIdAsync(null, 0, ct),
			Page("null", 5, 7),
			Page("null", 9),
			Page("null"));

		read.Select(o => o.UpdateId).Should().Equal(5, 7, 9);
		calls.Select(c => c.Uri.Query).Should().Equal(
			"?update_id__gt=0&order_by=update_id",
			"?update_id__gt=7&order_by=update_id",
			"?update_id__gt=9&order_by=update_id");
	}

	[Fact]
	public async Task EnumerateObservablesByUpdateIdAsync_KeepsTheFilters_AndLeavesTheCallersOptionsAlone()
	{
		var options = new ObservableListOptions { Status = "active", Limit = 1000, OrderBy = "-modified_ts" };

		var (read, calls) = await EnumerateAsync(
			(client, ct) => client.EnumerateObservablesByUpdateIdAsync(options, 11849, ct),
			Page("null", 11850),
			Page("null"));

		read.Should().ContainSingle();
		calls.Select(c => c.Uri.Query).Should().Equal(
			"?status=active&update_id__gt=11849&limit=1000&order_by=update_id",
			"?status=active&update_id__gt=11850&limit=1000&order_by=update_id");
		options.UpdateIdAfter.Should().BeNull();
		options.OrderBy.Should().Be("-modified_ts");
	}

	[Fact]
	public async Task EnumerateObservablesByUpdateIdAsync_WithAnAdvancedSearch_PutsTheBoundInTheQuery()
	{
		var options = new ObservableListOptions { Query = "confidence>=90", UpdateIdAfter = 3 };

		var (_, calls) = await EnumerateAsync(
			(client, ct) => client.EnumerateObservablesByUpdateIdAsync(options, 0, ct),
			Page("null"));

		Uri.UnescapeDataString(calls.Should().ContainSingle().Subject.Uri.Query).Should().Be("?q=(confidence>=90) AND update_id>0&order_by=update_id");
		options.Query.Should().Be("confidence>=90");
	}
}
