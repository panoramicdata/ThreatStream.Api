using Refit;
using System.Net;
using System.Text.Json.Serialization;
using ThreatStream.Api.Models;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

/// <summary>Pins how Refit and the ThreatStream serializer encode paths, query strings and request bodies.</summary>
public class RequestEncodingTests
{
	public interface IProbeApi
	{
		[Get("api/v1/probe/")]
		Task<ThreatStreamPage<ProbeObject>> ListAsync([Query] ProbeListOptions? options, CancellationToken cancellationToken);

		[Get("api/v1/probe/{id}/")]
		Task<ProbeObject> GetAsync(string id, CancellationToken cancellationToken);

		[Post("api/v1/probe/")]
		Task CreateAsync([Body] ProbeCreateRequest request, CancellationToken cancellationToken);

		[Post("api/v1/probe/upload/")]
		Task UploadAsync([Body] ProbeUploadRequest request, CancellationToken cancellationToken);

		[Get("api/v1/probe/export/")]
		Task<string> ExportAsync(CancellationToken cancellationToken);
	}

	public sealed class ProbeListOptions : ListOptions
	{
		[AliasAs("id__in")]
		public IReadOnlyList<long>? Ids { get; set; }

		[AliasAs("is_public")]
		public bool? IsPublic { get; set; }

		[AliasAs("modified_ts__gte")]
		public DateTimeOffset? ModifiedSince { get; set; }

		[AliasAs("q")]
		public string? Query { get; set; }
	}

	public sealed class ProbeCreateRequest
	{
		[JsonPropertyName("name")]
		public required string Name { get; init; }

		[JsonPropertyName("tags")]
		public IReadOnlyList<ProbeTag>? Tags { get; init; }

		[JsonPropertyName("is_public")]
		public bool? IsPublic { get; init; }
	}

	public sealed record ProbeTag([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("tlp")] string Tlp);

	public sealed class ProbeUploadRequest : ThreatStreamMultipartRequest
	{
		[JsonPropertyName("file")]
		public ThreatStreamFile? File { get; init; }

		public string? Classification { get; init; }

		[JsonPropertyName("confidence")]
		public int? Confidence { get; init; }

		[JsonPropertyName("remove_existing")]
		public bool? RemoveExisting { get; init; }

		[JsonPropertyName("trustedcircles")]
		public IReadOnlyList<long>? TrustedCircles { get; init; }

		[JsonPropertyName("expiration_ts")]
		public DateTimeOffset? ExpirationTs { get; init; }

		[JsonPropertyName("tags")]
		public IReadOnlyList<ProbeTag>? Tags { get; init; }

		[JsonIgnore]
		public string? NotSent { get; init; }
	}

	private static (IProbeApi Probe, StubHandler Stub, ThreatStreamClient Client) Create(string response = """{"meta":{},"objects":[]}""")
	{
		var stub = TestClient.Stub(response);
		var client = TestClient.Create(stub);
		return (client.For<IProbeApi>(), stub, client);
	}

	private static CancellationToken Ct => TestContext.Current.CancellationToken;

	[Fact]
	public async Task ListOptions_AreFlattenedIntoTheQuery_WithCommaSeparatedLists()
	{
		var (probe, stub, client) = Create();
		using (client)
		{
			await probe.ListAsync(
				new ProbeListOptions
				{
					Limit = 0,
					Offset = 40,
					OrderBy = "-created_ts",
					Ids = [1, 2, 3],
					IsPublic = false,
					ModifiedSince = new DateTimeOffset(2026, 1, 2, 4, 4, 5, TimeSpan.FromHours(1)),
					Query = "confidence>=80 AND itype=c2_domain"
				},
				Ct);
		}

		Uri.UnescapeDataString(stub.Calls[0].Uri.Query).Should().Be(
			"?id__in=1,2,3&is_public=false&modified_ts__gte=2026-01-02T03:04:05Z&q=confidence>=80 AND itype=c2_domain&limit=0&offset=40&order_by=-created_ts");
	}

	[Fact]
	public async Task NullListOptions_SendNoQuery()
	{
		var (probe, stub, client) = Create();
		using (client)
		{
			await probe.ListAsync(null, Ct);
		}

		stub.Calls[0].ShouldBe(HttpMethod.Get, "/api/v1/probe/");
	}

