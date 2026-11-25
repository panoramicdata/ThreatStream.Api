namespace ThreatStream.Api.Test.Core;

public class OptionsTests
{
	private const string Thumbprint = "AB:CD:EF:01:23:45:67:89:ab:cd:ef:01:23:45:67:89:AB:CD:EF:01:23:45:67:89:AB:CD:EF:01:23:45:67:89";

	private static ThreatStreamClientOptions Valid(Action<ThreatStreamClientOptions>? tweak = null)
	{
		var options = new ThreatStreamClientOptions { Username = "analyst@example.com", ApiKey = "key" };
		tweak?.Invoke(options);
		return options;
	}

	private static void Custom(ThreatStreamClientOptions options, string baseUrl)
		=> (options.Region, options.BaseUrl) = (ThreatStreamRegion.Custom, baseUrl);

	public static TheoryData<string, Action<ThreatStreamClientOptions>> InvalidOptions => new()
	{
		{ "Region", o => o.Region = (ThreatStreamRegion)7 },
		{ "BaseUrl", o => o.BaseUrl = "https://threatstream.example.com/" },
		{ "BaseUrl", o => (o.Region, o.BaseUrl) = (ThreatStreamRegion.Eu, "https://x.test") },
		{ "BaseUrl", o => o.Region = ThreatStreamRegion.Custom },
		{ "BaseUrl", o => Custom(o, "") },
		{ "BaseUrl", o => Custom(o, "threatstream.test/x") },
		{ "BaseUrl", o => Custom(o, "/relative/path") },
		{ "BaseUrl", o => Custom(o, "ftp://threatstream.test/") },
		{ "BaseUrl", o => Custom(o, "https://analyst:secret@threatstream.test/") },
		{ "BaseUrl", o => Custom(o, "https://threatstream.test/?api_key=x") },
		{ "BaseUrl", o => Custom(o, "https://threatstream.test/#top") },
		{ "ClientId", o => o.ClientId = "id" },
		{ "ClientId", o => o.ClientSecret = "secret" },
		{ "ClientId", o => (o.Username, o.ApiKey, o.ClientSecret) = (null, null, "secret") },
		{ "ClientSecret", o => (o.Username, o.ApiKey, o.ClientId) = (null, null, "id") },
		{ "Username", o => (o.Username, o.ApiKey) = (null, null) },
		{ "Username", o => o.Username = " " },
		{ "ApiKey", o => o.ApiKey = null },
		{ "thumbprint", o => o.TrustedServerCertificateThumbprint = "ABCD" },
		{ "MaxRetries", o => o.MaxRetries = -1 },
		{ "Timeout", o => o.Timeout = TimeSpan.Zero },
		{ "Timeout", o => o.Timeout = TimeSpan.MaxValue },
		{ "RetryBaseDelay", o => o.RetryBaseDelay = TimeSpan.FromTicks(-1) },
		{ "MaxRetryDelay", o => o.MaxRetryDelay = TimeSpan.Zero },
		{ "MaxRetryDelay", o => o.MaxRetryDelay = TimeSpan.FromDays(30) }
	};

	[Theory]
	[MemberData(nameof(InvalidOptions))]
	public void Validate_RejectsInvalidOptions(string parameter, Action<ThreatStreamClientOptions> tweak)
	{
		var options = Valid(tweak);

		var act = options.Validate;

		act.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(parameter);
	}

	public static TheoryData<Action<ThreatStreamClientOptions>> ValidOptions => new()
	{
		o => { },
		o => o.Region = ThreatStreamRegion.Eu,
		o => o.BaseUrl = " ",
		o => Custom(o, "http://threatstream.test/prefix/"),
		o => (o.Username, o.ApiKey, o.ClientId, o.ClientSecret) = (null, " ", "id", "secret"),
		o => o.TrustedServerCertificateThumbprint = Thumbprint,
		o => o.TrustedServerCertificateThumbprint = " ",
		o => (o.MaxRetries, o.RetryBaseDelay) = (0, TimeSpan.Zero),
		o => (o.Timeout, o.MaxRetryDelay) = (TimeSpan.FromMilliseconds(int.MaxValue), TimeSpan.FromMilliseconds(int.MaxValue))
	};

