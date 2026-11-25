# users-roles endpoints

Source: https://docs.anomali.com/Content/ThreatStream%20API/supported_endpoints.htm

Paths include the `/api/vN/` prefix and are relative to the base URL. One row per operation. `Client method` is `IInterface.Method`; `Test` is `TestClass.TestMethod`. Deprecated operations are marked and not implemented.

| Method | Path | Client method | Test |
|---|---|---|---|
| GET | `/api/v1/user_role/` | `IUserRoles.ListAsync` | `UserRolesTests.ListAsync_SendsGet` |
| POST | `/api/v1/user_role/` | `IUserRoles.CreateAsync` | `UserRolesTests.CreateAsync_SendsPostWithNameAndPermissions` |
| GET | `/api/v1/user_role/permissions` | `IUserRoles.GetPermissionsAsync` | `UserRolesTests.GetPermissionsAsync_SendsGet` |
| PATCH | `/api/v1/user_role/{id}/` | `IUserRoles.UpdateAsync` | `UserRolesTests.UpdateAsync_SendsPatchWithTheChanges` |
| DELETE | `/api/v1/user_role/{id}/` | `IUserRoles.DeleteAsync` | `UserRolesTests.DeleteAsync_SendsDelete` |
| POST | `/api/v1/user_role/users_set/` | `IUserRoles.AssignAsync` | `UserRolesTests.AssignAsync_SendsPostWithUsersAndRoles` |
| GET | `/api/v1/ssopermission/` | `ISsoPermissions.ListAsync` | `SsoPermissionsTests.ListAsync_SendsGetForTheIdentityProvider` |
| POST | `/api/v1/ssopermission/` | `ISsoPermissions.CreateAsync` | `SsoPermissionsTests.CreateAsync_SendsPostWithGroupProviderAndRoles` |
| PATCH | `/api/v1/ssopermission/{id}/` | `ISsoPermissions.UpdateAsync` | `SsoPermissionsTests.UpdateAsync_SendsPatchWithProviderAndRoles` |
| DELETE | `/api/v1/ssopermission/{id}/` | `ISsoPermissions.DeleteAsync` | `SsoPermissionsTests.DeleteAsync_SendsDelete` |
| GET | `/api/v1/identity-provider/` | `IIdentityProviders.ListAsync` | `IdentityProvidersTests.ListAsync_SendsGet` |
