namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A threat model entity of any type, as returned by the linked-entities endpoints
/// (<c>api/v1/{type}/{id}/{entity_type}/</c>). Only the attributes every type shares are modelled; the linked type's own
/// attributes stay in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </summary>
public sealed class ThreatModelAssociatedEntity : ThreatModelEntity
{
}
