using Refit;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// Paging, ordering and the common filters of a threat model list (<c>api/v1/{type}/</c>). Pass
/// <see langword="null"/> for ThreatStream's defaults.
/// </summary>
/// <remarks>
/// The type pages document only <c>limit</c>; the filters are ThreatStream's general field filters (field, then an
/// operator suffix), applied to the attributes every threat model entity shares. For searching across types, Anomali
/// recommends <c>api/v1/threat_model_search/</c>.
/// </remarks>
public class ThreatModelListOptions : ListOptions
{
	/// <summary>Only these entity IDs (<c>id__in</c>), sent comma-separated.</summary>
	[AliasAs("id__in")]
	public IReadOnlyList<long>? IdIn { get; set; }

	/// <summary>Only entities with exactly this name (<c>name</c>).</summary>
	[AliasAs("name")]
	public string? Name { get; set; }

	/// <summary>Only entities whose name contains this text (<c>name__contains</c>).</summary>
	[AliasAs("name__contains")]
	public string? NameContains { get; set; }

	/// <summary>Only entities in this review state (<c>publication_status</c>).</summary>
	[AliasAs("publication_status")]
	public ThreatModelPublicationStatus? PublicationStatus { get; set; }

	/// <summary>Only entities changed at or after this moment (<c>modified_ts__gte</c>).</summary>
	[AliasAs("modified_ts__gte")]
	public DateTimeOffset? ModifiedFrom { get; set; }

	/// <summary>Only entities changed before this moment (<c>modified_ts__lt</c>).</summary>
	[AliasAs("modified_ts__lt")]
	public DateTimeOffset? ModifiedBefore { get; set; }
}
