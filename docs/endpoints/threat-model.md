# threat-model endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/actor/` | `IActors.ListAsync` | `ActorsTests.ListAsync_SendsGet` |
| GET | `/api/v1/attackpattern/` | `IAttackPatterns.ListAsync` | `AttackPatternsTests.ListAsync_SendsGet` |
| GET | `/api/v1/campaign/` | `ICampaigns.ListAsync` | `CampaignsTests.ListAsync_SendsGet` |
| GET | `/api/v1/incident/` | `IIncidents.ListAsync` | `IncidentsTests.ListAsync_SendsGet` |
| GET | `/api/v1/malware/` | `IMalware.ListAsync` | `MalwareTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/signature/` | `ISignatures.ListAsync` | `SignaturesTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/tipreport/` | `IThreatBulletins.ListAsync` | `ThreatBulletinsTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/ttp/` | `ITtps.ListAsync` | `TtpsTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/vulnerability/` | `IVulnerabilities.ListAsync` | `VulnerabilitiesTests.ListAsync_SendsGetWithFilters` |
| POST | `/api/v1/actor/` | `IActors.CreateAsync` | `ActorsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/attackpattern/` | `IAttackPatterns.CreateAsync` | `AttackPatternsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/campaign/` | `ICampaigns.CreateAsync` | `CampaignsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/incident/` | `IIncidents.CreateAsync` | `IncidentsTests.CreateAsync_SendsPostWithJsonBody` |
| POST | `/api/v1/malware/` | `IMalware.CreateAsync` | `MalwareTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/signature/` | `ISignatures.CreateAsync` | `SignaturesTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/tipreport/` | `IThreatBulletins.CreateAsync` | `ThreatBulletinsTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/ttp/` | `ITtps.CreateAsync` | `TtpsTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/vulnerability/` | `IVulnerabilities.CreateAsync` | `VulnerabilitiesTests.CreateAsync_SendsTheDocumentedExample` |
| GET | `/api/v1/actor/{id}/` | `IActors.GetAsync` | `ActorsTests.GetAsync_SendsGet` |
| GET | `/api/v1/attackpattern/{id}/` | `IAttackPatterns.GetAsync` | `AttackPatternsTests.GetAsync_SendsGet` |
| GET | `/api/v1/campaign/{id}/` | `ICampaigns.GetAsync` | `CampaignsTests.GetAsync_SendsGet` |
| GET | `/api/v1/incident/{id}/` | `IIncidents.GetAsync` | `IncidentsTests.GetAsync_SendsGet` |
| GET | `/api/v1/malware/{id}/` | `IMalware.GetAsync` | `MalwareTests.GetAsync_SendsGet` |
| GET | `/api/v1/signature/{id}/` | `ISignatures.GetAsync` | `SignaturesTests.GetAsync_SendsGet` |
| GET | `/api/v1/tipreport/{id}/` | `IThreatBulletins.GetAsync` | `ThreatBulletinsTests.GetAsync_SendsGet` |
| GET | `/api/v1/ttp/{id}/` | `ITtps.GetAsync` | `TtpsTests.GetAsync_SendsGet` |
| GET | `/api/v1/vulnerability/{id}/` | `IVulnerabilities.GetAsync` | `VulnerabilitiesTests.GetAsync_SendsGet` |
| PATCH | `/api/v1/actor/{id}/` | `IActors.UpdateAsync` | `ActorsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/attackpattern/{id}/` | `IAttackPatterns.UpdateAsync` | `AttackPatternsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/campaign/{id}/` | `ICampaigns.UpdateAsync` | `CampaignsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/incident/{id}/` | `IIncidents.UpdateAsync` | `IncidentsTests.UpdateAsync_SendsPatchWithJsonBody` |
| PATCH | `/api/v1/malware/{id}/` | `IMalware.UpdateAsync` | `MalwareTests.UpdateAsync_SendsOnlyTheChangedFields`, `MalwareTests.UpdateAsync_SendsEveryField` |
| PATCH | `/api/v1/signature/{id}/` | `ISignatures.UpdateAsync` | `SignaturesTests.UpdateAsync_SendsOnlyTheChangedFields` |
| PATCH | `/api/v1/tipreport/{id}/` | `IThreatBulletins.UpdateAsync` | `ThreatBulletinsTests.UpdateAsync_SendsTheDocumentedExample`, `ThreatBulletinsTests.UpdateAsync_SendsEveryField` |
| PATCH | `/api/v1/ttp/{id}/` | `ITtps.UpdateAsync` | `TtpsTests.UpdateAsync_SendsEveryField` |
| PATCH | `/api/v1/vulnerability/{id}/` | `IVulnerabilities.UpdateAsync` | `VulnerabilitiesTests.UpdateAsync_SendsTheDocumentedExample` |
| GET | `/api/v1/actor/associated_with_intelligence/` | `IActors.ListAssociatedWithValueAsync`, `IActors.ListAssociatedWithIdsAsync` | `ActorsTests.ListAssociatedWithValueAsync_SendsTheValue`, `ActorsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/attackpattern/associated_with_intelligence/` | `IAttackPatterns.ListAssociatedWithValueAsync`, `IAttackPatterns.ListAssociatedWithIdsAsync` | `AttackPatternsTests.ListAssociatedWithValueAsync_SendsTheValue`, `AttackPatternsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/campaign/associated_with_intelligence/` | `ICampaigns.ListAssociatedWithValueAsync`, `ICampaigns.ListAssociatedWithIdsAsync` | `CampaignsTests.ListAssociatedWithValueAsync_SendsTheValue`, `CampaignsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/incident/associated_with_intelligence/` | `IIncidents.ListAssociatedWithValueAsync`, `IIncidents.ListAssociatedWithIdsAsync` | `IncidentsTests.ListAssociatedWithValueAsync_SendsTheValue`, `IncidentsTests.ListAssociatedWithIdsAsync_SendsTheIdsCommaSeparated` |
| GET | `/api/v1/malware/associated_with_intelligence/` | `IMalware.ListAssociatedWithValueAsync`, `IMalware.ListAssociatedWithIdsAsync` | `MalwareTests.ListAssociatedWithValueAsync_SendsTheValue`, `MalwareTests.ListAssociatedWithIdsAsync_SendsTheIds` |
| GET | `/api/v1/signature/associated_with_intelligence/` | `ISignatures.ListAssociatedWithValueAsync`, `ISignatures.ListAssociatedWithIdsAsync` | `SignaturesTests.ListAssociatedWithValueAsync_SendsTheValue`, `SignaturesTests.ListAssociatedWithIdsAsync_SendsTheIds` |
| GET | `/api/v1/tipreport/associated_with_intelligence/` | `IThreatBulletins.ListAssociatedWithValueAsync`, `IThreatBulletins.ListAssociatedWithIdsAsync` | `ThreatBulletinsTests.ListAssociatedWithValueAsync_SendsTheValue`, `ThreatBulletinsTests.ListAssociatedWithIdsAsync_SendsTheIds` |
| GET | `/api/v1/ttp/associated_with_intelligence/` | `ITtps.ListAssociatedWithValueAsync`, `ITtps.ListAssociatedWithIdsAsync` | `TtpsTests.ListAssociatedWithValueAsync_SendsTheValue`, `TtpsTests.ListAssociatedWithIdsAsync_SendsTheIds` |
| GET | `/api/v1/vulnerability/associated_with_intelligence/` | `IVulnerabilities.ListAssociatedWithValueAsync`, `IVulnerabilities.ListAssociatedWithIdsAsync` | `VulnerabilitiesTests.ListAssociatedWithValueAsync_SendsTheValue`, `VulnerabilitiesTests.ListAssociatedWithIdsAsync_SendsTheIds` |
| GET | `/api/v1/actor/{id}/{entity_type}/` | `IActors.ListLinkedAsync` | `ActorsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/attackpattern/{id}/{entity_type}/` | `IAttackPatterns.ListLinkedAsync` | `AttackPatternsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/campaign/{id}/{entity_type}/` | `ICampaigns.ListLinkedAsync` | `CampaignsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/incident/{id}/{entity_type}/` | `IIncidents.ListLinkedAsync` | `IncidentsTests.ListLinkedAsync_SendsTheEntityTypeInThePath` |
| GET | `/api/v1/malware/{id}/{entity_type}/` | `IMalware.ListLinkedAsync` | `MalwareTests.ListLinkedAsync_SendsTheEntityType` |
| GET | `/api/v1/signature/{id}/{entity_type}/` | `ISignatures.ListLinkedAsync` | `SignaturesTests.ListLinkedAsync_SendsTheEntityType` |
| GET | `/api/v1/tipreport/{id}/{entity_type}/` | `IThreatBulletins.ListLinkedAsync` | `ThreatBulletinsTests.ListLinkedAsync_SendsTheEntityType` |
| GET | `/api/v1/ttp/{id}/{entity_type}/` | `ITtps.ListLinkedAsync` | `TtpsTests.ListLinkedAsync_SendsTheEntityType` |
| GET | `/api/v1/vulnerability/{id}/{entity_type}/` | `IVulnerabilities.ListLinkedAsync` | `VulnerabilitiesTests.ListLinkedAsync_SendsTheEntityType` |
| GET | `/api/v1/actor/{id}/intelligence/` | `IActors.ListIntelligenceAsync` | `ActorsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/attackpattern/{id}/intelligence/` | `IAttackPatterns.ListIntelligenceAsync` | `AttackPatternsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/campaign/{id}/intelligence/` | `ICampaigns.ListIntelligenceAsync` | `CampaignsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/incident/{id}/intelligence/` | `IIncidents.ListIntelligenceAsync` | `IncidentsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/malware/{id}/intelligence/` | `IMalware.ListIntelligenceAsync` | `MalwareTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/signature/{id}/intelligence/` | `ISignatures.ListIntelligenceAsync` | `SignaturesTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/tipreport/{id}/intelligence/` | `IThreatBulletins.ListIntelligenceAsync` | `ThreatBulletinsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/ttp/{id}/intelligence/` | `ITtps.ListIntelligenceAsync` | `TtpsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/vulnerability/{id}/intelligence/` | `IVulnerabilities.ListIntelligenceAsync` | `VulnerabilitiesTests.ListIntelligenceAsync_SendsGet` |
| POST | `/api/v1/tipreport/{id}/attachment/` | `IThreatBulletins.AddAttachmentAsync` | `ThreatBulletinsTests.AddAttachmentAsync_SendsTheFileAndItsName` |
| GET | `/api/v1/victimtype/` | `IVictimTypes.ListAsync` | `VictimTypesTests.ListAsync_SendsGet` |
| GET | `/api/v1/actorsophisticationtype/` | `IActorSophisticationTypes.ListAsync` | `ActorSophisticationTypesTests.ListAsync_SendsGet` |
| GET | `/api/v1/campaignstatus/` | `ICampaignStatuses.ListAsync` | `CampaignStatusesTests.ListAsync_SendsGet` |
| GET | `/api/v1/incidentstatustype/` | `IIncidentStatusTypes.ListAsync` | `IncidentStatusTypesTests.ListAsync_SendsGet` |
| GET | `/api/v1/signature_type/` | `ISignatures.ListTypesAsync` | `SignaturesTests.ListTypesAsync_SendsGet` |
