using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>
/// The answer to a request that starts an import (<c>202 Accepted</c>): an import with approval, or the clone of an
/// observable. Poll the import session with <see cref="Interfaces.IImportSessions.GetAsync"/>, and approve it with
/// <see cref="Interfaces.IImportSessions.ApproveAllAsync"/> unless it was auto-approved.
/// </summary>
public sealed class ImportJob : ThreatStreamObject
{
	/// <summary>The import session the request created (<c>import_session_id</c>; ThreatStream sends it as text).</summary>
	[JsonPropertyName("import_session_id")]
	public long? ImportSessionId { get; init; }

	/// <summary>The background job's identifier, a UUID (<c>job_id</c>).</summary>
	[JsonPropertyName("job_id")]
	public string? JobId { get; init; }

	/// <summary>Whether the request was accepted (<c>success</c>); not sent for a clone.</summary>
	[JsonPropertyName("success")]
	public bool? Success { get; init; }
}
