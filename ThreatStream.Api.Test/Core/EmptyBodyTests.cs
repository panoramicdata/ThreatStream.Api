using Refit;
using System.Net;
using System.Reflection;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ObservableImports;
using ThreatStream.Api.Models.ObservableTags;
using ThreatStream.Api.Models.Rules;
using ThreatStream.Api.Models.Sandbox;
using ThreatStream.Api.Models.Snapshots;
using ThreatStream.Api.Models.ThreatModels;
using ThreatStream.Api.Models.UserAdministration;
using ThreatStream.Api.Models.UsersRoles;
using ThreatStream.Api.Models.Whitelist;
using ThreatStream.Api.Models.Workgroups;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

/// <summary>
/// ThreatStream may accept a write with <c>202 Accepted</c> or <c>204 No Content</c> and no body, which Refit reads as
/// <see langword="null"/>. Every write that returns a model, and every job start, therefore returns a nullable model, and
/// these tests pin that each one hands back <see langword="null"/> rather than failing.
/// </summary>
public class EmptyBodyTests
{
	/// <summary>Operations that are not GETs but only read, and so always answer with a body.</summary>
	private static readonly HashSet<string> ReadOnlyPosts = ["IEnrichments.LookupAsync"];

	private static readonly Dictionary<string, Func<ThreatStreamClient, CancellationToken, Task<object?>>> Writes = new()
	{
		["IActors.CreateAsync"] = async (c, ct) => await c.Actors.CreateAsync(new() { Name = "x" }, ct),
		["IActors.UpdateAsync"] = async (c, ct) => await c.Actors.UpdateAsync(1, new(), ct),
		["IAttackPatterns.CreateAsync"] = async (c, ct) => await c.AttackPatterns.CreateAsync(new() { Name = "x" }, ct),
		["IAttackPatterns.UpdateAsync"] = async (c, ct) => await c.AttackPatterns.UpdateAsync(1, new(), ct),
		["ICampaigns.CreateAsync"] = async (c, ct) => await c.Campaigns.CreateAsync(new() { Name = "x" }, ct),
		["ICampaigns.UpdateAsync"] = async (c, ct) => await c.Campaigns.UpdateAsync(1, new(), ct),
		["IIncidents.CreateAsync"] = async (c, ct) => await c.Incidents.CreateAsync(new() { Name = "x" }, ct),
		["IIncidents.UpdateAsync"] = async (c, ct) => await c.Incidents.UpdateAsync(1, new(), ct),
		["IMalware.CreateAsync"] = async (c, ct) => await c.Malware.CreateAsync(new() { Name = "x", IsFamily = true, MalwareTypes = ["worm"] }, ct),
		["IMalware.UpdateAsync"] = async (c, ct) => await c.Malware.UpdateAsync(1, new(), ct),
		["ISignatures.CreateAsync"] = async (c, ct) => await c.Signatures.CreateAsync(new() { Name = "x", SignatureType = 3 }, ct),
		["ISignatures.UpdateAsync"] = async (c, ct) => await c.Signatures.UpdateAsync(1, new(), ct),
		["IThreatBulletins.CreateAsync"] = async (c, ct) => await c.ThreatBulletins.CreateAsync(new() { Name = "x" }, ct),
		["IThreatBulletins.UpdateAsync"] = async (c, ct) => await c.ThreatBulletins.UpdateAsync(1, new(), ct),
		["IThreatBulletins.AddAttachmentAsync"] = async (c, ct) => await c.ThreatBulletins.AddAttachmentAsync(1, Attachment, ct),
		["ITtps.CreateAsync"] = async (c, ct) => await c.Ttps.CreateAsync(new() { Name = "x" }, ct),
		["ITtps.UpdateAsync"] = async (c, ct) => await c.Ttps.UpdateAsync(1, new(), ct),
		["IVulnerabilities.CreateAsync"] = async (c, ct) => await c.Vulnerabilities.CreateAsync(new() { Name = "x" }, ct),
		["IVulnerabilities.UpdateAsync"] = async (c, ct) => await c.Vulnerabilities.UpdateAsync(1, new(), ct),
		["IOrgAdminUsers.CreateAsync"] = async (c, ct) => await c.OrgAdminUsers.CreateAsync(new OrganizationUserCreateRequest { Email = "a@example.com" }, ct),
		["IOrgAdminUsers.UpdateAsync"] = async (c, ct) => await c.OrgAdminUsers.UpdateAsync(1, new OrganizationUserUpdateRequest(), ct),
		["ISsoPermissions.CreateAsync"] = async (c, ct) => await c.SsoPermissions.CreateAsync(new SsoGroupMappingCreateRequest { Group = "g", IdpId = 1, Roles = [2] }, ct),
		["ISsoPermissions.UpdateAsync"] = async (c, ct) => await c.SsoPermissions.UpdateAsync(1, new SsoGroupMappingUpdateRequest { IdpId = 1 }, ct),
		["IUserRoles.CreateAsync"] = async (c, ct) => await c.UserRoles.CreateAsync(new UserRoleCreateRequest { Name = "r", Permissions = [] }, ct),
		["IUserRoles.UpdateAsync"] = async (c, ct) => await c.UserRoles.UpdateAsync(1, new UserRoleUpdateRequest(), ct),
		["IRules.CreateAsync"] = async (c, ct) => await c.Rules.CreateAsync(new RuleCreateRequest { Name = "r", Keywords = ["k"] }, ct),
		["IRules.UpdateAsync"] = async (c, ct) => await c.Rules.UpdateAsync(1, new RuleUpdateRequest(), ct),
		["ISnapshots.CreateAsync"] = async (c, ct) => await c.Snapshots.CreateAsync(new SnapshotCreateRequest { CustomConfig = new SnapshotConfig() }, ct),
		["IWhitelist.UpdateAsync"] = async (c, ct) => await c.Whitelist.UpdateAsync(1, new WhitelistUpdateRequest { Notes = "n" }, ct),
		["ISandboxSubmissions.SubmitAsync"] = async (c, ct) => await c.SandboxSubmissions.SubmitAsync(new SandboxSubmitRequest { Url = "http://a.example/" }, ct),
		["IObservableWorkgroups.RestrictAsync"] = async (c, ct) => await c.ObservableWorkgroups.RestrictAsync(new BulkWorkgroupsRequest { Ids = [1], Workgroups = [2] }, ct),
		["IObservableImports.ImportAsync"] = async (c, ct) => await c.ObservableImports.ImportAsync(new ObservableImportRequest { DataText = "x" }, ct),
		["IObservables.CloneAsync"] = async (c, ct) => await c.Observables.CloneAsync(1, ct),
		["IObservableTags.BulkTagMatchingAsync"] = async (c, ct) => await c.ObservableTags.BulkTagMatchingAsync(new ObservableTagQueryRequest { Query = new() { Q = "x" } }, ct),
		["IUserActions.StartReportAsync"] = async (c, ct) => await c.UserActions.StartReportAsync(null, ct),
		["IApiUserActions.StartReportAsync"] = async (c, ct) => await c.ApiUserActions.StartReportAsync(null, ct)
	};

