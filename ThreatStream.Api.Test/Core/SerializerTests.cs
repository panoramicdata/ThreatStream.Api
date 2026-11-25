using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using ThreatStream.Api.Serialization;

namespace ThreatStream.Api.Test.Core;

public class SerializerTests
{
	private readonly ThreatStreamContentSerializer _serializer = new();

	private sealed class Named
	{
		[JsonPropertyName("wire.name")]
		public string? Renamed { get; init; }

		public string? PlainName { get; init; }

		public DateTimeOffset? ModifiedTs { get; init; }
	}

	[Fact]
	public async Task ToHttpContent_IsBufferedJson_WithSnakeCaseNamesAndUtcDates()
	{
		using var content = _serializer.ToHttpContent(new Named
		{
			Renamed = "x",
			PlainName = "y",
			ModifiedTs = new DateTimeOffset(2026, 1, 2, 4, 4, 5, TimeSpan.FromHours(1))
		});

		content.Should().BeAssignableTo<ByteArrayContent>();
		content.Headers.ContentType!.MediaType.Should().Be("application/json");
		(await content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Be("""{"wire.name":"x","plain_name":"y","modified_ts":"2026-01-02T03:04:05Z"}""");
	}

	[Fact]
	public async Task ToHttpContent_OmitsNulls()
	{
		using var content = _serializer.ToHttpContent(new Named());

		(await content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Be("{}");
	}

	[Fact]
	public void ToHttpContent_RequiresABody()
	{
		var act = () => _serializer.ToHttpContent<object?>(null);

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public async Task FromHttpContent_ReadsWithThreatStreamJsonOptions()
	{
		using var content = new StringContent("""{"WIRE.NAME":"x","plain_name":5}""", Encoding.UTF8, "application/json");

		var value = await _serializer.FromHttpContentAsync<Named>(content, TestContext.Current.CancellationToken);

		value!.Renamed.Should().Be("x");
		value.PlainName.Should().Be("5");
	}

	[Theory]
	[InlineData(nameof(Named.Renamed), "wire.name")]
	[InlineData(nameof(Named.PlainName), null)]
	public void GetFieldNameForProperty_UsesJsonPropertyName(string property, string? expected)
		=> _serializer.GetFieldNameForProperty(typeof(Named).GetProperty(property)!).Should().Be(expected);

	private enum Tlp
	{
		Unknown = 0,

		[JsonStringEnumMemberName("amber+strict")]
		AmberStrict = 1
	}

	public static TheoryData<object?, string?> UrlValues => new()
	{
		{ null, null },
		{ true, "true" },
		{ false, "false" },
		{ Tlp.AmberStrict, "amber+strict" },
		{ new DateTimeOffset(2026, 1, 2, 4, 4, 5, TimeSpan.FromHours(1)), "2026-01-02T03:04:05Z" },
		{ new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Unspecified), "2026-01-02T03:04:05Z" },
		{ new DateTime(2026, 1, 2, 3, 4, 5, 500, DateTimeKind.Utc), "2026-01-02T03:04:05.5Z" },
		{ 1.5, "1.5" },
		{ 12, "12" },
		{ "text", "text" }
	};

	[Theory]
	[MemberData(nameof(UrlValues))]
	public void UrlParameterFormatter_FormatsInvariantly(object? value, string? expected)
	{
		var previous = CultureInfo.CurrentCulture;
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
		try
		{
			new ThreatStreamUrlParameterFormatter().Format(value, Refit.GeneratedParameterAttributeProvider.Empty, value?.GetType() ?? typeof(object)).Should().Be(expected);
		}
		finally
		{
			CultureInfo.CurrentCulture = previous;
		}
	}

	[Fact]
	public void FormatScalar_ConvertsLocalTimesToUtc()
	{
		var local = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Local);

		ThreatStreamFormatting.FormatScalar(local).Should().Be(ThreatStreamFormatting.FormatDate(new DateTimeOffset(local)));
	}

	[Fact]
	public void FormatScalar_UsesToStringForOtherValues()
	{
		ThreatStreamFormatting.FormatScalar(new Uri("https://x.test/a")).Should().Be("https://x.test/a");
		ThreatStreamFormatting.FormatScalar(new object()).Should().Be("System.Object");
		ThreatStreamFormatting.FormatScalar(new NoText()).Should().BeEmpty();
	}

	private sealed class NoText
	{
		public override string? ToString() => null;
	}
}
