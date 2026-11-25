# intelligence endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v2/intelligence/` | `IObservables.ListAsync` | `ObservablesTests.ListAsync_SendsEveryFilter` |
| GET | `/api/v1/search_filter/` | `ISearchFilters.ListAsync` | `SearchFiltersTests.ListAsync_SendsGet` |
| POST | `/api/v2/intelligence/{observable_id}/clone/` | `IObservables.CloneAsync` | `ObservablesTests.CloneAsync_SendsPostToTheObservable` |
| GET | `/api/v2/intelligence/associated_with_intelligence/` | `IObservables.ListAssociatedWithIntelligenceAsync` | `ObservablesTests.ListAssociatedWithIntelligenceAsync_SendsTheValueAndIds` |
