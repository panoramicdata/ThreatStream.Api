using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Reads a nullable <see cref="DateTimeOffset"/> from ISO 8601 text (ThreatStream's usual <c>2026-01-02T03:04:05</c>
/// carries no offset and is UTC, so text without an offset is read as UTC) or from Unix epoch seconds (a number or numeric
/// string, fractions kept to the millisecond). JSON <c>null</c>, an empty string and 0 read as <see langword="null"/>;
/// unreadable text or a value outside the range of <see cref="DateTimeOffset"/> raises a <see cref="JsonException"/>.
/// Writes UTC ISO 8601 text (see <see cref="ThreatStreamFormatting.FormatDate(DateTimeOffset)"/>). Registered in
/// <see cref="ThreatStreamJson.Options"/> for every <c>DateTimeOffset?</c> property.
/// </summary>
internal sealed class TolerantDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
	// The Unix milliseconds of DateTimeOffset.MinValue and MaxValue.
	private const double MinMilliseconds = -62_135_596_800_000;
	private const double MaxMilliseconds = 253_402_300_799_999;

	/// <inheritdoc />
	public override bool HandleNull => true;

	/// <inheritdoc />
	public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String
			&& reader.GetString()!.Trim() is { Length: > 0 } text
			&& !double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
		{
			return DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed)
				? parsed
				: throw new JsonException($"Cannot read a date from \"{text}\".");
		}

		if (TolerantReader.ReadNumberText(ref reader) is not { } number)
		{
			return null;
		}

		var seconds = TolerantReader.ParseDouble(number);
		if (seconds == 0)
		{
			return null;
		}

		// Checked here: a NaN would cast to 0 (1970), and an infinity or a huge value would make FromUnixTimeMilliseconds
		// throw ArgumentOutOfRangeException rather than a JsonException naming the property.
		var milliseconds = Math.Round(seconds * 1000);
		return milliseconds is >= MinMilliseconds and <= MaxMilliseconds
			? DateTimeOffset.FromUnixTimeMilliseconds((long)milliseconds)
			: throw new JsonException($"\"{number}\" is not a valid epoch time in seconds.");
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
	{
		if (value is { } moment)
		{
			writer.WriteStringValue(ThreatStreamFormatting.FormatDate(moment));
		}
		else
		{
			writer.WriteNullValue();
		}
	}
}
