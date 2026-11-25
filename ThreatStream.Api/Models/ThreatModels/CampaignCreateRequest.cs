using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The body of a new campaign (<c>POST api/v1/campaign/</c>).</summary>
public sealed class CampaignCreateRequest : CampaignUpdateRequest
{
	/// <summary>The campaign's name, unique within the organisation, up to 255 characters (<c>name</c>). Required.</summary>
	[JsonPropertyName("name")]
	public new required string Name { get; init; }
}
