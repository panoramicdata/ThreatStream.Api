using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ObservableImports;

public sealed partial class ObservableImportRequest
{
	/// <summary>Actors to associate the observables with (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }

	/// <summary>Campaigns to associate the observables with (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<long>? Campaigns { get; init; }

	/// <summary>Threat bulletins to associate the observables with (<c>tipreports</c>).</summary>
	[JsonPropertyName("tipreports")]
	public IReadOnlyList<long>? ThreatBulletins { get; init; }

	/// <summary>Incidents to associate the observables with (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<long>? Incidents { get; init; }

	/// <summary>Malware to associate the observables with (<c>malware</c>).</summary>
	[JsonPropertyName("malware")]
	public IReadOnlyList<long>? Malware { get; init; }

	/// <summary>TTPs to associate the observables with (<c>ttps</c>).</summary>
	[JsonPropertyName("ttps")]
	public IReadOnlyList<long>? Ttps { get; init; }

	/// <summary>Source regions, countries or US states, such as <c>europe</c> or <c>US-FL</c> (<c>source_locations</c>).</summary>
	[JsonPropertyName("source_locations")]
	public IReadOnlyList<string>? SourceLocations { get; init; }

	/// <summary>Targeted regions, countries or US states (<c>target_locations</c>).</summary>
	[JsonPropertyName("target_locations")]
	public IReadOnlyList<string>? TargetLocations { get; init; }

	/// <summary>Targeted industries from the STIX 2.1 vocabulary, such as <c>financial-services</c> (<c>target_industry</c>).</summary>
	[JsonPropertyName("target_industry")]
	public IReadOnlyList<string>? TargetIndustry { get; init; }

	/// <summary>The indicator type for domains without one, such as <c>mal_domain</c> (<c>domain_mapping</c>).</summary>
	[JsonPropertyName("domain_mapping")]
	public string? DomainMapping { get; init; }

	/// <summary>The indicator type for email addresses without one (<c>email_mapping</c>).</summary>
	[JsonPropertyName("email_mapping")]
	public string? EmailMapping { get; init; }

	/// <summary>The indicator type for IP addresses without one (<c>ip_mapping</c>).</summary>
	[JsonPropertyName("ip_mapping")]
	public string? IpMapping { get; init; }

	/// <summary>The indicator type for hashes without one (<c>md5_mapping</c>).</summary>
	[JsonPropertyName("md5_mapping")]
	public string? Md5Mapping { get; init; }

	/// <summary>The indicator type for URLs without one (<c>url_mapping</c>).</summary>
	[JsonPropertyName("url_mapping")]
	public string? UrlMapping { get; init; }
}
