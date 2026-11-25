# Changelog

## Unreleased

## 1.0

Initial release.

- Typed Refit client for the Anomali ThreatStream REST API (v1 and v2) on System.Text.Json, with 134 operations across 42
  client groups: observables, imports and import sessions, STIX imports, enrichments and passive DNS, confidence, feeds,
  rules, sandbox submissions, snapshots, reports, associations, observable and threat model tags, threat model search,
  the threat model types (actors, attack patterns, campaigns, incidents, malware, signatures, threat bulletins, TTPs,
  vulnerabilities) and their attribute lists, organisation users, user and API activity reports, roles, SSO permissions
  and identity providers, SSO and app store history, whitelist, workgroups and asynchronous jobs.
- API key and OAuth 2.0 client-credentials authentication; US cloud, EU cloud and custom (on-premises) base URLs, with
  optional SHA-256 certificate pinning.
- Paging helpers: `EnumerateAsync` (`meta.next`), `EnumerateObservablesAsync`, and `update_id` paging with
  `EnumerateByUpdateIdAsync` and `EnumerateObservablesByUpdateIdAsync`.
- `RunAsyncJobAsync` to start, poll and download an asynchronous job; multipart uploads; text (CSV or JSON) responses.
- `IsReadOnly` mode with an allow-list of read-only POSTs, which also refuses GETs that start server-side jobs.
- Rate-limit aware retries (429 and 503 with `Retry-After`, `MaxRetryDelay` 60 seconds by default, connection retries).
- Not verified against a live ThreatStream: there is no sandbox available, so every operation is unit-verified against
  the API reference only. Ten operations are inferred from the reference's examples and links (see the README).
- Deprecated `/api/v1/intelligence` operations (`PATCH {observable_id}` and `POST import/`) are not implemented.
