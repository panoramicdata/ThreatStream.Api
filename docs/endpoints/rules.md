# rules endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/rule/` | `IRules.ListAsync` | `RulesTests.ListAsync_SendsGet` |
| POST | `/api/v1/rule/` | `IRules.CreateAsync` | `RulesTests.CreateAsync_PostsTheRule` |
| PATCH | `/api/v1/rule/{id}/` | `IRules.UpdateAsync` | `RulesTests.UpdateAsync_PatchesTheRule` |
| DELETE | `/api/v1/rule/` | `IRules.DeleteAsync` | `RulesTests.DeleteAsync_SendsDeleteWithAnIdFilter` |
