using System.Collections;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Builds the <c>multipart/form-data</c> body of a <see cref="ThreatStreamMultipartRequest"/>. See that type for the
/// rules. Every part is a <see cref="ByteArrayContent"/>, so the whole body can be replayed.
/// </summary>
internal static class MultipartEncoder
{
	private static readonly ConcurrentDictionary<Type, FormProperty[]> PropertyCache = new();

	public static MultipartFormDataContent Encode(ThreatStreamMultipartRequest request)
	{
		var content = new MultipartFormDataContent();
		var names = new HashSet<string>(StringComparer.Ordinal);
		foreach (var property in PropertyCache.GetOrAdd(request.GetType(), DiscoverProperties))
		{
			if (property.Getter(request) is { } value)
			{
				names.Add(property.Name);
				content.Add(Part(property.Name, value));
			}
		}

		foreach (var (key, value) in request.AdditionalFields)
		{
			if (!names.Add(key))
			{
				throw new ArgumentException($"The additional field '{key}' is also set by a typed property; set it in one place only.", nameof(request));
			}

			content.Add(Text(key, value));
		}

		return content;
	}

	private static HttpContent Part(string name, object value)
	{
		if (value is ThreatStreamFile file)
		{
			var part = new ByteArrayContent(file.Content.ToArray());
			part.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
			part.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = Quote(name), FileName = Quote(file.FileName) };
			return part;
		}

		return Text(name, FormatValue(value));
	}

	/// <summary>The text of one non-file part.</summary>
	internal static string FormatValue(object value)
		=> value switch
		{
			string text => text,
			IEnumerable sequence when IsScalarSequence(sequence, out var items) => string.Join(',', items.Select(ThreatStreamFormatting.FormatScalar)),
			bool or Enum or DateTimeOffset or DateTime or IFormattable => ThreatStreamFormatting.FormatScalar(value),
			_ => JsonSerializer.Serialize(value, value.GetType(), ThreatStreamJson.Options)
		};

	/// <summary>Whether every item of <paramref name="sequence"/> is a string or a formattable scalar (an empty list is).</summary>
	private static bool IsScalarSequence(IEnumerable sequence, out List<object> items)
	{
		items = [.. sequence.Cast<object?>().Where(i => i is not null).Cast<object>()];
		return items.TrueForAll(i => i is string or bool or Enum or DateTimeOffset or DateTime or IFormattable);
	}

	private static StringContent Text(string name, string value)
	{
		var part = new StringContent(value, Encoding.UTF8);
		// No media type: a plain form field, as a browser sends it.
		part.Headers.ContentType = null;
		part.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = Quote(name) };
		return part;
	}

	private static string Quote(string value) => $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";

	private static FormProperty[] DiscoverProperties(Type type)
		=> [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.GetGetMethod() is not null
				&& p.GetIndexParameters().Length == 0
				&& p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
			.OrderBy(p => p.MetadataToken)
			.Select(p => new FormProperty(
				p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? JsonNamingPolicy.SnakeCaseLower.ConvertName(p.Name),
				p.GetValue))];

	private sealed record FormProperty(string Name, Func<object?, object?> Getter);
}
