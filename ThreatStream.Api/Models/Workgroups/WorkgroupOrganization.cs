using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Workgroups;

/// <summary>The organization that owns a <see cref="Workgroup"/> (<c>owner_org</c>): its ID, name and resource URI.</summary>
public sealed class WorkgroupOrganization : ThreatStreamResource
{
	/// <summary>The organization's name (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
