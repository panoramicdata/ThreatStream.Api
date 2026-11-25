# Implementing endpoints

This is the contract for adding ThreatStream REST operations to ThreatStream.Api. The `confidence` category
(`IConfidence`, `Models/Confidence/ConfidenceTrend`, `ThreatStreamClient.Confidence.cs`, `ConfidenceTests`,
`ConfidenceIntegrationTests`) is the worked example; copy its shape.

## Source of truth

- `docs/endpoints/<category>.md` lists every operation of the Anomali ThreatStream API reference
  (`https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm` and the pages it links to), one row per
  path and method. Read the operation's own page for its parameters, response fields and examples before writing it.
- When an operation is implemented, fill its row: `Client method` = `` `IInterface.Method` `` (several, comma-separated,
  when more than one method serves the operation), `Test` = `` `TestClass.TestMethod` `` of the unit test that pins its
  request. `InventoryTests` checks that:
  - each named method exists and its verb and path match the row. The path is compared literally, including the
    `api/vN/` prefix, apart from leading and trailing slashes, the query string and placeholder names (`{id}`,
    `{actorId}` and `<id>` all match). A placeholder never stands for a literal segment: the row
    `/api/v1/actor/{id}/` needs an attribute path `api/v1/actor/{id}/`, not `api/v1/{modelType}/{id}/`.
  - each named test exists, and every Refit method in the library appears in some row.
  - every non-deprecated row of a category not listed in `docs/pending-categories.txt` is filled.
- Rows marked `(deprecated)` are **not** implemented (`/api/v1/intelligence/*` is replaced by `/api/v2/intelligence/`).
- A category leaves `docs/pending-categories.txt` once every non-deprecated row is filled.
- The reference has errors (listed in the lead's inventory: missing `/api` in a few examples, PATCH/DELETE listed on
  collections where the examples use `{id}`, missing trailing slashes). Follow the examples and Django's conventions
  (paths end in `/`), and say what you did in the method's XML `<remarks>`. If a row itself is wrong, tell the lead
  rather than editing other categories' rows.

## What you may edit

Each category agent works only in its own files:

- `ThreatStream.Api/Interfaces/I<Family>.cs` (new files)
- `ThreatStream.Api/Models/<Category>/` (new folder)
- `ThreatStream.Api/ThreatStreamClient.<Category>.cs` (new file)
- `ThreatStream.Api.Test/Groups/<Family>Tests*.cs` and, if needed, `ThreatStream.Api.Test/Support/<Category>TestKit.cs`
- `ThreatStream.Api.IntegrationTest/<Category>/`
- the rows of `docs/endpoints/<category>.md`, and your category's line in `docs/pending-categories.txt`

Do **not** edit the core: `ThreatStreamClient.cs`, `ThreatStreamClient.Paging.cs`, `ThreatStreamClientOptions.cs`,
`ThreatStreamJson.cs`, the exceptions and error mapper, `Handlers/`, `Serialization/`, the shared models in
`Models/*.cs`, the shared test `Support/` files or `Core/` tests. If you need a core change (a new read-only POST on the
allow-list, a converter, a shared model), stop and ask the lead, saying exactly what and why.

## Layout and naming

- Namespace root `ThreatStream.Api`. Interfaces in `ThreatStream.Api.Interfaces`; models in
  `ThreatStream.Api.Models.<Category>`, one public type per file. Never name a namespace segment `System`, and never
  give a type the same name as its namespace's last segment.