	[Theory]
	[MemberData(nameof(ValidOptions))]
	public void Validate_AcceptsValidOptions(Action<ThreatStreamClientOptions> tweak)
	{
		var act = Valid(tweak).Validate;

		act.Should().NotThrow();
	}

	[Fact]
	public void Defaults_AreDocumentedValues()
	{
		var options = new ThreatStreamClientOptions();

		options.Region.Should().Be(ThreatStreamRegion.Us);
		options.BaseUrl.Should().BeNull();
		options.Timeout.Should().Be(TimeSpan.FromSeconds(100));
		options.MaxRetries.Should().Be(3);
		options.RetryBaseDelay.Should().Be(TimeSpan.FromSeconds(1));
		options.MaxRetryDelay.Should().Be(TimeSpan.FromSeconds(60));
		options.IsReadOnly.Should().BeFalse();
		options.Logger.Should().BeNull();
		options.ServerCertificateValidationCallback.Should().BeNull();
		options.TimeProvider.Should().BeSameAs(TimeProvider.System);
	}

	[Theory]
	[InlineData(ThreatStreamRegion.Us, null, "https://api.threatstream.com/")]
	[InlineData(ThreatStreamRegion.Eu, null, "https://api-eu.threatstream.com/")]
	[InlineData(ThreatStreamRegion.Custom, "https://ts.example.com", "https://ts.example.com")]
	[InlineData(ThreatStreamRegion.Custom, null, "")]
	public void EffectiveBaseUrl_FollowsTheRegion(ThreatStreamRegion region, string? baseUrl, string expected)
		=> new ThreatStreamClientOptions { Region = region, BaseUrl = baseUrl }.EffectiveBaseUrl.Should().Be(expected);

	[Theory]
	[InlineData(null, nameof(AuthenticationKind.ApiKey))]
	[InlineData(" ", nameof(AuthenticationKind.ApiKey))]
	[InlineData("id", nameof(AuthenticationKind.OAuthClientCredentials))]
	public void AuthenticationKind_FollowsTheCredentials(string? clientId, string expected)
		=> new ThreatStreamClientOptions { ClientId = clientId }.AuthenticationKind.ToString().Should().Be(expected);

	[Fact]
	public void ToString_MasksSecrets()
	{
		var options = new ThreatStreamClientOptions
		{
			Region = ThreatStreamRegion.Custom,
			BaseUrl = "https://threatstream.test",
			Username = "analyst@example.com",
			ApiKey = "the-key",
			ClientId = "id",
			ClientSecret = "the-secret",
			IsReadOnly = true
		};

		options.ToString().Should().Be(
			"ThreatStreamClientOptions { Region = Custom, BaseUrl = https://threatstream.test, Username = analyst@example.com, ApiKey = ***, ClientId = id, ClientSecret = ***, IsReadOnly = True }");
	}

	[Fact]
	public void ToString_ShowsMissingSecretsAsNone()
		=> new ThreatStreamClientOptions().ToString().Should().Be(
			"ThreatStreamClientOptions { Region = Us, BaseUrl = , Username = , ApiKey = (none), ClientId = , ClientSecret = (none), IsReadOnly = False }");

	[Theory]
	[InlineData("https://analyst:secret@threatstream.test/x?y=1", "https://***@threatstream.test/x?y=1")]
	[InlineData("https://analyst:p@ss@threatstream.test", "https://***@threatstream.test")]
	[InlineData("https://threatstream.test/a@b", "https://threatstream.test/a@b")]
	[InlineData("https://threatstream.test", "https://threatstream.test")]
	[InlineData("not a url@all", "not a url@all")]
	public void ToString_MasksCredentialsInBaseUrl(string baseUrl, string shown)
	{
		var text = new ThreatStreamClientOptions { BaseUrl = baseUrl }.ToString();

		text.Should().StartWith($"ThreatStreamClientOptions {{ Region = Us, BaseUrl = {shown}, ");
		text.Should().NotContain("secret").And.NotContain("p@ss");
	}
}
