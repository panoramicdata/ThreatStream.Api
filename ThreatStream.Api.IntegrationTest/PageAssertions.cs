using ThreatStream.Api.Models;

namespace ThreatStream.Api.IntegrationTest;

/// <summary>Checks on a live page whose content depends on the account, so only its shape can be asserted.</summary>
internal static class PageAssertions
{
	/// <summary>Asserts that <paramref name="page"/> has its paging metadata, at most <paramref name="limit"/> objects, and that each satisfies <paramref name="each"/>.</summary>
	public static void ShouldBeAPageOf<T>(this ThreatStreamPage<T> page, int limit, Action<T> each)
	{
		page.Meta.Should().NotBeNull();
		page.Objects.Count.Should().BeLessThanOrEqualTo(limit);
		page.Objects.Should().AllSatisfy(each);
	}
}
