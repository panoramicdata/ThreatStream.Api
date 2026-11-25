# sandbox endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| POST | `/api/v1/submit/new/` | `ISandboxSubmissions.SubmitAsync` | `SandboxSubmissionsTests.SubmitAsync_SendsMultipartWithEveryField` |
| GET | `/api/v1/submit/search/` | `ISandboxSubmissions.SearchAsync` | `SandboxSubmissionsTests.SearchAsync_SendsGetWithTheKeyword` |
| GET | `/api/v1/submit/{id}/` | `ISandboxSubmissions.GetAsync` | `SandboxSubmissionsTests.GetAsync_SendsGetToTheDetonation` |
| GET | `/api/v1/submit/{id}/report/` | `ISandboxSubmissions.GetReportAsync` | `SandboxSubmissionsTests.GetReportAsync_SendsGetToTheReport` |
