# threat-model-tags endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| POST | `/api/v1/{t}/bulk_tagging/` | `IThreatModelTags.BulkTagAsync` | `ThreatModelTagsTests.BulkTagAsync_PostsTheIdsAndTags` |
