using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Associations between threat model entities and observables (<c>api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_add/</c> and <c>bulk_remove/</c>).</summary>
	public IAssociations Associations => field ??= For<IAssociations>();
}