	private static ThreatBulletinAttachmentRequest Attachment => new() { Attachment = new ThreatStreamFile("a.txt", "a"u8.ToArray()), FileName = "a.txt" };

	public static TheoryData<string> WriteNames => [.. Writes.Keys];

	[Theory]
	[MemberData(nameof(WriteNames))]
	public async Task Write_AnsweredWithoutABody_ReturnsNull(string method)
	{
		foreach (var status in new[] { HttpStatusCode.Accepted, HttpStatusCode.NoContent })
		{
			var stub = new StubHandler();
			stub.Enqueue(status, string.Empty);
			using var client = TestClient.Create(stub);

			(await Writes[method](client, TestContext.Current.CancellationToken)).Should().BeNull("{0} answered {1} without a body", method, status);
		}
	}

	[Fact]
	public void EveryWriteReturningAModel_IsNullable_AndPinnedHere()
	{
		var context = new NullabilityInfoContext();
		var writes = typeof(ThreatStreamClient).Assembly.GetExportedTypes()
			.Where(t => t.IsInterface && t.Namespace == "ThreatStream.Api.Interfaces")
			.SelectMany(t => t.GetMethods().Select(m => (Name: $"{t.Name}.{m.Name}", Method: m)))
			.Where(o => o.Method.GetCustomAttribute<HttpMethodAttribute>()!.Method != HttpMethod.Get
				&& o.Method.ReturnType.IsGenericType
				&& !ReadOnlyPosts.Contains(o.Name))
			.ToList();

		writes.Where(o => context.Create(o.Method.ReturnParameter).GenericTypeArguments[0].ReadState != NullabilityState.Nullable)
			.Select(o => o.Name)
			.Should().BeEmpty("a write may be answered without a body");
		writes.Select(o => o.Name).Should().BeSubsetOf(Writes.Keys);
	}
}
