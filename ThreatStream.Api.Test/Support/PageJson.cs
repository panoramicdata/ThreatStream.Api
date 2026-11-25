namespace ThreatStream.Api.Test.Support;

/// <summary>ThreatStream's list envelope, shared by every group (imported with <c>using static</c> by the test project).</summary>
internal static class PageJson
{
	/// <summary>An empty page.</summary>
	public const string EmptyPage = """{"meta": {"limit": 20, "offset": 0, "next": null, "previous": null, "total_count": 0}, "objects": []}""";

	/// <summary>A one-object page holding <paramref name="json"/>.</summary>
	public static string Page(string json) => $$"""
		{
			"meta": {"limit": 20, "offset": 0, "next": null, "previous": null, "total_count": 1},
			"objects": [{{json}}]
		}
		""";
}
