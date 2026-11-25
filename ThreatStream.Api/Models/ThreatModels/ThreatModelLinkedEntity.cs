namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// A threat model entity of any type linked to another (<c>GET api/v1/{type}/{id}/{entity_type}/</c>); the attributes of
/// its own type are kept in <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </summary>
public sealed class ThreatModelLinkedEntity : ThreatModelEntity;
