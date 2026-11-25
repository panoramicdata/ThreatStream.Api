using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Threat actors (<c>api/v1/actor/</c>).</summary>
	public IActors Actors => field ??= For<IActors>();

	/// <summary>Attack patterns (<c>api/v1/attackpattern/</c>).</summary>
	public IAttackPatterns AttackPatterns => field ??= For<IAttackPatterns>();

	/// <summary>Campaigns (<c>api/v1/campaign/</c>).</summary>
	public ICampaigns Campaigns => field ??= For<ICampaigns>();

	/// <summary>Incidents (<c>api/v1/incident/</c>).</summary>
	public IIncidents Incidents => field ??= For<IIncidents>();

	/// <summary>The victim types threat model entities refer to (<c>api/v1/victimtype/</c>).</summary>
	public IVictimTypes VictimTypes => field ??= For<IVictimTypes>();

	/// <summary>The actor sophistication types (<c>api/v1/actorsophisticationtype/</c>).</summary>
	public IActorSophisticationTypes ActorSophisticationTypes => field ??= For<IActorSophisticationTypes>();

	/// <summary>The campaign statuses (<c>api/v1/campaignstatus/</c>).</summary>
	public ICampaignStatuses CampaignStatuses => field ??= For<ICampaignStatuses>();

	/// <summary>The incident statuses (<c>api/v1/incidentstatustype/</c>).</summary>
	public IIncidentStatusTypes IncidentStatusTypes => field ??= For<IIncidentStatusTypes>();
}
