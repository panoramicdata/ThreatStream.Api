using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Rules;

public abstract partial class RuleRequest
{
	/// <summary>The IDs of the actors to associate matched entities with (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<long>? Actors { get; init; }

	/// <summary>The IDs of the campaigns to associate matched entities with (<c>campaigns</c>).</summary>
	[JsonPropertyName("campaigns")]
	public IReadOnlyList<long>? Campaigns { get; init; }

	/// <summary>The IDs of the incidents to associate matched entities with (<c>incidents</c>).</summary>
	[JsonPropertyName("incidents")]
	public IReadOnlyList<long>? Incidents { get; init; }

	/// <summary>The IDs of the malware to associate matched entities with (<c>malware</c>).</summary>
	[JsonPropertyName("malware")]
	public IReadOnlyList<long>? Malware { get; init; }

	/// <summary>The IDs of the signatures to associate matched entities with (<c>signatures</c>).</summary>
	[JsonPropertyName("signatures")]
	public IReadOnlyList<long>? Signatures { get; init; }

	/// <summary>The IDs of the threat bulletins to associate matched entities with (<c>tips</c>).</summary>
	[JsonPropertyName("tips")]
	public IReadOnlyList<long>? Tips { get; init; }

	/// <summary>The IDs of the TTPs to associate matched entities with (<c>ttps</c>).</summary>
	[JsonPropertyName("ttps")]
	public IReadOnlyList<long>? Ttps { get; init; }

	/// <summary>The IDs of the vulnerabilities to associate matched entities with (<c>vulnerabilities</c>).</summary>
	[JsonPropertyName("vulnerabilities")]
	public IReadOnlyList<long>? Vulnerabilities { get; init; }
}