- Category folder and partial-file names:

  | docs/endpoints | Folder, namespace and `ThreatStreamClient.<Category>.cs` |
  |---|---|
  | appstorehistory | `AppStoreHistory` |
  | associations | `Associations` |
  | confidence | `Confidence` (done) |
  | enrichments | `Enrichments` |
  | feed | `Feeds` |
  | import | `Imports` |
  | intelligence | `Intelligence` |
  | observable-tags | `ObservableTags` |
  | report | `Reports` |
  | rules | `Rules` |
  | sandbox | `Sandbox` |
  | snapshot | `Snapshots` |
  | sso-history | `SsoHistory` |
  | threat-model | `ThreatModel` |
  | threat-model-search | `ThreatModelSearch` |
  | threat-model-tags | `ThreatModelTags` |
  | user-administration | `UserAdministration` |
  | user-audit | `UserAudit` |
  | users-roles | `UsersRoles` |
  | whitelist | `Whitelist` |
  | workgroup | `Workgroups` |

- **Interfaces:** one per endpoint family (a resource path such as `api/v1/rule/`), named for the resource in the plural:
  `IRules`, `IFeeds`, `IActors`, `IMalware`, `IImportSessions`. Interface names and client property names must be unique
  across the library, so prefix where a name could clash (the threat model types are `IActors`, `IAttackPatterns`,
  `ICampaigns`, `IIncidents`, `IMalware`, `ISignatures`, `IThreatBulletins` for `tipreport`, `ITtps`, `IVulnerabilities`,
  one interface per type).
- **Client properties:** `ThreatStreamClient.<Category>.cs` holds one line per interface, with an XML summary naming the
  path: `public IRules Rules => field ??= For<IRules>();`.
- **Shared models already in `ThreatStream.Api.Models`** (reuse, do not duplicate): `ThreatStreamPage<T>`,
  `ThreatStreamPageMeta`, `ThreatStreamObject` (keeps unmodelled properties in `AdditionalProperties`),
  `ThreatStreamResource` (adds `long Id` and `ResourceUri`), `ListOptions` (`Limit`, `Offset`, `OrderBy`), `Tlp`,
  `ThreatStreamTag`, `ThreatStreamMultipartRequest` and `ThreatStreamFile`.
- A model one category defines and another needs (the observable model, for example) belongs to the category that owns
  the resource (`Models/Intelligence/Observable.cs`); the other category references it and says so in its report.

## Interface methods

- Refit attribute paths are relative, include the version prefix, have no leading slash and keep the trailing slash:
  `[Get("api/v1/rule/")]`, `[Patch("api/v1/rule/{id}/")]`. The client appends them to the base URL (US, EU or on-prem).
- Every method returns `Task` or `Task<T>`, takes a **required** `CancellationToken cancellationToken` last, and has
  **no optional parameters** (Sonar S2360). Optional query parameters travel in one nullable options object
  (`[Query] XListOptions? options`; callers pass `null`), optional body fields in the request model.
- IDs are `long id` (or a more specific camelCase name, such as `long importSessionId`), used in the attribute's braces.
  Refit escapes a path value as one segment (`a/b` becomes `a%2Fb`). A required scalar query parameter is a plain
  parameter with `[AliasAs("wire_name")]` (see `IConfidence.GetTrendAsync`).
- Standard shapes for a resource `X` (adapt to what the reference documents):
  - list: `Task<ThreatStreamPage<X>> ListAsync([Query] XListOptions? options, CancellationToken cancellationToken)`, where
    `XListOptions : ListOptions` adds the endpoint's filters, each with `[AliasAs("wire_name")]` (including the operator
    suffix: `[AliasAs("created_ts__gte")]`). Filter-heavy endpoints also take the advanced query expression as
    `[AliasAs("q")] public string? Query`. Lists of values are sent comma-separated (`id__in=1,2,3`) automatically.
    Use `ListOptions` itself when the endpoint has no filters of its own.
  - get: `Task<X> GetAsync(long id, CancellationToken cancellationToken)`.
  - create: `Task<X> CreateAsync([Body] XCreateRequest request, CancellationToken cancellationToken)`.
  - update (PATCH): `Task<X> UpdateAsync(long id, [Body] XUpdateRequest request, CancellationToken cancellationToken)`.
  - delete: `Task DeleteAsync(long id, CancellationToken cancellationToken)`.
  - actions (`approve_all`, `unlock`, `bulk_add`, `clone`...) get their own method named for the action, returning the
    documented response model (a job or import session reference for asynchronous work).
