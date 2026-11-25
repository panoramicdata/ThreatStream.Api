using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The imports, observables and sandbox reports associated with a threat bulletin, each an ID or an object.</summary>
public sealed partial class ThreatBulletin
{
	/// <summary>
	/// Associated import sessions (<c>import_sessions</c>). Their observables may not also be in <see cref="Intelligence"/>,
	/// so read both for the complete set.
	/// </summary>
	[JsonPropertyName("import_sessions")]
	public IReadOnlyList<JsonElement> ImportSessions { get; init; } = [];

	/// <summary>Observables associated other than through an import (<c>intelligence</c>).</summary>
	[JsonPropertyName("intelligence")]
	public IReadOnlyList<JsonElement> Intelligence { get; init; } = [];

	/// <summary>Associated sandbox reports (<c>sandbox_reports</c>).</summary>
	[JsonPropertyName("sandbox_reports")]
	public IReadOnlyList<JsonElement> SandboxReports { get; init; } = [];
}
