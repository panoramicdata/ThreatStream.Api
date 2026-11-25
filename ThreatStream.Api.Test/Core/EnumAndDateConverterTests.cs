using System.Text.Json;
using System.Text.Json.Serialization;
using ThreatStream.Api.Serialization;
using ThreatStream.Api.Test.Support;

namespace ThreatStream.Api.Test.Core;

public class EnumAndDateConverterTests
{
	public enum Tlp
	{
		Unknown = 0,

		[JsonStringEnumMemberName("amber+strict")]
		AmberStrict = 1,

		Green = 2,

#pragma warning disable CA1069 // A deliberate alias: WireNames writes the first member's name and reads both.
		[JsonStringEnumMemberName("verde")]
		Verde = 2
#pragma warning restore CA1069
	}

	public enum Small : byte
	{
		Unknown = 0,
		One = 1
	}

	public enum Big : long
	{
		Unknown = 0,
		Huge = 1L << 40
	}

	private static T ReadEnum<T>(string json) where T : struct, Enum
		=> JsonSerializer.Deserialize<T>(json, ThreatStreamJson.Options);

	[Theory]
	[InlineData("\"amber+strict\"", Tlp.AmberStrict)]
	[InlineData("\"AMBER+STRICT\"", Tlp.AmberStrict)]
	[InlineData("\"Green\"", Tlp.Green)]
	[InlineData("\"verde\"", Tlp.Green)]
	[InlineData("\"AmberStrict\"", Tlp.Unknown)]
	[InlineData("\"purple\"", Tlp.Unknown)]
	[InlineData("\"\"", Tlp.Unknown)]
	[InlineData("1", Tlp.AmberStrict)]
	[InlineData("7", Tlp.Unknown)]
	[InlineData("1.5", Tlp.Unknown)]
	[InlineData("null", Tlp.Unknown)]
	public void Enum_ReadsWireNamesAndNumbers(string json, Tlp expected)
		=> ReadEnum<Tlp>(json).Should().Be(expected);

	[Theory]
	[InlineData("1", Small.One)]
	[InlineData("257", Small.Unknown)]
	[InlineData("-1", Small.Unknown)]
	public void Enum_ReadsNumbersForAByteEnum(string json, Small expected)
		=> ReadEnum<Small>(json).Should().Be(expected);

	[Fact]
	public void Enum_ReadsNumbersForALongEnum()
		=> ReadEnum<Big>("1099511627776").Should().Be(Big.Huge);

	[Fact]
	public void Enum_ReadsNullableEnums()
	{
		JsonSerializer.Deserialize<Tlp?>("null", ThreatStreamJson.Options).Should().BeNull();
		JsonSerializer.Deserialize<ThreatStreamRegion?>("\"eu\"", ThreatStreamJson.Options).Should().Be(ThreatStreamRegion.Eu);
	}

	[Theory]
	[InlineData("true")]
	[InlineData("{}")]
	public void Enum_OtherTokens_Throw(string json)
	{
		var act = () => ReadEnum<Tlp>(json);

		act.Should().Throw<JsonException>().WithMessage("*Cannot read Tlp from a JSON*");
	}

	[Theory]
	[InlineData(Tlp.AmberStrict, "\"amber\\u002Bstrict\"")]
	[InlineData(Tlp.Green, "\"Green\"")]
	[InlineData((Tlp)9, "\"9\"")]
	public void Enum_WritesTheWireName(Tlp value, string json)
		=> JsonSerializer.Serialize(value, ThreatStreamJson.Options).Should().Be(json);

	[Fact]
	public void WireNames_UndefinedValuesAreNumbers_AndUnknownNamesTheDefault()
	{
		WireNames.Of((Small)200).Should().Be("200");
		WireNames.Of(Tlp.AmberStrict).Should().Be("amber+strict");
		WireNames.Parse<Tlp>("VERDE").Should().Be(Tlp.Green);
		WireNames.Parse<Tlp>("nope").Should().Be(Tlp.Unknown);
	}

	[Theory]
	[InlineData("\"2026-01-02T03:04:05\"", "2026-01-02T03:04:05+00:00")]
	[InlineData("\"2026-01-02T03:04:05.123456\"", "2026-01-02T03:04:05.123456+00:00")]
	[InlineData("\"2026-01-02T03:04:05Z\"", "2026-01-02T03:04:05+00:00")]
	[InlineData("\"2026-01-02T04:04:05+01:00\"", "2026-01-02T03:04:05+00:00")]
	[InlineData("\" 2026-01-02 \"", "2026-01-02T00:00:00+00:00")]
	[InlineData("1767323045", "2026-01-02T03:04:05+00:00")]
	[InlineData("\"1767323045\"", "2026-01-02T03:04:05+00:00")]
	[InlineData("1767323045.5", "2026-01-02T03:04:05.5+00:00")]
	[InlineData("253402300799.999", "9999-12-31T23:59:59.999+00:00")]
	[InlineData("-62135596800", "0001-01-01T00:00:00+00:00")]
	public void Date_ReadsIsoTextAsUtc_AndEpochSeconds(string json, string expected)
	{
		var value = Json.Read(new TolerantDateTimeOffsetConverter(), json);

		value.Should().Be(DateTimeOffset.Parse(expected, System.Globalization.CultureInfo.InvariantCulture));
		value!.Value.Offset.Should().Be(TimeSpan.Zero);
	}

	[Theory]
	[InlineData("null")]
	[InlineData("\"\"")]
	[InlineData("\"  \"")]
	[InlineData("0")]
	[InlineData("\"0.0\"")]
	public void Date_NullEmptyOrZero_IsNull(string json)
		=> Json.Read(new TolerantDateTimeOffsetConverter(), json).Should().BeNull();

	[Theory]
	[InlineData("\"soon\"", "Cannot read a date from \"soon\".")]
	[InlineData("\"NaN\"", "*is not a valid epoch time in seconds.")]
	[InlineData("1e300", "*is not a valid epoch time in seconds.")]
	[InlineData("253402300800", "*is not a valid epoch time in seconds.")]
	[InlineData("-62135596801", "*is not a valid epoch time in seconds.")]
	[InlineData("{}", "Cannot read a number from a JSON StartObject.")]
	public void Date_OutOfRangeOrUnreadable_Throws(string json, string message)
	{
		var act = () => Json.Read(new TolerantDateTimeOffsetConverter(), json);

		act.Should().Throw<JsonException>().WithMessage(message);
	}

	[Fact]
	public void Date_WritesUtcIsoOrNull()
	{
		Json.Write<DateTimeOffset?>(new TolerantDateTimeOffsetConverter(), new DateTimeOffset(2026, 1, 2, 4, 4, 5, TimeSpan.FromHours(1))).Should().Be("\"2026-01-02T03:04:05Z\"");
		Json.Write<DateTimeOffset?>(new TolerantDateTimeOffsetConverter(), new DateTimeOffset(2026, 1, 2, 3, 4, 5, 250, TimeSpan.Zero)).Should().Be("\"2026-01-02T03:04:05.25Z\"");
		Json.Write<DateTimeOffset?>(new TolerantDateTimeOffsetConverter(), null).Should().Be("null");
	}

	private sealed record Dated(DateTimeOffset? CreatedTs);

	[Fact]
	public void Options_ReadDatesInModels()
		=> JsonSerializer.Deserialize<Dated>("""{"created_ts":"2026-01-02T03:04:05"}""", ThreatStreamJson.Options)!.CreatedTs
			.Should().Be(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
}
