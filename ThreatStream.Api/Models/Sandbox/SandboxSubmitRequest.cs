using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Sandbox;

/// <summary>
/// A URL or file to detonate in a sandbox (<c>POST api/v1/submit/new/</c>, sent as <c>multipart/form-data</c>). Set
/// <see cref="Url"/> or <see cref="File"/>, and choose a sandbox with one of the <c>Use...Sandbox</c> flags.
/// </summary>
/// <remarks>
/// Files in an archive are detonated one by one; tag the submission with <see cref="Notes"/> to find them all later with
/// <see cref="Interfaces.ISandboxSubmissions.SearchAsync"/>.
/// </remarks>
public sealed class SandboxSubmitRequest : ThreatStreamMultipartRequest
{
	/// <summary>The URL to detonate, at most 2,000 characters (<c>url</c>).</summary>
	[JsonPropertyName("url")]
	public string? Url { get; init; }

	/// <summary>The file to detonate (<c>file</c>).</summary>
	[JsonPropertyName("file")]
	public ThreatStreamFile? File { get; init; }

	/// <summary>
	/// The platform to detonate on, such as <c>WINDOWS7</c>, <c>WINDOWS10x64</c>, <c>MACOSXVM</c>, <c>LINUX</c> or
	/// <c>ANDROID9.0</c> (<c>platform</c>). Required for Joe Sandbox; leave it unset for VMRay.
	/// </summary>
	[JsonPropertyName("platform")]
	public string? Platform { get; init; }

	/// <summary>The visibility of the submission: <c>public</c> or <c>private</c> (<c>classification</c>).</summary>
	[JsonPropertyName("classification")]
	public string? Classification { get; init; }

	/// <summary>Whether to use the Anomali Sandbox, which allows two free detonations a day (<c>use_anomali_sandbox</c>).</summary>
	[JsonPropertyName("use_anomali_sandbox")]
	public bool? UseAnomaliSandbox { get; init; }

	/// <summary>Whether to use Joe Sandbox, which needs an active integration (<c>use_premium_sandbox</c>).</summary>
	[JsonPropertyName("use_premium_sandbox")]
	public bool? UsePremiumSandbox { get; init; }

	/// <summary>Whether to use Polyswarm, which needs an active integration (<c>use_polyswarm_sandbox</c>).</summary>
	[JsonPropertyName("use_polyswarm_sandbox")]
	public bool? UsePolyswarmSandbox { get; init; }

	/// <summary>Whether to use VMRay, which needs an active integration (<c>use_vmray_sandbox</c>).</summary>
	[JsonPropertyName("use_vmray_sandbox")]
	public bool? UseVmraySandbox { get; init; }

	/// <summary>With VMRay, how many detonations to run, each on a different platform (<c>vmray_max_jobs</c>).</summary>
	[JsonPropertyName("vmray_max_jobs")]
	public int? VmrayMaxJobs { get; init; }

	/// <summary>Polyswarm only: whether to scan the URL or file (<c>polyswarm_scan</c>).</summary>
	[JsonPropertyName("polyswarm_scan")]
	public bool? PolyswarmScan { get; init; }

	/// <summary>Polyswarm only: whether to analyse the URLs in the file's QR codes (<c>is_qrcode</c>).</summary>
	[JsonPropertyName("is_qrcode")]
	public bool? IsQrCode { get; init; }

	/// <summary>Whether the file is password-protected; set <see cref="FilePassword"/> too (<c>file_has_password</c>).</summary>
	[JsonPropertyName("file_has_password")]
	public bool? FileHasPassword { get; init; }

	/// <summary>The file's password (<c>file_password</c>).</summary>
	[JsonPropertyName("file_password")]
	public string? FilePassword { get; init; }

	/// <summary>Whether to start an import of the observables the detonation finds; <see langword="true"/> by default (<c>import_indicators</c>).</summary>
	[JsonPropertyName("import_indicators")]
	public bool? ImportIndicators { get; init; }

	/// <summary>Tags for the imported observables, sent comma-separated (<c>notes</c>).</summary>
	[JsonPropertyName("notes")]
	public IReadOnlyList<string>? Notes { get; init; }

	/// <summary>The trusted circles to associate the results with, sent comma-separated (<c>circles</c>).</summary>
	[JsonPropertyName("circles")]
	public IReadOnlyList<long>? Circles { get; init; }
}
