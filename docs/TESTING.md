# Testing

## Unit tests

`ThreatStream.Api.Test` needs no network and runs in CI on every push and pull request:

```shell
dotnet test --project ThreatStream.Api.Test/ThreatStream.Api.Test.csproj
```

Every operation has a test that pins its exact HTTP method, path, query string and body, and one that maps a
realistic response (from the ThreatStream API reference's examples, extended where the reference is thin). Skipped
tests fail the run (`failSkips`). `InventoryTests` keeps `docs/endpoints/*.md` and the Refit interfaces in step.

Coverage, as CI collects it:

```shell
dotnet build ThreatStream.Api.Test/ThreatStream.Api.Test.csproj
./ThreatStream.Api.Test/bin/Debug/net11.0/ThreatStream.Api.Test --coverage --coverage-settings "$PWD/coverage.config" --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
```

(`--coverage-settings` must be an absolute path.)

## Integration tests

`ThreatStream.Api.IntegrationTest` runs against a real ThreatStream deployment. ThreatStream is a commercial cloud
service with no Docker image or public sandbox, so the tests use credentials you supply:

```powershell
dotnet user-secrets set ThreatStream:BaseUrl https://api.threatstream.com/ --project ThreatStream.Api.IntegrationTest
dotnet user-secrets set ThreatStream:Username analyst@example.com --project ThreatStream.Api.IntegrationTest
dotnet user-secrets set ThreatStream:ApiKey <your API key> --project ThreatStream.Api.IntegrationTest
dotnet test --project ThreatStream.Api.IntegrationTest/ThreatStream.Api.IntegrationTest.csproj
```

- `ThreatStream:BaseUrl` is `https://api.threatstream.com/` for the US cloud, `https://api-eu.threatstream.com/` for the
  EU cloud, or the URL of an on-premises deployment.
- `ThreatStream:Username` is the account's email address; `ThreatStream:ApiKey` is the key from the My Account tab.
- For an on-premises deployment with a self-signed certificate, also set `ThreatStream:TrustedServerCertificateThumbprint`
  (its SHA-256 thumbprint). Certificate validation is never turned off.
- The settings can also be environment variables (`ThreatStream__BaseUrl`, `ThreatStream__Username`,
  `ThreatStream__ApiKey`). A missing setting fails the tests with a message naming it; nothing is skipped.
- Never print, log or commit the API key.

**Live verification is pending**: no ThreatStream sandbox account is available to the maintainers yet, so the
integration tests have been built but not run against a live deployment. Until they have, every operation is
unit-verified only, against the API reference.

### Safety rules

The account is a production threat intelligence platform shared with analysts, so the tests are conservative:

- The fixture's client is **read-only** (`IsReadOnly = true`): it refuses every state-changing request before sending
  it. Most tests only read.
- A test that must write turns read-only off for its own client, creates only objects named `threatstream_api_it_...`
  (`ThreatStreamFixture.UniqueName`), and deletes them in the same test.
- **Never** run live: sandbox detonations, observable imports (with or without approval, STIX, clone), snapshot creation,
  bulk tagging or workgroup changes on real observables, user administration, role or SSO mapping changes, whitelist
  changes, or PDF report generation. Those are unit-verified only.
- ThreatStream rate-limits per user (150 requests a minute on the busy endpoints, and one a minute on any endpoint
  without a documented limit). Test collections run one at a time, and the fixture waits up to 75 seconds for a rate
  limit window, so keep live tests few.

CI does not run the integration tests: that would need ThreatStream credentials in every build.