	[Fact]
	public async Task PathValues_AreOneEscapedSegment_AndTheTrailingSlashIsKept()
	{
		var (probe, stub, client) = Create("{}");
		using (client)
		{
			await probe.GetAsync("a/b c", Ct);
		}

		stub.Calls[0].Uri.AbsolutePath.Should().Be("/api/v1/probe/a%2Fb%20c/");
	}

	[Fact]
	public async Task Body_IsSentAsJson()
	{
		var (probe, stub, client) = Create("{}");
		using (client)
		{
			await probe.CreateAsync(new ProbeCreateRequest { Name = "threatstream_api_it_x", Tags = [new("t", "red")] }, Ct);
		}

		stub.Calls[0].ShouldBe(HttpMethod.Post, "/api/v1/probe/", body: """{"name":"threatstream_api_it_x","tags":[{"name":"t","tlp":"red"}]}""");
	}

	[Fact]
	public async Task MultipartRequest_IsSentPartByPart()
	{
		var (probe, stub, client) = Create("{}");
		using (client)
		{
			await probe.UploadAsync(
				new ProbeUploadRequest
				{
					File = new ThreatStreamFile("iocs.csv", "value,itype\n1.2.3.4,c2_ip\n"u8.ToArray(), "text/csv"),
					Classification = "private",
					Confidence = 80,
					RemoveExisting = false,
					TrustedCircles = [10, 20],
					ExpirationTs = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
					Tags = [new("t", "red")],
					NotSent = "x",
					AdditionalFields = { ["notes"] = "from a test" }
				},
				Ct);
		}

		IntelligenceTestKit.PartsOf(stub.Calls[0]).Should().Equal(
			("file", "iocs.csv", "text/csv", "value,itype\n1.2.3.4,c2_ip\n"),
			("classification", null, null, "private"),
			("confidence", null, null, "80"),
			("remove_existing", null, null, "false"),
			("trustedcircles", null, null, "10,20"),
			("expiration_ts", null, null, "2026-01-02T03:04:05Z"),
			("tags", null, null, """[{"name":"t","tlp":"red"}]"""),
			("notes", null, null, "from a test"));
	}

	[Fact]
	public async Task MultipartRequest_OmitsNullProperties_AndDefaultsTheFileType()
	{
		var (probe, stub, client) = Create("{}");
		using (client)
		{
			await probe.UploadAsync(new ProbeUploadRequest { File = new ThreatStreamFile("a.bin", new byte[] { 1 }) }, Ct);
		}

		stub.Calls[0].Parts.Should().ContainSingle().Which.ContentType.Should().Be("application/octet-stream");
	}

	[Fact]
	public async Task MultipartRequest_AdditionalFieldRepeatingAProperty_IsRefused()
	{
		var (probe, stub, client) = Create("{}");
		using (client)
		{
			var act = () => probe.UploadAsync(new ProbeUploadRequest { Confidence = 1, AdditionalFields = { ["confidence"] = "2" } }, Ct);

			await act.Should().ThrowAsync<ArgumentException>().WithMessage("*'confidence'*");
		}

		stub.Calls.Should().BeEmpty();
	}

	[Fact]
	public async Task TextResponse_IsReturnedAsIs()
	{
		var (probe, _, client) = Create("value,notes\n1.2.3.4,x\n");
		using (client)
		{
			(await probe.ExportAsync(Ct)).Should().Be("value,notes\n1.2.3.4,x\n");
		}
	}

	[Theory]
	[InlineData(new[] { "a", "b" }, "a,b")]
	[InlineData(new string[0], "")]
	public void Multipart_StringListsAreCommaSeparated(string[] values, string expected)
		=> Serialization.MultipartEncoder.FormatValue(values).Should().Be(expected);

	[Fact]
	public void Multipart_ListsWithNullsSkipThem_AndMixedListsAreJson()
	{
		Serialization.MultipartEncoder.FormatValue(new List<long?> { 1, null, 3 }).Should().Be("1,3");
		Serialization.MultipartEncoder.FormatValue(new List<object> { 1, new ProbeTag("t", "red") }).Should().Be("""[1,{"name":"t","tlp":"red"}]""");
	}
}
