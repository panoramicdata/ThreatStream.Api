# workgroup endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/workgroup/` | `IWorkgroups.ListAsync` | `WorkgroupsTests.ListAsync_SendsGet` |
| POST | `/api/v2/intelligence/bulk_workgroups/` | `IObservableWorkgroups.RestrictAsync` | `ObservableWorkgroupsTests.RestrictAsync_PostsTheIdsAndWorkgroups` |
| PATCH | `/api/v1/intelligence/{observable_id} (deprecated)` |  |  |
| POST | `/api/v1/intelligence/import/ (deprecated)` |  |  |
