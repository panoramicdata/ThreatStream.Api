namespace ThreatStream.Api.Test.Support;

/// <summary>Assertions on a request recorded by <see cref="StubHandler"/>, shared by every endpoint group's tests.</summary>
internal static class RecordedCallAssertions
{
	/// <summary>The query string of a request that sends no parameters.</summary>
	public const string NoQuery = "";

	/// <summary>Asserts the method, escaped path, query and body of a recorded request; every test kit's request assertion builds on it.</summary>
	public static void ShouldMatch(this RecordedCall call, HttpMethod method, string path, string query, string? body)
	{
		call.Method.Should().Be(method);
		call.Uri.AbsolutePath.Should().Be(path);
		call.Uri.Query.Should().Be(query);
		call.Body.Should().Be(body);
	}

	/// <summary>
	/// Asserts the method, escaped path (including the <c>/api/vN/</c> prefix and any trailing slash), query (with its
	/// leading <c>?</c>, or empty) and body of a recorded request, and that it was authenticated. A body must be sent as
	/// <c>application/json</c> unless it is multipart (assert its <see cref="RecordedCall.Parts"/> instead) or deliberately
	/// another type (which the calling test asserts itself, passing <paramref name="contentType"/>).
	/// </summary>
	public static void ShouldBe(this RecordedCall call, HttpMethod method, string path, string query = NoQuery, string? body = null, string contentType = "application/json")
	{
		call.ShouldMatch(method, path, query, body);
		call.Authorization.Should().Be(TestClient.ApiKeyAuthorization);
		if (body is not null)
		{
			call.ContentType.Should().Be(contentType);
		}
	}
}
