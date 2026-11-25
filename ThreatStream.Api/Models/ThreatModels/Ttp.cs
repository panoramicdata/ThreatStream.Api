using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A tactic, technique or procedure (<c>api/v1/ttp/</c>). Its description, associated observables and signatures are in
/// <see cref="ThreatModelEntity"/>.
/// </summary>
public sealed class Ttp : ThreatModelEntity
{
	/// <summary>Associated actors (<c>actors</c>).</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<ThreatStreamReference> Actors { get; init; } = [];
}
