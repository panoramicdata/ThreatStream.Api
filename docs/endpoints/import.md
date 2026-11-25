# import endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| POST | `/api/v2/intelligence/import/` |  |  |
| GET | `/api/v1/importsession/{import_session_id}/` |  |  |
| GET | `/api/v1/importsession/{import_session_id}/intelligence` |  |  |
| PATCH | `/api/v1/importsession/{import_session_id}/approve_all` |  |  |
| PATCH | `/api/v1/importsession/{import_session_id}/approve_selected/` |  |  |
| PATCH | `/api/v2/intelligence/` |  |  |
| POST | `/api/v1/stix_import/` |  |  |
