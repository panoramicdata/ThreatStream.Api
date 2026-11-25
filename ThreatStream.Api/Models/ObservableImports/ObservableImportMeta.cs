using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

/// <summary>The <c>meta</c> section of an import without approval: settings for every observable.</summary>
public sealed class ObservableImportMeta : ObservableImportSettings
{
	/// <summary>Whether to import domains that do not resolve, which are otherwise ignored (<c>allow_unresolved</c>).</summary>
	[JsonPropertyName("allow_unresolved")]
	public bool? AllowUnresolved { get; init; }

	/// <summary>Whether to skip the global whitelist checks of domains, IPs and emails (<c>ignore_whitelist</c>).</summary>
	[JsonPropertyName("ignore_whitelist")]
	public bool? IgnoreWhitelist { get; init; }

	/// <summary>Whether to skip the global whitelist checks of URL hosts (<c>ignore_url_host_whitelists</c>).</summary>
	[JsonPropertyName("ignore_url_host_whitelists")]
	public bool? IgnoreUrlHostWhitelists { get; init; }
}
