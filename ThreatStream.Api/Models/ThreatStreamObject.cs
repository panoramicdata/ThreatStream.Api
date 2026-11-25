using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>
/// The base of response models: every JSON property the model does not declare is kept in
/// <see cref="AdditionalProperties"/>, so nothing ThreatStream sends is lost.
/// </summary>
public abstract class ThreatStreamObject
{
	/// <summary>The properties this model does not declare, by their JSON name.</summary>
	[JsonExtensionData]
	public IDictionary<string, JsonElement> AdditionalProperties { get; init; } = new Dictionary<string, JsonElement>();
}
