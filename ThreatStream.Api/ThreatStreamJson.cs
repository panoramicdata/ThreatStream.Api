using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ThreatStream.Api.Serialization;

namespace ThreatStream.Api;

/// <summary>Shared <see cref="JsonSerializerOptions"/> for ThreatStream payloads.</summary>
/// <remarks>
/// ThreatStream is loose about JSON types: the same property can arrive as <c>true</c>, <c>1</c> or <c>"true"</c>, numbers
/// often arrive as strings (sometimes empty), and timestamps as ISO 8601 text without an offset (UTC) or as epoch seconds.
/// These options read all of those forms, map unrecognised enum names to the enum's default (<c>Unknown</c>) value, read
/// numbers and booleans into string properties as their text, and write <see cref="DateTimeOffset"/> values as UTC.
/// A JSON <c>null</c> for a collection property keeps the property's initial value (typically <c>[]</c>). Property names
/// not given by <see cref="JsonPropertyNameAttribute"/> are snake_case.
/// </remarks>
public static class ThreatStreamJson
{
	/// <summary>
	/// The options used by <see cref="ThreatStreamClient"/>. They are read-only: copy them
	/// (<c>new JsonSerializerOptions(ThreatStreamJson.Options)</c>) to customise.
	/// </summary>
	public static JsonSerializerOptions Options { get; } = Create();

	private static JsonSerializerOptions Create()
	{
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
			PropertyNameCaseInsensitive = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
			TypeInfoResolver = new DefaultJsonTypeInfoResolver
			{
				Modifiers = { CollectionDefaults.KeepOnNull }
			},
		};
		options.Converters.Add(new TolerantBooleanConverter());
		options.Converters.Add(new TolerantNullableBooleanConverter());
		options.Converters.Add(new TolerantStringConverter());
		options.Converters.Add(new TolerantInt32Converter());
		options.Converters.Add(new TolerantNullableInt32Converter());
		options.Converters.Add(new TolerantInt64Converter());
		options.Converters.Add(new TolerantNullableInt64Converter());
		options.Converters.Add(new TolerantDoubleConverter());
		options.Converters.Add(new TolerantNullableDoubleConverter());
		options.Converters.Add(new TolerantDateTimeOffsetConverter());
		options.Converters.Add(new TolerantEnumConverterFactory());
		// Shared and static: lock it so no caller can change serialization for every client.
		options.MakeReadOnly();
		return options;
	}
}
