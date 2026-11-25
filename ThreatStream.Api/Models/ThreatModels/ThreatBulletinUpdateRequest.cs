using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>Changes to a threat bulletin (<c>PATCH api/v1/tipreport/{id}/</c>); only the fields set are sent.</summary>
/// <remarks>
/// A threat bulletin's stage is <see cref="Status"/>, and its tags with their own TLP go in
/// <see cref="ThreatModelUpdateRequest.TagsV2"/>. The deprecated attributes <c>comments</c>, <c>history</c>,
/// <c>threat_actor</c>, <c>ttp</c> and <c>vulnerability</c> are not offered; associate threat model entities through the
/// threat model associations endpoints.
/// </remarks>
public class ThreatBulletinUpdateRequest : ThreatModelUpdateRequest
{
	/// <summary>The bulletin's full text, up to 100 MB, in the <see cref="BodyContentType"/> format (<c>body</c>).</summary>
	[JsonPropertyName("body")]
	public string? Body { get; init; }

	/// <summary>The format of <see cref="Body"/> (<c>body_content_type</c>); ThreatStream's default is markdown, and it cannot be changed once set.</summary>
	[JsonPropertyName("body_content_type")]
	public ThreatModelBodyContentType? BodyContentType { get; init; }

	/// <summary>The bulletin's stage (<c>status</c>); ThreatStream's default is <see cref="ThreatModelPublicationStatus.New"/>.</summary>
	[JsonPropertyName("status")]
	public ThreatModelPublicationStatus? Status { get; init; }

	/// <summary>The ID of the organisation to assign the bulletin to (<c>assignee_org_id</c>).</summary>
	[JsonPropertyName("assignee_org_id")]
	public long? AssigneeOrgId { get; init; }

	/// <summary>The ID of the user to assign the bulletin to (<c>assignee_user_id</c>).</summary>
	[JsonPropertyName("assignee_user_id")]
	public long? AssigneeUserId { get; init; }

	/// <summary>Whether to anonymise the owning user and organisation (<c>is_anonymous</c>).</summary>
	/// <remarks>The reference describes the value as <c>1</c> or <c>0</c>; this sends a JSON boolean, which is not verified live.</remarks>
	[JsonPropertyName("is_anonymous")]
	public bool? IsAnonymous { get; init; }

	/// <summary>The source of the intelligence, such as <c>USCERT</c> (<c>source</c>).</summary>
	[JsonPropertyName("source")]
	public string? Source { get; init; }

	/// <summary>The IDs of the import sessions (jobs) whose observables to associate (<c>import_sessions</c>).</summary>
	[JsonPropertyName("import_sessions")]
	public IReadOnlyList<long>? ImportSessions { get; init; }

	/// <summary>The IDs of the sandbox reports to associate (<c>sandbox_reports</c>).</summary>
	[JsonPropertyName("sandbox_reports")]
	public IReadOnlyList<long>? SandboxReports { get; init; }
}
