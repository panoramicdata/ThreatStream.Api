using System.Net;
using System.Text;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Whitelist;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Groups;

public partial class WhitelistTests
{
	private static readonly WhitelistAddRequest AddExample = new()
	{
		Entries = [new WhitelistEntryRequest { ValueType = WhitelistValueType.Domain, Value = "example44.org", Notes = "example domain" }]
	};

	private static readonly WhitelistUpdateRequest NoteUpdate = new() { Notes = "updated note text" };

	private static WhitelistUploadRequest UploadExample => new() { File = new ThreatStreamFile("whitelist.csv", Encoding.UTF8.GetBytes(Csv), "text/csv") };

	[Fact]
	public async Task AddAsync_PostsTheEntriesToBulk()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.AddAsync(AddExample, ct), "{}");

		call.ShouldBe(
			HttpMethod.Post,
			"/api/v1/orgwhitelist/bulk/",
			body: """{"whitelist":[{"value_type":"domain","value":"example44.org","notes":"example domain"}]}""");
	}

	[Fact]
	public async Task AddAsync_SendsEveryValueTypeAndOmitsMissingNotes()
	{
		var request = new WhitelistAddRequest
		{
			Entries = [.. new[]
			{
				WhitelistValueType.Cidr, WhitelistValueType.Email, WhitelistValueType.Ip, WhitelistValueType.Md5,
				WhitelistValueType.Url, WhitelistValueType.UserAgent
			}.Select(t => new WhitelistEntryRequest { ValueType = t, Value = "v" })]
		};

		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.AddAsync(request, ct), "{}");

		call.Body.Should().Be("""{"whitelist":[{"value_type":"cidr","value":"v"},{"value_type":"email","value":"v"},{"value_type":"ip","value":"v"},{"value_type":"md5","value":"v"},{"value_type":"url","value":"v"},{"value_type":"user-agent","value":"v"}]}""");
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task UploadAsync_PostsTheFileWithRemoveExistingInTheQuery(bool removeExisting)
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.UploadAsync(UploadExample, removeExisting, ct), "{}");

		call.ShouldMatch(HttpMethod.Post, "/api/v1/orgwhitelist/upload/", removeExisting ? "?remove_existing=true" : "?remove_existing=false", null);
		call.ContentType.Should().Be("multipart/form-data");
		var part = call.Parts.Should().ContainSingle().Subject;
		part.Name.Should().Be("file");
		part.FileName.Should().Be("whitelist.csv");
		part.ContentType.Should().Be("text/csv");
		part.Text.Should().Be(Csv);
	}

	[Fact]
	public async Task UpdateAsync_PatchesTheNote()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.UpdateAsync(1201, NoteUpdate, ct), EntryJson);

		call.ShouldBe(HttpMethod.Patch, "/api/v1/orgwhitelist/1201/", body: """{"notes":"updated note text"}""");
	}

	[Fact]
	public async Task UpdateAsync_MapsTheEntry()
	{
		var entry = await TestClient.ReadAsync((client, ct) => client.Whitelist.UpdateAsync(1201, NoteUpdate, ct), EntryJson);

		ShouldBeTheExampleEntry(entry!);
	}

	[Fact]
	public async Task DeleteAsync_DeletesTheEntry()
	{
		var call = await TestClient.CaptureAsync((client, ct) => client.Whitelist.DeleteAsync(1201, ct), string.Empty);

		call.ShouldBe(HttpMethod.Delete, "/api/v1/orgwhitelist/1201/");
	}

	[Fact]
	public Task AddAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Whitelist.AddAsync(AddExample, ct), "POST");

	[Fact]
	public Task UploadAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Whitelist.UploadAsync(UploadExample, false, ct), "POST");

	[Fact]
	public Task UpdateAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Whitelist.UpdateAsync(1201, NoteUpdate, ct), "PATCH");

	[Fact]
	public Task DeleteAsync_ReadOnlyClient_Refuses()
		=> SideEffectTestKit.ShouldBeRefusedWhenReadOnlyAsync((client, ct) => client.Whitelist.DeleteAsync(1201, ct), "DELETE");

	[Fact]
	public Task AddAsync_Error_RaisesThreatStreamApiException()
		=> TestClient.ShouldFailAsync(
			(client, ct) => client.Whitelist.AddAsync(AddExample, ct),
			HttpStatusCode.BadRequest,
			"""{"message": "Invalid value_type"}""",
			"Invalid value_type");
}
