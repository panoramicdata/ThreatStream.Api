# observable-tags endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| POST | `/api/v2/intelligence/bulk_tagging/` | `IObservableTags.BulkTagAsync`, `IObservableTags.BulkTagMatchingAsync` | `ObservableTagsTests.BulkTagAsync_PostsTheIdsAndTags`, `ObservableTagsTests.BulkTagMatchingAsync_PostsTheQueryAsynchronously` |
