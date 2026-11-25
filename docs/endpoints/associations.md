# associations endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| POST | `/api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_add/` | `IAssociations.BulkAddAsync` | `AssociationsTests.BulkAddAsync_PostsTheIds` |
| POST | `/api/v1/{entity_type}/{entity_id}/{associated_entity_type}/bulk_remove/` | `IAssociations.BulkRemoveAsync` | `AssociationsTests.BulkRemoveAsync_PostsTheIds` |
