# threat-model endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/actor/` |  |  |
| GET | `/api/v1/attackpattern/` |  |  |
| GET | `/api/v1/campaign/` |  |  |
| GET | `/api/v1/incident/` |  |  |
| GET | `/api/v1/malware/` | `IMalware.ListAsync` | `MalwareTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/signature/` | `ISignatures.ListAsync` | `SignaturesTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/tipreport/` | `IThreatBulletins.ListAsync` | `ThreatBulletinsTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/ttp/` | `ITtps.ListAsync` | `TtpsTests.ListAsync_SendsGetWithFilters` |
| GET | `/api/v1/vulnerability/` | `IVulnerabilities.ListAsync` | `VulnerabilitiesTests.ListAsync_SendsGetWithFilters` |
| POST | `/api/v1/actor/` |  |  |
| POST | `/api/v1/attackpattern/` |  |  |
| POST | `/api/v1/campaign/` |  |  |
| POST | `/api/v1/incident/` |  |  |
| POST | `/api/v1/malware/` | `IMalware.CreateAsync` | `MalwareTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/signature/` | `ISignatures.CreateAsync` | `SignaturesTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/tipreport/` | `IThreatBulletins.CreateAsync` | `ThreatBulletinsTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/ttp/` | `ITtps.CreateAsync` | `TtpsTests.CreateAsync_SendsTheDocumentedExample` |
| POST | `/api/v1/vulnerability/` | `IVulnerabilities.CreateAsync` | `VulnerabilitiesTests.CreateAsync_SendsTheDocumentedExample` |
| GET | `/api/v1/actor/{id}/` |  |  |
| GET | `/api/v1/attackpattern/{id}/` |  |  |
| GET | `/api/v1/campaign/{id}/` |  |  |
| GET | `/api/v1/incident/{id}/` |  |  |
| GET | `/api/v1/malware/{id}/` | `IMalware.GetAsync` | `MalwareTests.GetAsync_SendsGet` |
| GET | `/api/v1/signature/{id}/` | `ISignatures.GetAsync` | `SignaturesTests.GetAsync_SendsGet` |
| GET | `/api/v1/tipreport/{id}/` | `IThreatBulletins.GetAsync` | `ThreatBulletinsTests.GetAsync_SendsGet` |
| GET | `/api/v1/ttp/{id}/` | `ITtps.GetAsync` | `TtpsTests.GetAsync_SendsGet` |
| GET | `/api/v1/vulnerability/{id}/` | `IVulnerabilities.GetAsync` | `VulnerabilitiesTests.GetAsync_SendsGet` |
| PATCH | `/api/v1/actor/{id}/` |  |  |
| PATCH | `/api/v1/attackpattern/{id}/` |  |  |
| PATCH | `/api/v1/campaign/{id}/` |  |  |
| PATCH | `/api/v1/incident/{id}/` |  |  |
| PATCH | `/api/v1/malware/{id}/` | `IMalware.UpdateAsync` | `MalwareTests.UpdateAsync_SendsOnlyTheChangedFields`, `MalwareTests.UpdateAsync_SendsEveryField` |
| PATCH | `/api/v1/signature/{id}/` | `ISignatures.UpdateAsync` | `SignaturesTests.UpdateAsync_SendsOnlyTheChangedFields` |
| PATCH | `/api/v1/tipreport/{id}/` | `IThreatBulletins.UpdateAsync` | `ThreatBulletinsTests.UpdateAsync_SendsTheDocumentedExample`, `ThreatBulletinsTests.UpdateAsync_SendsEveryField` |
| PATCH | `/api/v1/ttp/{id}/` | `ITtps.UpdateAsync` | `TtpsTests.UpdateAsync_SendsEveryField` |
| PATCH | `/api/v1/vulnerability/{id}/` | `IVulnerabilities.UpdateAsync` | `VulnerabilitiesTests.UpdateAsync_SendsTheDocumentedExample` |
| GET | `/api/v1/actor/associated_with_intelligence/` |  |  |
| GET | `/api/v1/attackpattern/associated_with_intelligence/` |  |  |
| GET | `/api/v1/campaign/associated_with_intelligence/` |  |  |
| GET | `/api/v1/incident/associated_with_intelligence/` |  |  |
| GET | `/api/v1/malware/associated_with_intelligence/` | `IMalware.ListAssociatedWithIntelligenceAsync`, `IMalware.ListAssociatedWithIntelligenceByIdsAsync` | `MalwareTests.ListAssociatedWithIntelligenceAsync_SendsTheValue`, `MalwareTests.ListAssociatedWithIntelligenceByIdsAsync_SendsTheIds` |
| GET | `/api/v1/signature/associated_with_intelligence/` | `ISignatures.ListAssociatedWithIntelligenceAsync`, `ISignatures.ListAssociatedWithIntelligenceByIdsAsync` | `SignaturesTests.ListAssociatedWithIntelligenceAsync_SendsTheValue`, `SignaturesTests.ListAssociatedWithIntelligenceByIdsAsync_SendsTheIds` |
| GET | `/api/v1/tipreport/associated_with_intelligence/` | `IThreatBulletins.ListAssociatedWithIntelligenceAsync`, `IThreatBulletins.ListAssociatedWithIntelligenceByIdsAsync` | `ThreatBulletinsTests.ListAssociatedWithIntelligenceAsync_SendsTheValue`, `ThreatBulletinsTests.ListAssociatedWithIntelligenceByIdsAsync_SendsTheIds` |
| GET | `/api/v1/ttp/associated_with_intelligence/` | `ITtps.ListAssociatedWithIntelligenceAsync`, `ITtps.ListAssociatedWithIntelligenceByIdsAsync` | `TtpsTests.ListAssociatedWithIntelligenceAsync_SendsTheValue`, `TtpsTests.ListAssociatedWithIntelligenceByIdsAsync_SendsTheIds` |
| GET | `/api/v1/vulnerability/associated_with_intelligence/` | `IVulnerabilities.ListAssociatedWithIntelligenceAsync`, `IVulnerabilities.ListAssociatedWithIntelligenceByIdsAsync` | `VulnerabilitiesTests.ListAssociatedWithIntelligenceAsync_SendsTheValue`, `VulnerabilitiesTests.ListAssociatedWithIntelligenceByIdsAsync_SendsTheIds` |
| GET | `/api/v1/actor/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/attackpattern/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/campaign/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/incident/{id}/{entity_type}/` |  |  |
| GET | `/api/v1/malware/{id}/{entity_type}/` | `IMalware.ListLinkedEntitiesAsync` | `MalwareTests.ListLinkedEntitiesAsync_SendsTheEntityType` |
| GET | `/api/v1/signature/{id}/{entity_type}/` | `ISignatures.ListLinkedEntitiesAsync` | `SignaturesTests.ListLinkedEntitiesAsync_SendsTheEntityType` |
| GET | `/api/v1/tipreport/{id}/{entity_type}/` | `IThreatBulletins.ListLinkedEntitiesAsync` | `ThreatBulletinsTests.ListLinkedEntitiesAsync_SendsTheEntityType` |
| GET | `/api/v1/ttp/{id}/{entity_type}/` | `ITtps.ListLinkedEntitiesAsync` | `TtpsTests.ListLinkedEntitiesAsync_SendsTheEntityType` |
| GET | `/api/v1/vulnerability/{id}/{entity_type}/` | `IVulnerabilities.ListLinkedEntitiesAsync` | `VulnerabilitiesTests.ListLinkedEntitiesAsync_SendsTheEntityType` |
| GET | `/api/v1/actor/{id}/intelligence/` |  |  |
| GET | `/api/v1/attackpattern/{id}/intelligence/` |  |  |
| GET | `/api/v1/campaign/{id}/intelligence/` |  |  |
| GET | `/api/v1/incident/{id}/intelligence/` |  |  |
| GET | `/api/v1/malware/{id}/intelligence/` | `IMalware.ListIntelligenceAsync` | `MalwareTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/signature/{id}/intelligence/` | `ISignatures.ListIntelligenceAsync` | `SignaturesTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/tipreport/{id}/intelligence/` | `IThreatBulletins.ListIntelligenceAsync` | `ThreatBulletinsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/ttp/{id}/intelligence/` | `ITtps.ListIntelligenceAsync` | `TtpsTests.ListIntelligenceAsync_SendsGet` |
| GET | `/api/v1/vulnerability/{id}/intelligence/` | `IVulnerabilities.ListIntelligenceAsync` | `VulnerabilitiesTests.ListIntelligenceAsync_SendsGet` |
| POST | `/api/v1/tipreport/{id}/attachment/` | `IThreatBulletins.AddAttachmentAsync` | `ThreatBulletinsTests.AddAttachmentAsync_SendsTheFileAndItsName` |
| GET | `/api/v1/victimtype/` |  |  |
| GET | `/api/v1/actorsophisticationtype/` |  |  |
| GET | `/api/v1/campaignstatus/` |  |  |
| GET | `/api/v1/incidentstatustype/` |  |  |
| GET | `/api/v1/signature_type/` | `ISignatures.ListTypesAsync` | `SignaturesTests.ListTypesAsync_SendsGet` |
