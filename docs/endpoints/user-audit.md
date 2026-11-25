# user-audit endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/useraction/` | `IUserActions.StartReportAsync`, `IUserActions.GetReportAsync` | `UserActionsTests.StartReportAsync_SendsTheReferenceExample`, `UserActionsTests.GetReportAsync_SendsGetWithoutAsyncResult` |
| GET | `/api/v1/apiuseraction/` | `IApiUserActions.StartReportAsync`, `IApiUserActions.GetReportAsync` | `ApiUserActionsTests.StartReportAsync_SendsTheReferenceExample`, `ApiUserActionsTests.GetReportAsync_SendsGetWithoutAsyncResult` |
| GET | `/api/v1/generic_async_result/{id}/` | `IAsyncResults.GetAsync` | `AsyncResultsTests.GetAsync_SendsGetForTheJob` |
| GET | `/api/v1/generic_async_result/{id}/get_result/` | `IAsyncResults.GetResultAsync` | `AsyncResultsTests.GetResultAsync_SendsGetForTheResult` |
