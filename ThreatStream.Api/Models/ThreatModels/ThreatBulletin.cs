using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A threat bulletin (<c>api/v1/tipreport/</c>).</summary>
/// <remarks>
/// The deprecated attributes <c>comments</c>, <c>history</c>, <c>threat_actor</c>, <c>ttp</c> and <c>vulnerability</c>
/// (the last three always <see langword="null"/>) are not modelled; if sent, they are in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>. A threat bulletin's stage is <see cref="Status"/>, not
/// <see cref="ThreatModelEntity.PublicationStatus"/>, and its tags with their own TLP are in
/// <see cref="ThreatModelEntity.TagsV2"/>.
/// </remarks>
public sealed partial class ThreatBulletin : ThreatModelEntity
{
	/// <summary>The bulletin's stage, shown as Stage in the user interface (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public ThreatModelPublicationStatus? Status { get; init; }

	/// <summary>The ID of the organisation the bulletin is assigned to (<c>assignee_org_id</c>).</summary>
	[JsonPropertyName("assignee_org_id")]
	public long? AssigneeOrgId { get; init; }

	/// <summary>The ID of the user the bulletin is assigned to (<c>assignee_user_id</c>).</summary>
	[JsonPropertyName("assignee_user_id")]
	public long? AssigneeUserId { get; init; }

	/// <summary>The ID of the owning organisation (<c>owner_org_id</c>).</summary>
	[JsonPropertyName("owner_org_id")]
	public long? OwnerOrgId { get; init; }

	/// <summary>The source of the intelligence, such as <c>USCERT</c> (<c>source</c>).</summary>
	[JsonPropertyName("source")]
	public string? Source { get; init; }

	/// <summary>The bulletin's full text, up to 100 MB, in the <see cref="BodyContentType"/> format (<c>body</c>).</summary>
	[JsonPropertyName("body")]
	public string? Body { get; init; }

	/// <summary>The format of <see cref="Body"/> (<c>body_content_type</c>).</summary>
	[JsonPropertyName("body_content_type")]
	public ThreatModelBodyContentType? BodyContentType { get; init; }
}
