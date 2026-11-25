using System.Net;
using ThreatStream.Api.Handlers;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public partial class AuthenticationHandlerTests
{
	[Fact]
	public async Task OAuth_ConcurrentRequests_ObtainOneToken()
	{
		var requested = new TaskCompletionSource();
		var release = new TaskCompletionSource();
		var transport = new DelegateHandler(async (request, _) =>
		{
			if (request.RequestUri!.AbsolutePath == "/oauth/token")
			{
				requested.SetResult();
				await release.Task;
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TestClient.TokenJson("shared")) };
			}

			return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Trend) };
		});
		using var client = TestClient.Create(transport, o =>
		{
			TestClient.UseOAuth(o);
			o.TimeProvider = new ManualTimeProvider();
		});

		var first = client.Confidence.GetTrendAsync("a", Ct);
		await requested.Task.WaitAsync(Ct);
		var second = client.Confidence.GetTrendAsync("b", Ct);
		release.SetResult();
		await Task.WhenAll(first, second);

		transport.Requests.Count(r => r.RequestUri!.AbsolutePath == "/oauth/token").Should().Be(1);
		transport.Authorizations.Where(a => a is not null).Should().AllBe("Bearer shared").And.HaveCount(2);
	}

	[Theory]
	[InlineData("""{"access_token":"t","expires_in":120}""", 120)]
	[InlineData("""{"access_token":"t","expires_in":"300"}""", 300)]
	[InlineData("""{"access_token":"t","expires_in":1.5}""", 1.5)]
	[InlineData("""{"access_token":"t"}""", 900)]
	[InlineData("""{"access_token":"t","expires_in":null}""", 900)]
	[InlineData("""{"access_token":"t","expires_in":"soon"}""", 900)]
	[InlineData("""{"access_token":"t","expires_in":0}""", 900)]
	[InlineData("""{"access_token":"t","expires_in":-5}""", 900)]
	[InlineData("""{"access_token":"t","expires_in":1e300}""", 900)]
	public void ReadToken_ReadsTheLifetimeTolerantly(string body, double seconds)
	{
		var (value, lifetime) = AuthenticationHandler.ReadToken(body);

		value.Should().Be("t");
		lifetime.Should().Be(TimeSpan.FromSeconds(seconds));
	}

	[Theory]
	[InlineData("")]
	[InlineData("not json")]
	[InlineData("[]")]
	[InlineData("{}")]
	[InlineData("""{"access_token":""}""")]
	[InlineData("""{"access_token":5}""")]
	public void ReadToken_WithoutAToken_Throws(string body)
	{
		var act = () => AuthenticationHandler.ReadToken(body);

		act.Should().Throw<InvalidOperationException>().WithMessage("*did not contain an access token.");
	}

	[Theory]
	[InlineData(900, 60)]
	[InlineData(120, 60)]
	[InlineData(119, 59.5)]
	[InlineData(1, 0.5)]
	public void Margin_IsAMinuteOrHalfTheLifetime(double lifetime, double margin)
		=> AuthenticationHandler.Margin(TimeSpan.FromSeconds(lifetime)).Should().Be(TimeSpan.FromSeconds(margin));

	[Fact]
	public async Task OAuth_TokenWithoutAccessToken_RaisesInvalidOperation()
	{
		using var client = OAuthClient(TestClient.Stub("""{"token_type":"Bearer"}"""), new ManualTimeProvider());

		var act = () => client.Confidence.GetTrendAsync("a", Ct);

		await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*did not contain an access token.");
	}
}
