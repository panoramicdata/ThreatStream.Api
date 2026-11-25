# enrichments endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/integration_package/direct/ip/{IP_address}/` | `IEnrichments.GetIpAsync` | `EnrichmentsTests.GetIpAsync_SendsGetWithTheServiceName` |
| GET | `/api/v1/pdns/{domain or ip}/{value}/` | `IPassiveDns.GetAsync` | `PassiveDnsTests.GetAsync_SendsGetForADomain` |
| GET | `/api/v1/integration_package/direct/` | `IEnrichments.LookupWithGetAsync` | `EnrichmentsTests.LookupWithGetAsync_SendsTheLookupInTheQuery` |
| POST | `/api/v1/integration_package/direct/` | `IEnrichments.LookupAsync` | `EnrichmentsTests.LookupAsync_PostsTheLookup` |
