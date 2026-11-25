using ThreatStream.Api.Models;
using ThreatStream.Api.Models.ThreatModels;

namespace ThreatStream.Api.IntegrationTest.ThreatModelsB;

/// <summary>The read checks the malware, signature, threat bulletin, TTP and vulnerability tests share.</summary>
internal static class ThreatModelsBReads
{
	/// <summary>
	/// Lists one entity and, when the deployment has any, reads it back by ID; two requests, as entity lists are limited to
	/// one a minute.
	/// </summary>
	public static async Task ListThenGetAsync<T>(
		Func<ThreatModelListOptions, CancellationToken, Task<ThreatStreamPage<T>>> list,
		Func<long, CancellationToken, Task<T>> get,
		string type) where T : ThreatModelEntity
	{
		var cancellationToken = TestContext.Current.CancellationToken;

		var page = await list(new ThreatModelListOptions { Limit = 1 }, cancellationToken);

		page.Meta.Should().NotBeNull();
		page.Objects.Should().HaveCountLessThanOrEqualTo(1);
		if (page.Objects is [var first])
		{
			first.ResourceUri.Should().Be($"/api/v1/{type}/{first.Id}/");
			var detail = await get(first.Id, cancellationToken);
			detail.Id.Should().Be(first.Id);
			detail.Name.Should().Be(first.Name);
		}
	}
}
