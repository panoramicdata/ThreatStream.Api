namespace ThreatStream.Api.Test.Support;

/// <summary>
/// Assertions for the read-only mode, shared by every group (imported with <c>using static</c> by the test project): a
/// read-only client must refuse each operation that changes ThreatStream before anything is sent, and still send reads,
/// including the read-only POSTs on its allow-list.
/// </summary>
internal static class SideEffectTestKit
{
	/// <summary>
	/// Asserts that a read-only client refuses <paramref name="call"/> with <paramref name="method"/> on
	/// <paramref name="path"/> (relative to <see cref="TestClient.BaseUrl"/>, with or without a leading slash) and sends
	/// nothing.
	/// </summary>
	public static async Task ShouldBeRefusedWhenReadOnlyAsync(Func<ThreatStreamClient, CancellationToken, Task> call, string method, string path)
	{
		var stub = new StubHandler();
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);

		var act = () => call(client, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<ThreatStreamReadOnlyException>();
		thrown.Which.Method.Should().Be(method);
		thrown.Which.Path.Should().Be(TestClient.BaseUrl + path.TrimStart('/'));
		stub.Calls.Should().BeEmpty();
	}

	/// <summary>
	/// Asserts that a read-only client sends <paramref name="call"/>, a read-only operation, exactly once, and returns what
	/// it read.
	/// </summary>
	public static async Task<T> ReadWhenReadOnlyAsync<T>(Func<ThreatStreamClient, CancellationToken, Task<T>> call, string response)
	{
		var stub = TestClient.Stub(response);
		using var client = TestClient.Create(stub, o => o.IsReadOnly = true);
		var result = await call(client, TestContext.Current.CancellationToken);
		stub.Calls.Should().ContainSingle();
		return result;
	}
}
