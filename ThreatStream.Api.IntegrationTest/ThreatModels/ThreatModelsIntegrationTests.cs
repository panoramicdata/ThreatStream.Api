using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.IntegrationTest.ThreatModels;

/// <summary>
/// Read-only checks of the actor, attack pattern, campaign and incident endpoints: each lists a page, then reads the first
/// entity it found (if the account can see any) by ID. Creates and updates are never run live.
/// </summary>
[Collection(ThreatStreamTestGroup.Name)]
public class ThreatModelsIntegrationTests(ThreatStreamFixture fixture)
{
	private static readonly ThreatModelListOptions FirstTwo = new() { Limit = 2 };

	private static CancellationToken Token => TestContext.Current.CancellationToken;

	[Fact]
	public Task Actors_ListThenGet_ReturnWellFormedActors()
		=> ListThenGetAsync("actor", ct => fixture.Client.Actors.ListAsync(FirstTwo, ct), (id, ct) => fixture.Client.Actors.GetAsync(id, null, ct));

	[Fact]
	public Task AttackPatterns_ListThenGet_ReturnWellFormedAttackPatterns()
		=> ListThenGetAsync("attackpattern", ct => fixture.Client.AttackPatterns.ListAsync(FirstTwo, ct), (id, ct) => fixture.Client.AttackPatterns.GetAsync(id, null, ct));

	[Fact]
	public Task Campaigns_ListThenGet_ReturnWellFormedCampaigns()
		=> ListThenGetAsync("campaign", ct => fixture.Client.Campaigns.ListAsync(FirstTwo, ct), (id, ct) => fixture.Client.Campaigns.GetAsync(id, null, ct));

	[Fact]
	public Task Incidents_ListThenGet_ReturnWellFormedIncidents()
		=> ListThenGetAsync("incident", ct => fixture.Client.Incidents.ListAsync(FirstTwo, ct), (id, ct) => fixture.Client.Incidents.GetAsync(id, null, ct));

	[Fact]
	public async Task Actors_AssociatedWithAPublicDomain_AreWellFormed()
	{
		// A popular domain: ThreatStream may or may not associate actors with it, so the page may be empty.
		var page = await fixture.Client.Actors.ListAssociatedWithValueAsync("google.com", new ListOptions { Limit = 5 }, Token);

		page.Objects.Should().HaveCountLessThanOrEqualTo(5).And.OnlyContain(a => a.Id > 0);
	}

	private static async Task ListThenGetAsync<T>(string type, Func<CancellationToken, Task<ThreatStreamPage<T>>> list, Func<long, CancellationToken, Task<T>> get)
		where T : ThreatModelEntity
	{
		var page = await list(Token);

		page.Meta.Should().NotBeNull();
		page.Objects.Should().HaveCountLessThanOrEqualTo(2);
		page.Objects.Should().OnlyContain(e => e.Id > 0 && e.ResourceUri!.Contains($"/api/v1/{type}/"));
		if (page.Objects.Count > 0)
		{
			var first = page.Objects[0];
			var entity = await get(first.Id, Token);
			entity.Id.Should().Be(first.Id);
			entity.Name.Should().Be(first.Name);
		}
	}
}
