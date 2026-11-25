using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Sandbox;

/// <summary>The answer to a sandbox submission (<c>api/v1/submit/new/</c>).</summary>
public sealed class SandboxSubmission : ThreatStreamObject
{
	/// <summary>Whether the submission was accepted (<c>success</c>).</summary>
	[JsonPropertyName("success")]
	public bool? Success { get; init; }

	/// <summary>The detonation started on each platform, by platform name such as <c>WINDOWS7</c> (<c>reports</c>).</summary>
	[JsonPropertyName("reports")]
	public IReadOnlyDictionary<string, SandboxReportLinks> Reports { get; init; } = new Dictionary<string, SandboxReportLinks>();
}
