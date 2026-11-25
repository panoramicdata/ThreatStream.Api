namespace ThreatStream.Api.Models.UsersRoles;

/// <summary>
/// An SSO identity provider configured for the organisation (<c>api/v1/identity-provider/</c>). The reference documents
/// only that its ID is the <c>idp_id</c> of SSO security group mappings, not its fields, so everything except
/// <see cref="ThreatStreamResource.Id"/> and <see cref="ThreatStreamResource.ResourceUri"/> is kept in
/// <see cref="ThreatStreamObject.AdditionalProperties"/>.
/// </summary>
public sealed class IdentityProvider : ThreatStreamResource;
