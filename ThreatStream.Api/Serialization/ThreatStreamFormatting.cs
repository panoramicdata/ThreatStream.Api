using System.Globalization;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Formats single values the way ThreatStream reads them in a query string or a multipart form field: booleans as
/// lowercase <c>true</c>/<c>false</c>, enums by their wire name, dates as UTC ISO 8601, numbers in the invariant culture.
/// </summary>
internal static class ThreatStreamFormatting
{
	/// <summary>Formats a single value.</summary>
	public static string FormatScalar(object value)
		=> value switch
		{
			string text => text,
			bool flag => flag ? "true" : "false",
			Enum member => WireNames.Of(member),
			DateTimeOffset moment => FormatDate(moment),
			DateTime moment => FormatDate(moment.Kind == DateTimeKind.Local
				? new DateTimeOffset(moment)
				: new DateTimeOffset(DateTime.SpecifyKind(moment, DateTimeKind.Utc))),
			IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
			_ => value.ToString() ?? string.Empty
		};

	/// <summary>
	/// Formats a moment as UTC ISO 8601 with a <c>Z</c> suffix, such as <c>2026-01-02T03:04:05Z</c>, keeping a fraction of
	/// a second only when there is one. ThreatStream reads timestamps without an offset as UTC, so the moment is converted
	/// to UTC first, whatever its offset.
	/// </summary>
	public static string FormatDate(DateTimeOffset moment)
		=> moment.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'", CultureInfo.InvariantCulture);
}
