using System.Text.Json;
using System.Text.Json.Serialization;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Reads a list of tags, which ThreatStream sends as plain names (<c>["apt", "phishing"]</c>), as tag objects
/// (<c>[{"name": "apt", "tlp": "red"}]</c>) or as a mix of both, depending on the endpoint. Each name becomes a
/// <see cref="ThreatStreamTag"/> with only <see cref="ThreatStreamTag.Name"/> set; JSON nulls in the list are dropped.
/// Writes tag objects. Registered in <see cref="ThreatStreamJson.Options"/>, so it applies to every
/// <c>IReadOnlyList&lt;ThreatStreamTag&gt;</c> property.
/// </summary>
internal sealed class TolerantTagListConverter : JsonConverter<IReadOnlyList<ThreatStreamTag>>
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
	/// <remarks>Writes each tag in turn: serializing the list itself would come back to this converter.</remarks>
	public override void Write(Utf8JsonWriter writer, IReadOnlyList<ThreatStreamTag> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var tag in value)
		{
			JsonSerializer.Serialize(writer, tag, options);
		}

		writer.WriteEndArray();
	}
}
