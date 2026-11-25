namespace ThreatStream.Api.Test.Support;

/// <summary>
/// Assertions for operations that change ThreatStream (associations, bulk tagging, rules, whitelist, workgroups): a
/// read-only client must refuse them before anything is sent, and allow the read-only POSTs on its allow-list.
/// </summary>
internal static class SideEffectTestKit
{
	/// <summary>Asserts that a read-only client refuses <paramref name="call"/> with <paramref name="method"/> and sends nothing.</summary>
	public static async Task ShouldBeRefusedWhenReadOnlyAsync(Func<ThreatStreamClient, CancellationToken, Task> call, string method)
	{
		var stub = new StubHandler();
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		var act = () => call(client, TestContext.Current.CancellationToken);

		(await act.Should().ThrowAsync<ThreatStreamReadOnlyException>()).Which.Method.Should().Be(method);
		stub.Calls.Should().BeEmpty();
	}

	/// <summary>Asserts that a read-only client sends <paramref name="call"/>, a read-only operation, and returns what it read.</summary>
	public static async Task<T> ReadWhenReadOnlyAsync<T>(Func<ThreatStreamClient, CancellationToken, Task<T>> call, string response)
	{
		using var client = TestClient.Create(TestClient.Stub(response), o => o.IsReadOnly = true);
		return await call(client, TestContext.Current.CancellationToken);
	}
}