- Responses that are not JSON: `Task<string>` for text (CSV exports, user audit reports), `Task<Stream>` or
  `Task<HttpContent>` for a download the caller reads and disposes. **Never** return `HttpResponseMessage` or
  `ApiResponse<T>`: they bypass the error mapping.
- Callers page with `client.EnumerateAsync(ct => client.X.ListAsync(options, ct), cancellationToken)`; observables beyond
  10,000 with `ThreatStreamClient.EnumerateByUpdateIdAsync`. Do not write paging loops in interfaces.
- XML documentation on every public member: what the operation does, the verb and path in `<c>...</c>`, required
  permissions (Org Admin, Approve Intelligence), side effects, rate limits that differ from the default, and anything
  surprising (OnPrem-only parameters, asynchronous jobs).

## Request models

- **JSON bodies** (most POST and PATCH): a plain class with `[JsonPropertyName("wire_name")]` on **every** property.
  Documented required fields use the C# `required` modifier; everything else is nullable and omitted when `null`.
  Booleans are `bool?`, numbers `int?`/`long?`/`double?`, timestamps `DateTimeOffset?` (written as UTC ISO 8601), enums
  by `[JsonStringEnumMemberName]`, ID lists `IReadOnlyList<long>?`, tags `IReadOnlyList<ThreatStreamTag>?`.
- **Multipart bodies** (import with approval, STIX import, sandbox submission, whitelist upload, threat bulletin
  attachment): derive from `ThreatStreamMultipartRequest` and pass it as `[Body]`. Put `[JsonPropertyName]` on every
  property; a file is a `ThreatStreamFile` property. See `ThreatStreamMultipartRequest` for how each type is sent
  (scalar lists comma-separated, complex values as JSON). Undocumented fields from the examples go in
  `AdditionalFields`.
- The import-without-approval body (`PATCH api/v2/intelligence/`) is `{"meta": {...}, "objects": [...]}`: model it as a
  request class with those two properties.

## Response models

- Derive from `ThreatStreamResource` when the object has `id` and `resource_uri`, else from `ThreatStreamObject`. Model
  the documented and commonly used fields; everything else stays in `AdditionalProperties`.
- `[JsonPropertyName("wire")]` on every property. IDs `long` (or `long?` when optional); counts `int?`/`long?`; scores
  `int?` or `double?`; booleans `bool?`; timestamps `DateTimeOffset?` (the shared options read ISO text without an offset
  as UTC, and epoch seconds); lists `IReadOnlyList<T>` defaulting to `[]`; strings `string?`. `ThreatStreamJson.Options`
  already reads ThreatStream's loose types (`"1"`, `"true"`, `""`, quoted numbers), so do not add converters.
- Enums only for small closed sets, first member `Unknown = 0`, the rest with `[JsonStringEnumMemberName("wire")]`
  (unrecognised values read as `Unknown`). Use `Tlp` for TLP. Prefer `string` for open or growing sets (`itype`,
  `threat_type`, sandbox platforms).

## Tests (ThreatStream.Api.Test, xUnit v3 + AwesomeAssertions)

- One test class per interface: `ThreatStream.Api.Test/Groups/<Interface without I>Tests.cs`, split into partial files
  when it grows (Codacy grades a file down past about 40 total complexity or 300 lines).
