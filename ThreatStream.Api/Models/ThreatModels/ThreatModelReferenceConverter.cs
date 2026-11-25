using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThreatStream.Api.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// Reads a <see cref="ThreatModelReference"/> from a bare ID, a name or an object, and writes it as an object.
/// </summary>
internal sealed class ThreatModelReferenceConverter : JsonConverter<ThreatModelReference>
{
	/// <inheritdoc />
	public override ThreatModelReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.Number => new ThreatModelReference { Id = TolerantReader.ParseInt64(TolerantReader.ReadNumberText(ref reader)!) },
			JsonTokenType.String => FromText(reader.GetString()!),
			JsonTokenType.StartObject => FromShape(JsonSerializer.Deserialize<Shape>(ref reader, options)!),
			_ => throw new JsonException($"Cannot read a threat model reference from a JSON {reader.TokenType}.")
		};

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, ThreatModelReference value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, new Shape
		{
			Id = value.Id,
			Name = value.Name,
			Value = value.Value,
			DisplayName = value.DisplayName,
			ResourceUri = value.ResourceUri,
			AdditionalProperties = value.AdditionalProperties
		}, options);

	private static ThreatModelReference FromText(string text)
		=> long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
			? new ThreatModelReference { Id = id }
			: new ThreatModelReference { Name = text };

	private static ThreatModelReference FromShape(Shape shape)
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
