namespace ThreatStream.Api.Models.Sandbox;

/// <summary>
/// The report of a sandbox detonation (<c>api/v1/submit/{id}/report/</c>), whose content depends on the sandbox vendor.
/// </summary>
/// <remarks>
/// The reference shows this path only as the <c>detail</c> link of a submission and documents no attributes, so every
/// property is in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </remarks>
public sealed class SandboxReportDetail : ThreatStreamObject;