- **Every operation** has a request test asserting the exact method, escaped path, query and body:

  ```csharp
  var call = await TestClient.CaptureAsync((client, ct) => client.Rules.GetAsync(5, ct), RuleJson);
  call.ShouldBe(HttpMethod.Get, "/api/v1/rule/5/");                    // query "" and no body by default
  call.ShouldBe(HttpMethod.Post, "/api/v1/rule/", body: """{"name":"x"}"""); // JSON body, exact text
  ```

  `ShouldBe` also checks the request carried the API key. Assert a multipart body through `call.Parts` (name, file
  name, content type, text) instead of `body`. JSON bodies use System.Text.Json's default escaping, so an expected body
  spells `+`, `&`, `<`, `>` and `'` as `+`, `&`, `<`, `>` and `'` (for example
  `"tlp":"amber+strict"`); write those expectations in a regular string, or avoid the characters in test data.
- Every response model has a mapping test asserting **every** modelled field, from the reference's example response
  (anonymise organisation names, emails and keys). Use `TestClient.ReadAsync`.
- Each interface has at least one error test: `TestClient.ShouldFailAsync(call, status, body, expectedMessage)`.
- Shared JSON goes in constants or a `Support/<Category>TestKit.cs`, repeated assertions in helpers, so Codacy finds no
  duplication. No skipped tests (`failSkips`), no `Thread.Sleep`, no network.
- Line and branch coverage of what you add must be 100%.
- These core tests already enforce the rules above on everything you add; do not weaken them:
  - `InventoryTests`: rows, client methods, tests and pending categories in step.
  - `ApiSurfaceTests`: no optional parameters in the public API; endpoint interfaces public; every endpoint method a
    Refit operation ending in a `CancellationToken`; attribute paths relative, starting `api/v` and ending in `/`; no
    `HttpResponseMessage` or `ApiResponse<T>` returns; exactly one `ThreatStreamClient` property per interface, with
    unique names; `[JsonPropertyName]` (or `[AliasAs]` on query options) on every model property; timestamps typed
    `DateTimeOffset?`. If an endpoint genuinely needs an exception (a path without a trailing slash that a live
    deployment requires), ask the lead.
- Throwaway scripts and captured responses go under `temp/<your category>/` (git-ignored), never the shared scratchpad
  root, so parallel agents do not overwrite each other.

## Live verification (ThreatStream.Api.IntegrationTest)

- No ThreatStream sandbox account is available yet, so live tests are written but cannot be run; say so in your report
  and record the operations as unit-verified only. Build the project anyway.
- Add tests under `ThreatStream.Api.IntegrationTest/<Category>/` with `[Collection(ThreatStreamTestGroup.Name)]` and
  `ThreatStreamFixture`. Use `fixture.Client`, which is **read-only**. Cover the read operations: at most one or two
  tests per interface (rate limits: one request a minute on undocumented endpoints).
- Writes only where they are safe and reversible: a test creates its own client with
  `fixture.CreateOptions(o => o.IsReadOnly = false)`, names every object with `ThreatStreamFixture.UniqueName(...)`
  (prefix `threatstream_api_it_`) and deletes it in the same test (try/finally).
- **Never** live: sandbox detonations, imports of any kind, clone, snapshot creation, bulk tagging or workgroup changes on
  existing observables, user administration, roles and SSO mappings, whitelist changes, report generation. Unit tests
  only for those.
- Never print, log or commit the credentials (`ThreatStream:Username`, `ThreatStream:ApiKey` in user secrets).

## Code quality

- Zero warnings (`TreatWarningsAsErrors`), XML docs on all public API, file-scoped namespaces, tabs, CRLF. Files written
  by tools are LF: run `dotnet format whitespace <project>` before building. Write C# with an editor tool, never a shell
  heredoc (it eats backslashes).
- Keep each file small and simple (Codacy grade A): split large interfaces or models into partial files or separate
  families rather than growing one file.
- Build only the projects you touched: `dotnet build ThreatStream.Api.Test/ThreatStream.Api.Test.csproj`, then
  `dotnet test --project ThreatStream.Api.Test/ThreatStream.Api.Test.csproj --no-build` (exit code 0 = all passed).
  Run the whole unit suite, not a filter, before reporting.
