using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Workgroups;

/// <summary>
/// A workgroup of your organization (<c>api/v1/workgroup/</c>): a set of users to whom the visibility of observables can be
/// restricted.
/// </summary>
public sealed class Workgroup : ThreatStreamResource
{
	/// <summary>The workgroup's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The number of users who belong to the workgroup (<c>members</c>).</summary>
	[JsonPropertyName("members")]
	public int? Members { get; init; }

	/// <summary>When the workgroup was created (<c>created_ts</c>).</summary>
	[JsonPropertyName("created_ts")]
	public DateTimeOffset? CreatedTs { get; init; }

	/// <summary>When the workgroup was last modified (<c>modified_ts</c>).</summary>
	[JsonPropertyName("modified_ts")]
	public DateTimeOffset? ModifiedTs { get; init; }

	/// <summary>The URL of the workgroup's avatar image, if it has one (<c>avatar_s3_url</c>).</summary>
	[JsonPropertyName("avatar_s3_url")]
	public string? AvatarS3Url { get; init; }

	/// <summary>The organization that owns the workgroup: yours (<c>owner_org</c>).</summary>
	[JsonPropertyName("owner_org")]
	public WorkgroupOrganization? OwnerOrg { get; init; }
}
