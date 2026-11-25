# whitelist endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/orgwhitelist/` | `IWhitelist.ListAsync`, `IWhitelist.ExportCsvAsync` | `WhitelistTests.ListAsync_SendsGetAsJson`, `WhitelistTests.ExportCsvAsync_SendsGetAsCsv` |
| POST | `/api/v1/orgwhitelist/bulk/` | `IWhitelist.AddAsync` | `WhitelistTests.AddAsync_PostsTheEntriesToBulk` |
| POST | `/api/v1/orgwhitelist/upload/` | `IWhitelist.UploadAsync` | `WhitelistTests.UploadAsync_PostsTheFileWithRemoveExistingInTheQuery` |
| PATCH | `/api/v1/orgwhitelist/{Entry_ID}/` | `IWhitelist.UpdateAsync` | `WhitelistTests.UpdateAsync_PatchesTheNote` |
| DELETE | `/api/v1/orgwhitelist/{Entry_ID}/` | `IWhitelist.DeleteAsync` | `WhitelistTests.DeleteAsync_DeletesTheEntry` |
