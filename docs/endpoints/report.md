# report endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/alfred/report/` | `IReports.GenerateAsync` | `ReportsTests.GenerateAsync_SendsGetWithTheResource` |
| GET | `/api/v1/alfred/async/{id}/` | `IReports.GetAsyncReportAsync` | `ReportsTests.GetAsyncReportAsync_SendsGetToTheJob` |
