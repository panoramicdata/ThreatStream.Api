using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// Reads a threat model entity's <c>tags</c>, which ThreatStream documents as a list of plain names
/// (<c>["apt", "phishing"]</c>) but may send as tag objects (<c>[{"name": "apt", "tlp": "red"}]</c>). Each name becomes a
/// <see cref="ThreatStreamTag"/> with only <see cref="ThreatStreamTag.Name"/> set; JSON nulls in the list are dropped.
/// Writes tag objects.
/// </summary>
internal sealed class ThreatModelTagListConverter : JsonConverter<IReadOnlyList<ThreatStreamTag>>
{
	/// <inheritdoc />
	public override IReadOnlyList<ThreatStreamTag> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException($"Cannot read tags from a JSON {reader.TokenType}.");
		}

		var tags = new List<ThreatStreamTag>();
		// The serializer hands a converter the whole value, so the closing bracket is always there.
		for (reader.Read(); reader.TokenType != JsonTokenType.EndArray; reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.String:
					tags.Add(new ThreatStreamTag { Name = reader.GetString()! });
					break;
				case JsonTokenType.Null:
					break;
				default:
					tags.Add(JsonSerializer.Deserialize<ThreatStreamTag>(ref reader, options)!);
					break;
			}
		}

		return tags;
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, IReadOnlyList<ThreatStreamTag> value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, value, options);
}
