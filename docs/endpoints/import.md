# import endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| POST | `/api/v2/intelligence/import/` | `IObservableImports.ImportAsync` | `ObservableImportsTests.ImportAsync_SendsMultipartWithEveryField` |
| GET | `/api/v1/importsession/{import_session_id}/` | `IImportSessions.GetAsync` | `ImportSessionsTests.GetAsync_SendsGetToTheSession` |
| GET | `/api/v1/importsession/{import_session_id}/intelligence` | `IImportSessions.ListIntelligenceAsync` | `ImportSessionsTests.ListIntelligenceAsync_SendsGetWithPaging` |
| PATCH | `/api/v1/importsession/{import_session_id}/approve_all` | `IImportSessions.ApproveAllAsync` | `ImportSessionsTests.ApproveAllAsync_SendsPatchWithoutABody` |
| PATCH | `/api/v1/importsession/{import_session_id}/approve_selected/` | `IImportSessions.ApproveSelectedAsync` | `ImportSessionsTests.ApproveSelectedAsync_SendsPatchWithTheIds` |
| PATCH | `/api/v2/intelligence/` | `IObservableImports.ImportWithoutApprovalAsync` | `ObservableImportsTests.ImportWithoutApprovalAsync_SendsPatchWithMetaAndObjects` |
| POST | `/api/v1/stix_import/` | `IStixImports.ImportAsync` | `StixImportsTests.ImportAsync_SendsMultipartWithTheFileAndSettings` |
