using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>A tactic, technique or procedure (<c>api/v1/ttp/</c>). Its description is in <see cref="ThreatModelEntity.Description"/>.</summary>
public sealed class Ttp : ThreatModelEntity
{
	/// <summary>Associated actors (<c>actors</c>), each an ID or an object.</summary>
	[JsonPropertyName("actors")]
	public IReadOnlyList<JsonElement> Actors { get; init; } = [];

	/// <summary>Associated observables (<c>intelligence</c>), each an ID or an object.</summary>
	[JsonPropertyName("intelligence")]
	public IReadOnlyList<JsonElement> Intelligence { get; init; } = [];

	/// <summary>Associated signatures (<c>signatures</c>), each an ID or an object.</summary>
	[JsonPropertyName("signatures")]
	public IReadOnlyList<JsonElement> Signatures { get; init; } = [];

	/// <summary>On ThreatStream OnPrem, whether the entity is remote, held in the cloud (<c>remote_api</c>).</summary>
	[JsonPropertyName("remote_api")]
	public bool? RemoteApi { get; init; }
}
