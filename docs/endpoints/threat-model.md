# threat-model endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/actor/` | `IActors.ListAsync` | `ActorsTests.ListAsync_SendsGet` |
| GET | `/api/v1/attackpattern/` | `IAttackPatterns.ListAsync` | `AttackPatternsTests.ListAsync_SendsGet` |
| GET | `/api/v1/campaign/` | `ICampaigns.ListAsync` | `CampaignsTests.ListAsync_SendsGet` |
| GET | `/api/v1/incident/` | `IIncidents.ListAsync` | `IncidentsTests.ListAsync_SendsGet` |
| GET | `/api/v1/malware/` |  |  |
| GET | `/api/v1/signature/` |  |  |
| GET | `/api/v1/tipreport/` |  |  |
| GET | `/api/v1/ttp/` |  |  |
| GET | `/api/v1/vulnerability/` |  |  |
| POST | `/api/v1/actor/` | `IActors.CreateAsync` | `ActorsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/attackpattern/` | `IAttackPatterns.CreateAsync` | `AttackPatternsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/campaign/` | `ICampaigns.CreateAsync` | `CampaignsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/incident/` | `IIncidents.CreateAsync` | `IncidentsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/malware/` |  |  |
| POST | `/api/v1/signature/` |  |  |
| POST | `/api/v1/tipreport/` |  |  |
| POST | `/api/v1/ttp/` |  |  |
| POST | `/api/v1/vulnerability/` |  |  |
| GET | `/api/v1/actor/{id}/` | `IActors.GetAsync` | `ActorsTests.GetAsync_SendsGet` |
| GET | `/api/v1/attackpattern/{id}/` | `IAttackPatterns.GetAsync` | `AttackPatternsTests.GetAsync_SendsGet` |
| GET | `/api/v1/campaign/{id}/` | `ICampaigns.GetAsync` | `CampaignsTests.GetAsync_SendsGet` |
| GET | `/api/v1/incident/{id}/` | `IIncidents.GetAsync` | `IncidentsTests.GetAsync_SendsGet` |
| GET | `/api/v1/malware/{id}/` |  |  |
| GET | `/api/v1/signature/{id}/` |  |  |
| GET | `/api/v1/tipreport/{id}/` |  |  |
| GET | `/api/v1/ttp/{id}/` |  |  |
| GET | `/api/v1/vulnerability/{id}/` |  |  |
| PATCH | `/api/v1/actor/{id}/` | `IActors.UpdateAsync` | `ActorsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/attackpattern/{id}/` | `IAttackPatterns.UpdateAsync` | `AttackPatternsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/campaign/{id}/` | `ICampaigns.UpdateAsync` | `CampaignsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/incident/{id}/` | `IIncidents.UpdateAsync` | `IncidentsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/malware/{id}/` |  |  |
| PATCH | `/api/v1/signature/{id}/` |  |  |
| PATCH | `/api/v1/tipreport/{id}/` |  |  |
| PATCH | `/api/v1/ttp/{id}/` |  |  |
| PATCH | `/api/v1/vulnerability/{id}/` |  |  |
| GET | `/api/v1/actor/associated_with_intelligence/` | `IActors.ListAssociatedWithValueAsync`, `IActors.ListAssociatedWithIdsAsync` | `ActorsTests.ListAssociatedWithValueAsync_SendsTheValue`, `ActorsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/attackpattern/associated_with_intelligence/` | `IAttackPatterns.ListAssociatedWithValueAsync`, `IAttackPatterns.ListAssociatedWithIdsAsync` | `AttackPatternsTests.ListAssociatedWithValueAsync_SendsTheValue`, `AttackPatternsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/campaign/associated_with_intelligence/` | `ICampaigns.ListAssociatedWithValueAsync`, `ICampaigns.ListAssociatedWithIdsAsync` | `CampaignsTests.ListAssociatedWithValueAsync_SendsTheValue`, `CampaignsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/incident/associated_with_intelligence/` | `IIncidents.ListAssociatedWithValueAsync`, `IIncidents.ListAssociatedWithIdsAsync` | `IncidentsTests.ListAssociatedWithValueAsync_SendsTheValue`, `IncidentsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/malware/associated_with_intelligence/` |  |  |
| GET | `/api/v1/signature/associated_with_intelligence/` |  |  |
| GET | `/api/v1/tipreport/associated_with_intelligence/` |  |  |
| GET | `/api/v1/ttp/associated_with_intelligence/` |  |  |
| GET | `/api/v1/vulnerability/associated_with_intelligence/` |  |  |
| GET | `/api/v1/actor/{id}/{entity_type}/` | `IActors.ListLinkedAsync` | `ActorsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/attackpattern/{id}/{entity_type}/` | `IAttackPatterns.ListLinkedAsync` | `AttackPatternsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/campaign/{id}/{entity_type}/` | `ICampaigns.ListLinkedAsync` | `CampaignsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/incident/{id}/{entity_type}/` | `IIncidents.ListLinkedAsync` | `IncidentsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/malware/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/signature/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/tipreport/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/ttp/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/vulnerability/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/actor/{id}/intelligence/` | `IActors.ListIntelligenceAsync` | `ActorsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/attackpattern/{id}/intelligence/` | `IAttackPatterns.ListIntelligenceAsync` | `AttackPatternsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/campaign/{id}/intelligence/` | `ICampaigns.ListIntelligenceAsync` | `CampaignsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/incident/{id}/intelligence/` | `IIncidents.ListIntelligenceAsync` | `IncidentsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/malware/{id}/intelligence/` |  |  |
| GET | `/api/v1/signature/{id}/intelligence/` |  |  |
| GET | `/api/v1/tipreport/{id}/intelligence/` |  |  |
| GET | `/api/v1/ttp/{id}/intelligence/` |  |  |
| GET | `/api/v1/vulnerability/{id}/intelligence/` |  |  |
| POST | `/api/v1/tipreport/{id}/attachment/` |  |  |
| GET | `/api/v1/victimtype/` | `IVictimTypes.ListAsync` | `VictimTypesTests.ListAsync_SendsGet` |
| GET | `/api/v1/actorsophisticationtype/` | `IActorSophisticationTypes.ListAsync` | `ActorSophisticationTypesTests.ListAsync_SendsGet` |
| GET | `/api/v1/campaignstatus/` | `ICampaignStatuses.ListAsync` | `CampaignStatusesTests.ListAsync_SendsGet` |
| GET | `/api/v1/incidentstatustype/` | `IIncidentStatusTypes.ListAsync` | `IncidentStatusTypesTests.ListAsync_SendsGet` |
| GET | `/api/v1/signature_type/` |  |  |
