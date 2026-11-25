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

> **Not verified against a live ThreatStream.** There is no public ThreatStream sandbox, and none of the maintainers has
> had access to a deployment, so no operation has been run against a real ThreatStream. See
> [Verification status](#verification-status) before relying on this package in production.

## Installation

```shell
dotnet add package ThreatStream.Api
```

The package version line is 1.0. It implements the ThreatStream API reference as published at
https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm.

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

Every group of operations is a property of `ThreatStreamClient` that returns a Refit interface (see
[Coverage](#coverage)). The client owns its `HttpClient`; dispose it when you are done.

### Choosing the deployment

| Deployment | Options |
|---|---|
| US cloud (`https://api.threatstream.com/`) | `Region = ThreatStreamRegion.Us` (the default) |
| EU cloud (`https://api-eu.threatstream.com/`) | `Region = ThreatStreamRegion.Eu` |
| On-premises (OnPrem) or any other URL | `Region = ThreatStreamRegion.Custom`, `BaseUrl = "https://threatstream.example.com/"` |

```csharp
// EU cloud
var eu = new ThreatStreamClientOptions { Region = ThreatStreamRegion.Eu, Username = user, ApiKey = apiKey };

// Custom base URL (a path prefix is kept; endpoint paths are appended to it)
var custom = new ThreatStreamClientOptions
{
	Region = ThreatStreamRegion.Custom,
	BaseUrl = "https://threatstream.example.com/",
	Username = user,
	ApiKey = apiKey
};
```

`BaseUrl` is only valid with `Region = Custom`, must be an absolute http or https URL, and must not contain
credentials, a query string or a fragment.

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

`ServerCertificateValidationCallback` gives full control. Both are ignored when you use the
`ThreatStreamClient(options, innerHandler)` constructor, which accepts your own `HttpMessageHandler` (a proxy, or
instrumentation).

## Authentication

Set exactly one of the two pairs. Setting both, or half of a pair, throws `ArgumentException` when the client is created.

| Options | How it authenticates |
|---|---|
| `Username` + `ApiKey` | `Authorization: apikey <username>:<api_key>` on every request. Recommended by Anomali. |
| `ClientId` + `ClientSecret` | OAuth 2.0 client credentials: a bearer token from `oauth/token`, renewed before it expires (15 minutes) and once more if ThreatStream answers 401. OAuth is a limited-availability feature; ask Anomali support to enable it. |

```csharp
// API key
using var apiKeyClient = new ThreatStreamClient(new ThreatStreamClientOptions
{
	Username = "analyst@example.com",
	ApiKey = apiKey
});

// OAuth 2.0 client credentials
using var oauthClient = new ThreatStreamClient(new ThreatStreamClientOptions
{
	ClientId = clientId,
	ClientSecret = clientSecret
});
```

Credentials are never put in the URL, never logged, and masked by `ThreatStreamClientOptions.ToString()`.

## Paging

List operations return one page, `ThreatStreamPage<T>` (`Meta` and `Objects`). Three helpers walk the pages.

`ThreatStreamClient.EnumerateAsync<T>` follows `meta.next` through every page of any list operation. Its options
(filters, `Limit`, `OrderBy`) carry over, since ThreatStream's next links repeat them:

```csharp
// IAsyncEnumerable<T> EnumerateAsync<T>(Func<CancellationToken, Task<ThreatStreamPage<T>>> firstPage, CancellationToken cancellationToken)
await foreach (var feed in client.EnumerateAsync(ct => client.Feeds.ListAsync(new FeedListOptions { Limit = 1000 }, ct), cancellationToken))
{
	Console.WriteLine(feed.Name);
}
```

`EnumerateObservablesAsync` is the same for `api/v2/intelligence/`, up to about 10,000 results:

```csharp
// IAsyncEnumerable<Observable> EnumerateObservablesAsync(ObservableListOptions? options, CancellationToken cancellationToken)
await foreach (var observable in client.EnumerateObservablesAsync(new ObservableListOptions { Type = "ip" }, cancellationToken))
{
	Console.WriteLine(observable.Value);
}
```

For more than 10,000 observables, ThreatStream recommends paging by `update_id`, because offset paging becomes slow and
is capped:

```csharp
// IAsyncEnumerable<Observable> EnumerateObservablesByUpdateIdAsync(ObservableListOptions? options, long afterUpdateId, CancellationToken cancellationToken)
await foreach (var observable in client.EnumerateObservablesByUpdateIdAsync(null, afterUpdateId: 0, cancellationToken))
{
	// results arrive in update_id order
}
```

`ThreatStreamClient.EnumerateByUpdateIdAsync<T>(getPage, updateIdOf, afterUpdateId, cancellationToken)` is the static,
general form for any list that has an `update_id`: your delegate queries `update_id__gt` the value it is given, ordered
by `update_id`. For a continuous sync also set `ModifiedBefore` to a minute ago.

Safeguards: a `meta.next` link to another scheme, host or port is refused (`InvalidOperationException`), so credentials
are never sent elsewhere; so is a next link that repeats, and an `update_id` page that does not advance.

## Read-only mode

`IsReadOnly = true` makes the client refuse, before anything is sent, every request that could change ThreatStream: any
method other than GET and HEAD, except POSTs to this allow-list of read-only operations (matched exactly, trailing
slash optional, POST only):

| POST | Why it is read-only |
|---|---|
| `oauth/token` | Obtains an OAuth token. |
| `api/v2/intelligence/` | An observable query too long for a URL (over 4000 characters). |
| `api/v1/threat_model_search/` | A threat model search too long for a URL. |
| `api/v1/integration_package/direct/` | A Recorded Future enrichment lookup, documented as read-only. |

The refusal raises `ThreatStreamReadOnlyException` (`Method` and `Path`). `PATCH api/v2/intelligence/` and every
sub-path (`import/`, `bulk_tagging/`, `{id}/clone/`) are refused.

GETs that start a server-side job are refused as well: generating a PDF report (`api/v1/alfred/report/`, which
`IReports.GenerateAsync` calls) and any request with `async_result=true` (the user activity reports started by
`IUserActions.StartReportAsync` and `IApiUserActions.StartReportAsync`). For a truly read-only identity, also use a
read-only ThreatStream user.

## Files, non-JSON responses and asynchronous jobs

**Multipart uploads.** Operations that take files are sent as `multipart/form-data`: observable and STIX imports,
sandbox submissions, whitelist uploads and threat bulletin attachments. Their request models derive from
`ThreatStreamMultipartRequest`; each property becomes one form part named by its JSON name, and null properties are
omitted. A file is a `ThreatStreamFile(FileName, Content, ContentType)`, kept in memory so the request can be retried.

```csharp
var submission = await client.SandboxSubmissions.SubmitAsync(new SandboxSubmitRequest
{
	Url = "https://example.com/suspicious",
	UseAnomaliSandbox = true
}, cancellationToken);
```

**Non-JSON responses.** Where ThreatStream answers with text, the method returns `Task<string>`:
`IWhitelist.ExportCsvAsync`, `IUserActions.GetReportAsync`, `IApiUserActions.GetReportAsync` and
`IAsyncResults.GetResultAsync`. Other responses are read tolerantly: unknown properties are kept in
`ThreatStreamObject.AdditionalProperties` rather than lost.

**Asynchronous jobs.** `RunAsyncJobAsync` starts a job, polls `api/v1/generic_async_result/{id}/` until it is `done`,
then downloads the result as text:

```csharp
// Task<string> RunAsyncJobAsync(Func<CancellationToken, Task<GenericAsyncResult>> start, TimeSpan pollInterval, CancellationToken cancellationToken)
var csvOrJson = await client.RunAsyncJobAsync(
	ct => client.UserActions.StartReportAsync(options, ct),
	pollInterval: TimeSpan.FromMinutes(1),   // the job endpoint is limited to one request a minute by default
	cancellationToken);
```

A job whose status is `failed` or `error`, or whose response status is 400 or higher, raises
`InvalidOperationException`. Give the token a timeout to bound how long you will wait.

## Errors, retries and rate limits

- A non-success response raises `ThreatStreamApiException` with `StatusCode`, ThreatStream's error text and, for a rate
  limit, `RetryAfter`.
- ThreatStream limits requests per user: 150 a minute on the busiest endpoints, and one request a minute per user on any
  endpoint without a documented limit (`Confidence`, the report and asynchronous job endpoints, and others). Expect
  `429` if you call the latter faster, and keep live use of them sparse.
- 429 and 503 are retried for any verb, honouring `Retry-After`. Other 5xx responses are retried only for idempotent
  verbs (GET, HEAD, PUT, DELETE), never POST or PATCH. A connection that could not be established is retried too, since
  nothing was sent. Requests with a stream body are never retried. Tune with `MaxRetries` (3), `RetryBaseDelay` (1
  second, doubled each retry) and `MaxRetryDelay` (60 seconds).
- A `Retry-After` longer than `MaxRetryDelay` is not waited out: the `ThreatStreamApiException` is raised at once, with
  `RetryAfter` set. The 60 second default covers ThreatStream's one-request-a-minute limit.
- `Timeout` (100 seconds by default) applies per attempt and raises `TimeoutException`; cancelling your token raises
  `OperationCanceledException`.
- Pass an `ILogger` as `Logger` to see method, path and retry decisions. Credentials, tokens and query strings (which
  can carry observable values) are never logged.

## Coverage

134 operations of the ThreatStream API reference are implemented. `docs/endpoints/` lists every one with its path, the
client method and the test that pins its request, and `InventoryTests` keeps those lists and the code in step. Two
deprecated operations (`PATCH /api/v1/intelligence/{observable_id}` and `POST /api/v1/intelligence/import/`) are not
implemented; use the v2 equivalents.

An operation is one HTTP method and path. A few take two client methods that differ in query parameters (for example
`IWhitelist.ListAsync` and `ExportCsvAsync`, or the `ListAssociatedWithValueAsync` and `ListAssociatedWithIdsAsync` pair
on each threat model type), so the method count is slightly higher than the operation count.

| Category (`docs/endpoints/`) | Client property | Interface | Operations |
|---|---|---|---|
| appstorehistory | `AppStoreHistory` | `IAppStoreHistory` | 1 |
| associations | `Associations` | `IAssociations` | 2 |
| confidence | `Confidence` | `IConfidence` | 1 |
| enrichments | `Enrichments` | `IEnrichments` | 3 |
| enrichments | `PassiveDns` | `IPassiveDns` | 1 |
| feed | `Feeds` | `IFeeds` | 1 |
| import | `ObservableImports` | `IObservableImports` | 2 |
| import | `StixImports` | `IStixImports` | 1 |
| import | `ImportSessions` | `IImportSessions` | 4 |
| intelligence | `Observables` | `IObservables` | 3 |
| intelligence | `SearchFilters` | `ISearchFilters` | 1 |
| observable-tags | `ObservableTags` | `IObservableTags` | 1 |
| report | `Reports` | `IReports` | 2 |
| rules | `Rules` | `IRules` | 4 |
| sandbox | `SandboxSubmissions` | `ISandboxSubmissions` | 4 |
| snapshot | `Snapshots` | `ISnapshots` | 3 |
| sso-history | `SsoHistory` | `ISsoHistory` | 1 |
| threat-model-search | `ThreatModelSearch` | `IThreatModelSearch` | 1 |
| threat-model-tags | `ThreatModelTags` | `IThreatModelTags` | 1 |
| threat-model | `Actors` | `IActors` | 7 |
| threat-model | `AttackPatterns` | `IAttackPatterns` | 7 |
| threat-model | `Campaigns` | `ICampaigns` | 7 |
| threat-model | `Incidents` | `IIncidents` | 7 |
| threat-model | `Malware` | `IMalware` | 7 |
| threat-model | `Signatures` | `ISignatures` | 8 |
| threat-model | `ThreatBulletins` | `IThreatBulletins` | 8 |
| threat-model | `Ttps` | `ITtps` | 7 |
| threat-model | `Vulnerabilities` | `IVulnerabilities` | 7 |
| threat-model | `VictimTypes` | `IVictimTypes` | 1 |
| threat-model | `ActorSophisticationTypes` | `IActorSophisticationTypes` | 1 |
| threat-model | `CampaignStatuses` | `ICampaignStatuses` | 1 |
| threat-model | `IncidentStatusTypes` | `IIncidentStatusTypes` | 1 |
| user-administration | `OrgAdminUsers` | `IOrgAdminUsers` | 6 |
| user-audit | `UserActions` | `IUserActions` | 1 |
| user-audit | `ApiUserActions` | `IApiUserActions` | 1 |
| user-audit | `AsyncResults` | `IAsyncResults` | 2 |
| users-roles | `UserRoles` | `IUserRoles` | 6 |
| users-roles | `SsoPermissions` | `ISsoPermissions` | 4 |
| users-roles | `IdentityProviders` | `IIdentityProviders` | 1 |
| whitelist | `Whitelist` | `IWhitelist` | 5 |
| workgroup | `Workgroups` | `IWorkgroups` | 1 |
| workgroup | `ObservableWorkgroups` | `IObservableWorkgroups` | 1 |
| **Total** | | | **134** |

Each threat model type (actors, attack patterns, campaigns, incidents, malware, signatures, threat bulletins, TTPs,
vulnerabilities) has list, create, get, update, `ListIntelligenceAsync`, `ListLinkedAsync` and the
`ListAssociatedWithValueAsync` / `ListAssociatedWithIdsAsync` lookup. Signatures add `ListTypesAsync` and threat
bulletins add `AddAttachmentAsync`.

## Verification status

**No operation in this package has been run against a live ThreatStream.** ThreatStream is a commercial platform with no
public sandbox, and Panoramic Data has not had access to a deployment. What has been done:

- Every operation has a unit test that pins the exact HTTP method, path, query string and body it sends.
- Every operation has a test that parses a response. The responses come from the examples in the ThreatStream API
  reference (extended where the reference is thin), not from a real server.
- Response models for the threat model types (actors, campaigns, incidents, malware, signatures, threat bulletins, TTPs,
  vulnerabilities, attack patterns) are the least certain: the reference documents their request fields but gives no
  response examples, so their shapes are inferred. Unmodelled properties are preserved in
  `ThreatStreamObject.AdditionalProperties`, but property names, types and nullability may be wrong.
- Integration tests exist (see [docs/TESTING.md](docs/TESTING.md)) and are read-only by default, but they have never been
  run, because there are no credentials to run them with.

Ten operations are additionally **inferred**: the reference names the path but does not document it, or shows it only as
a link in another response, so the method, parameters and response shape follow ThreatStream's conventions and are
unit-verified only. They are marked in their XML documentation.

| Client method | Why it is inferred |
|---|---|
| `IActorSophisticationTypes.ListAsync` | Path named in the actor attribute table, not documented. |
| `ICampaignStatuses.ListAsync` | Path named in the campaign attribute table, not documented. |
| `IIncidentStatusTypes.ListAsync` | Path named in the incident attribute table, not documented. |
| `IVictimTypes.ListAsync` | Path named in the actor, campaign and incident attribute tables, not documented. |
| `ISignatures.ListTypesAsync` | Path named in the `s_type` attribute, not documented. |
| `IImportSessions.ListIntelligenceAsync` | The reference says only that observable IDs are in their `id` attributes. |
| `IObservables.ListAssociatedWithIntelligenceAsync` | Path appears only in the rate limit table; parameters follow the threat model `associated_with_intelligence` lookups. |
| `ISandboxSubmissions.GetAsync` | Shown only as the `status` link of a submission. |
| `ISandboxSubmissions.GetReportAsync` | Shown only as the `detail` link of a submission. |
| `IReports.GetAsyncReportAsync` | Shown only as a `callbackUrl` on `optic.threatstream.com`; this client sends it to your base address. |

Also unconfirmed: `ISnapshots.CreateAsync` documents the new snapshot's address only in a `Location` header, which the
method cannot return, so its `Id` may be 0 (find the snapshot with `ListAsync` ordered by `-id`).

**Sandbox access wanted.** Panoramic Data is seeking sandbox access to ThreatStream through its
[technology partnerships programme](https://www.panoramicdata.com/technology-partnerships/). Until then, treat the
package as unit-verified only, test against your own deployment before depending on it, and please
[report anything that behaves differently](https://github.com/panoramicdata/ThreatStream.Api/issues).

## Quality

- Unit tests need no network and run in CI; Codacy grade and coverage are shown by the badges above.
- Zero compiler warnings, nullable reference types, XML documentation on every public member.
- Credentials are never logged, certificate validation is never turned off, and a read-only mode is available.

## Links

- NuGet: https://www.nuget.org/packages/ThreatStream.Api
- Source: https://github.com/panoramicdata/ThreatStream.Api
- Issues: https://github.com/panoramicdata/ThreatStream.Api/issues
- Contributing: [CONTRIBUTING.md](CONTRIBUTING.md), adding endpoints: [docs/IMPLEMENTING.md](docs/IMPLEMENTING.md)
- Testing: [docs/TESTING.md](docs/TESTING.md)
- Security: [SECURITY.md](SECURITY.md)

## License

MIT. See [LICENSE](LICENSE).
