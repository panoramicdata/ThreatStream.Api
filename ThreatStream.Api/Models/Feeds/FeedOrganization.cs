using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Feeds;

/// <summary>The organisation that owns a <see cref="Feed"/>.</summary>
public sealed class FeedOrganization : ThreatStreamResource
{
	/// <summary>The organisation's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
