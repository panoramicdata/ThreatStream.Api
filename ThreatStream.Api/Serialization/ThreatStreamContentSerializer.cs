using Refit;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ThreatStream.Api.Models;

namespace ThreatStream.Api.Serialization;

/// <summary>
/// Refit content serializer for ThreatStream: request bodies are written as JSON with
/// <see cref="ThreatStreamJson.Options"/>, or as <c>multipart/form-data</c> for a <see cref="ThreatStreamMultipartRequest"/>;
/// responses are read as JSON with the same options. Both kinds of body are buffered, so a request can be retried.
/// </summary>
internal sealed class ThreatStreamContentSerializer : IHttpContentSerializer, IReflectionFieldNameProvider
{
	private readonly SystemTextJsonContentSerializer _json = new(ThreatStreamJson.Options);

	/// <inheritdoc />
	public HttpContent ToHttpContent<T>(T item)
	{
		ArgumentNullException.ThrowIfNull(item);
		if (item is ThreatStreamMultipartRequest multipart)
		{
			return MultipartEncoder.Encode(multipart);
		}

		// StringContent is a ByteArrayContent: buffered, so it can be replayed on retry or with a renewed token.
		return new StringContent(
			JsonSerializer.Serialize(item, item.GetType(), ThreatStreamJson.Options),
			Encoding.UTF8,
			new MediaTypeHeaderValue("application/json"));
	}

	/// <inheritdoc />
	public Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
		=> _json.FromHttpContentAsync<T>(content, cancellationToken);

	/// <inheritdoc />
	public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
		=> propertyInfo.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()?.Name;
}
