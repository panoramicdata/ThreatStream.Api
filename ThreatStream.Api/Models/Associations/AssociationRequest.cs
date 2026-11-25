using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Associations;

/// <summary>The body of <c>bulk_add/</c> and <c>bulk_remove/</c>: the entities or observables to associate or dissociate.</summary>
public sealed class AssociationRequest
{
	/// <summary>The IDs of the entities (or observables) of the associated entity type (<c>ids</c>).</summary>
	[JsonPropertyName("ids")]
	public required IReadOnlyList<long> Ids { get; init; }
}
