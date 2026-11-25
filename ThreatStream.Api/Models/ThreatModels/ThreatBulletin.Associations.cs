using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The attachments, imports and sandbox reports of a threat bulletin.</summary>
public sealed partial class ThreatBulletin
{
	/// <summary>
	/// The files (white papers, reports, news) attached to the bulletin (<c>attachments</c>); visible to every organisation
	/// that can see the bulletin.
	/// </summary>
	[JsonPropertyName("attachments")]
	public IReadOnlyList<ThreatModelReference> Attachments { get; init; } = [];

	/// <summary>
	/// Associated import sessions (<c>import_sessions</c>). Their observables may not also be in
	/// <see cref="ThreatModelEntity.Intelligence"/>, so read both for the complete set.
	/// </summary>
	[JsonPropertyName("import_sessions")]
	public IReadOnlyList<ThreatModelReference> ImportSessions { get; init; } = [];

	/// <summary>Associated sandbox reports (<c>sandbox_reports</c>).</summary>
	[JsonPropertyName("sandbox_reports")]
	public IReadOnlyList<ThreatModelReference> SandboxReports { get; init; } = [];
}
