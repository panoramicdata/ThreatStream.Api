using Refit;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Formats query and path parameters as <see cref="ThreatStreamFormatting.FormatScalar(object)"/> does: booleans as
/// lowercase <c>true</c>/<c>false</c>, enums by their wire name, dates as UTC ISO 8601, numbers in the invariant culture.
/// </summary>
internal sealed class ThreatStreamUrlParameterFormatter : DefaultUrlParameterFormatter
{
	/// <inheritdoc />
	public override string? Format(object? value, IParameterAttributeProvider attributeProvider, Type type)
		=> value switch
		{
			null => null,
			bool or Enum or DateTimeOffset or DateTime => ThreatStreamFormatting.FormatScalar(value),
			_ => base.Format(value, attributeProvider, type)
		};
}
