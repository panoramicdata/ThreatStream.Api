using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Reads a <see cref="ThreatStreamReference"/> from a bare ID, an API path, a name or an object, and writes it as an
/// object.
/// </summary>
internal sealed class ThreatStreamReferenceConverter : JsonConverter<ThreatStreamReference>
{
	/// <inheritdoc />
	public override ThreatStreamReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.Number => new ThreatStreamReference { Id = TolerantReader.ParseInt64(TolerantReader.ReadNumberText(ref reader)!) },
			JsonTokenType.String => FromText(reader.GetString()!),
			JsonTokenType.StartObject => FromShape(JsonSerializer.Deserialize<Shape>(ref reader, options)!),
			_ => throw new JsonException($"Cannot read a reference from a JSON {reader.TokenType}.")
		};

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, ThreatStreamReference value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, new Shape
		{
			Id = value.Id,
			Name = value.Name,
			Value = value.Value,
			DisplayName = value.DisplayName,
			ResourceUri = value.ResourceUri,
			AdditionalProperties = value.AdditionalProperties
		}, options);

	private static ThreatStreamReference FromText(string text)
	{
		if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
		{
			return new ThreatStreamReference { Id = id };
		}

		return text.StartsWith("/api/", StringComparison.Ordinal)
			? new ThreatStreamReference { ResourceUri = text }
			: new ThreatStreamReference { Name = text };
	}

	private static ThreatStreamReference FromShape(Shape shape)
		=> new()
		{
			Id = shape.Id,
			Name = shape.Name,
			Value = shape.Value,
			DisplayName = shape.DisplayName,
			ResourceUri = shape.ResourceUri,
			AdditionalProperties = shape.AdditionalProperties
		};

	/// <summary>The object form, read and written without this converter.</summary>
	private sealed class Shape
	{
		[JsonPropertyName("id")]
		public long? Id { get; init; }

		[JsonPropertyName("name")]
		public string? Name { get; init; }

		[JsonPropertyName("value")]
		public string? Value { get; init; }

		[JsonPropertyName("display_name")]
		public string? DisplayName { get; init; }

		[JsonPropertyName("resource_uri")]
		public string? ResourceUri { get; init; }

		[JsonExtensionData]
		public IDictionary<string, JsonElement> AdditionalProperties { get; init; } = new Dictionary<string, JsonElement>();
	}
}
