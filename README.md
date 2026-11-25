# ThreatStream.Api

[![NuGet](https://img.shields.io/nuget/v/ThreatStream.Api.svg)](https://www.nuget.org/packages/ThreatStream.Api)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 11.0](https://img.shields.io/badge/.NET-11.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![CI](https://github.com/panoramicdata/ThreatStream.Api/actions/workflows/ci.yml/badge.svg)](https://github.com/panoramicdata/ThreatStream.Api/actions/workflows/ci.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0f466a662e1f45ecb45e619df03cbfce)](https://app.codacy.com/gh/panoramicdata/ThreatStream.Api/dashboard)
[![Codacy Coverage](https://app.codacy.com/project/badge/Coverage/0f466a662e1f45ecb45e619df03cbfce)](https://app.codacy.com/gh/panoramicdata/ThreatStream.Api/dashboard)

> **Need help with APIs or .NET?** Panoramic Data are experts in APIs and .NET, and we can help with your development. [Contact us](https://www.panoramicdata.com/contact-us) or see our [software services](https://www.panoramicdata.com/services/software).

A typed, modern .NET client for the [Anomali ThreatStream REST API](https://docs.anomali.com/Content/ThreatStream%20API/introduction_api.htm)
(v1 and v2), built on Refit and System.Text.Json.

Supports .NET 11.0.

ThreatStream.Api is an independent open-source project by Panoramic Data Limited. It is not made, endorsed or supported
by Anomali Inc. Anomali and ThreatStream are trademarks of Anomali Inc.

> **Live verification is pending.** Every operation is unit-tested against the ThreatStream API reference, but the
> integration tests have not yet been run against a live ThreatStream deployment. Please report anything that behaves
> differently.

## Installation

```shell
dotnet add package ThreatStream.Api
```

## Quick start

```csharp
using ThreatStream.Api;

using var client = new ThreatStreamClient(new ThreatStreamClientOptions
{
	Username = "analyst@example.com",   // the account's email address
	ApiKey = apiKey                     // from the My Account tab
});

var trend = await client.Confidence.GetTrendAsync("198.51.100.7", cancellationToken);
Console.WriteLine($"Confidence {trend.MinConfidence}-{trend.MaxConfidence}, average {trend.AverageConfidence}");
```

### Choosing the deployment

| Deployment | Options |
|---|---|
| US cloud (`https://api.threatstream.com/`) | `Region = ThreatStreamRegion.Us` (the default) |
| EU cloud (`https://api-eu.threatstream.com/`) | `Region = ThreatStreamRegion.Eu` |
| On-premises (OnPrem) | `Region = ThreatStreamRegion.Custom`, `BaseUrl = "https://threatstream.example.com/"` |

An on-premises deployment with a self-signed certificate can be trusted by pinning its SHA-256 thumbprint; other
certificates are still validated normally, and validation is never turned off:

```csharp
var options = new ThreatStreamClientOptions
{
	Region = ThreatStreamRegion.Custom,
	BaseUrl = "https://threatstream.example.com/",
	Username = "analyst@example.com",
	ApiKey = apiKey,
	TrustedServerCertificateThumbprint = "<SHA-256 thumbprint>"
};
```

`ServerCertificateValidationCallback` gives full control, and the `ThreatStreamClient(options, innerHandler)` constructor
accepts your own `HttpMessageHandler` (a proxy, or instrumentation).

## Authentication

| Options | How it authenticates |
|---|---|
| `Username` + `ApiKey` | `Authorization: apikey <username>:<api_key>` on every request. Recommended by Anomali. |
| `ClientId` + `ClientSecret` | OAuth 2.0 client credentials: a bearer token from `oauth/token`, renewed before it expires (15 minutes) and once more if ThreatStream answers 401. OAuth is a limited-availability feature; ask Anomali support to enable it. |

Credentials are never put in the URL, never logged, and masked by `ThreatStreamClientOptions.ToString()`.

## Paging

List operations return one page, `ThreatStreamPage<T>` (`Meta` and `Objects`). `EnumerateAsync` follows `meta.next`
through every page:

```csharp
await foreach (var item in client.EnumerateAsync(ct => client.Feeds.ListAsync(new() { Limit = 1000 }, ct), cancellationToken))
{
	Console.WriteLine(item.Name);
}
```

For more than 10,000 observables, ThreatStream recommends paging by `update_id`. `EnumerateByUpdateIdAsync` does that:
your delegate queries `update_id__gt` the value it is given, ordered by `update_id`.

## Read-only mode

`IsReadOnly = true` makes the client refuse, before anything is sent, every request that could change ThreatStream: any
method other than GET and HEAD, except POSTs to this allow-list of read-only operations:

| POST | Why it is read-only |
|---|---|
| `oauth/token` | Obtains an OAuth token. |
| `api/v2/intelligence/` | An observable query too long for a URL (over 4000 characters). |
| `api/v1/threat_model_search/` | A threat model search too long for a URL. |
| `api/v1/integration_package/direct/` | A Recorded Future enrichment lookup, documented as read-only. |

It raises `ThreatStreamReadOnlyException`. A few GETs still start server-side jobs (PDF reports, user audit reports
requested with `async_result`), so use a read-only ThreatStream user for a truly read-only identity.

## Errors, retries and rate limits

- A non-success response raises `ThreatStreamApiException` with the status code, ThreatStream's error text and, for a
  rate limit, `RetryAfter`.
- ThreatStream limits requests per user (150 a minute on the busiest endpoints, one a minute on any endpoint without a
  documented limit). 429 and 503 are retried for any verb, honouring `Retry-After`; other 5xx are retried for idempotent
  verbs; a connection that could not be established is retried too, since nothing was sent (`MaxRetries`,
  `RetryBaseDelay`, `MaxRetryDelay`). A `Retry-After` longer than `MaxRetryDelay` (60 seconds by default) raises the
  429 at once rather than waiting.
- `Timeout` applies per attempt and raises `TimeoutException`; cancelling your token raises `OperationCanceledException`.
- Pass an `ILogger` as `Logger` to see method, path and retry decisions; query strings are never logged.

## Coverage

`docs/endpoints/` lists every operation in the ThreatStream API reference, with the client method that implements it
and the test that pins its request. `InventoryTests` keeps that list and the code in step. Deprecated operations
(`/api/v1/intelligence/...`) are not implemented.

## Quality

- Every operation has a unit test pinning its exact HTTP method, path, query and body, and a test mapping the
  reference's example response. The library has 100% line and branch coverage.
- Integration tests are read-only by default; see [docs/TESTING.md](docs/TESTING.md).
- Zero compiler warnings, nullable reference types, XML documentation on every public member.

## Links

- NuGet: https://www.nuget.org/packages/ThreatStream.Api
- Source: https://github.com/panoramicdata/ThreatStream.Api
- Issues: https://github.com/panoramicdata/ThreatStream.Api/issues
- Contributing: [CONTRIBUTING.md](CONTRIBUTING.md), adding endpoints: [docs/IMPLEMENTING.md](docs/IMPLEMENTING.md)

## License

MIT. See [LICENSE](LICENSE).
