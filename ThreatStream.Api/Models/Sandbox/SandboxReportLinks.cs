using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Sandbox;

/// <summary>Where to follow one detonation of a <see cref="SandboxSubmission"/>.</summary>
public sealed class SandboxReportLinks : ThreatStreamObject
{
	/// <summary>The detonation's status path, such as <c>/api/v1/submit/188/</c> (<c>status</c>).</summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>The detonation's report path, such as <c>/api/v1/submit/188/report/</c> (<c>detail</c>).</summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	/// <summary>
	/// The detonation's ID, read from <see cref="Status"/> (the number after <c>submit/</c>), for
	/// <see cref="Interfaces.ISandboxSubmissions.GetAsync"/>; <see langword="null"/> when the path has none.
	/// </summary>
	[JsonIgnore]
	public long? Id
		=> Status?.Split('/', StringSplitOptions.RemoveEmptyEntries) is [.., "submit", var id] && long.TryParse(id, System.Globalization.CultureInfo.InvariantCulture, out var value)
			? value
			: null;
}
