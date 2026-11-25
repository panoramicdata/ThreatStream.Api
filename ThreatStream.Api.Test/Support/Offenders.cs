namespace ThreatStream.Api.Test.Support;

/// <summary>Assertions for the rule-enforcing core tests, which list every offender rather than stopping at the first.</summary>
internal static class Offenders
{
	/// <summary>Asserts that <paramref name="offending"/> is empty, listing each offender, one per line, when it is not.</summary>
	public static void AssertNone(IEnumerable<string> offending, string because)
	{
		var list = offending.ToList();
		list.Should().BeEmpty("{0}; found {1}:{2}{3}", because, list.Count, Environment.NewLine, string.Join(Environment.NewLine, list));
	}
}
