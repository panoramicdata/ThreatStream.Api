# user-administration endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/orgadmin/` | `IOrgAdminUsers.ListAsync` | `OrgAdminUsersTests.ListAsync_SendsGet` |
| POST | `/api/v1/orgadmin/` | `IOrgAdminUsers.CreateAsync` | `OrgAdminUsersTests.CreateAsync_SendsPostWithPrivileges` |
| GET | `/api/v1/orgadmin/{user_id}/` | `IOrgAdminUsers.GetAsync` | `OrgAdminUsersTests.GetAsync_SendsGetForTheUser` |
| PATCH | `/api/v1/orgadmin/{user_id}/` | `IOrgAdminUsers.UpdateAsync` | `OrgAdminUsersTests.UpdateAsync_SendsPatchWithOnlyTheChanges` |
| DELETE | `/api/v1/orgadmin/{user_id}/` | `IOrgAdminUsers.DeleteAsync` | `OrgAdminUsersTests.DeleteAsync_SendsDelete` |
| PATCH | `/api/v1/orgadmin/{user_id}/unlock/` | `IOrgAdminUsers.UnlockAsync` | `OrgAdminUsersTests.UnlockAsync_SendsPatchWithoutABody` |
