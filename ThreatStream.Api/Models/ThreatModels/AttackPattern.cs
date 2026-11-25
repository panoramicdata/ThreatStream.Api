using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>An attack pattern: a way adversaries compromise targets, often from MITRE ATT&amp;CK (<c>api/v1/attackpattern/</c>).</summary>
/// <remarks><c>parent</c> is internal to ThreatStream and stays in <see cref="ThreatStreamObject.AdditionalProperties"/>.</remarks>
public sealed partial class AttackPattern : StixThreatModelEntity
{
	/// <summary>Other names the attack pattern is known by (<c>aliases</c>).</summary>
	[JsonPropertyName("aliases")]
	public IReadOnlyList<ThreatStreamReference> Aliases { get; init; } = [];

	/// <summary>The user the attack pattern is assigned to (<c>assignee_user</c>).</summary>
	[JsonPropertyName("assignee_user")]
	public ThreatStreamReference? AssigneeUser { get; init; }

	/// <summary>The body text (<c>body</c>), in <see cref="BodyContentType"/>.</summary>
	[JsonPropertyName("body")]
	public string? Body { get; init; }

	/// <summary>The format of the body text (<c>body_content_type</c>).</summary>
	[JsonPropertyName("body_content_type")]
	public ThreatModelBodyContentType? BodyContentType { get; init; }

	/// <summary>Whether the user can add public tags to the attack pattern (<c>can_add_public_tags</c>).</summary>
	[JsonPropertyName("can_add_public_tags")]
	public bool? CanAddPublicTags { get; init; }

	/// <summary>Not used by ThreatStream (<c>embedded_content_type</c>).</summary>
	[JsonPropertyName("embedded_content_type")]
	public string? EmbeddedContentType { get; init; }

	/// <summary>Not used by ThreatStream (<c>embedded_content_url</c>).</summary>
	[JsonPropertyName("embedded_content_url")]
	public string? EmbeddedContentUrl { get; init; }

	/// <summary>The references (sources) about the attack pattern (<c>external_references</c>).</summary>
	[JsonPropertyName("external_references")]
	public IReadOnlyList<ThreatStreamReference> ExternalReferences { get; init; } = [];

	/// <summary>Whether MITRE describes the attack pattern (<c>is_mitre</c>).</summary>
	[JsonPropertyName("is_mitre")]
	public bool? IsMitre { get; init; }

	/// <summary>
	/// The kill chain phases by model (<c>kill_chain_phases</c>), such as
	/// <c>{"lockheed-martin": ["delivery", "exploitation"]}</c>, as ThreatStream sends it.
	/// </summary>
	[JsonPropertyName("kill_chain_phases")]
	public JsonElement? KillChainPhases { get; init; }

	/// <summary>The Lockheed Martin kill chain stages, such as <c>delivery</c> (<c>killchainstages</c>).</summary>
	[JsonPropertyName("killchainstages")]
	public IReadOnlyList<ThreatStreamReference> KillChainStages { get; init; } = [];

	/// <summary>The users' votes on the attack pattern (<c>votes</c>), as ThreatStream sends them.</summary>
	[JsonPropertyName("votes")]
	public JsonElement? Votes { get; init; }
}
