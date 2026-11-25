using Refit;

namespace ThreatStream.Api.Models.ThreatModelSearch;

public sealed partial class ThreatModelSearchOptions
{
	/// <summary>Entities created on ThreatStream at or after this moment (<c>created_ts__gte</c>).</summary>
	[AliasAs("created_ts__gte")]
	public DateTimeOffset? CreatedSince { get; set; }

	/// <summary>Entities created on ThreatStream before this moment (<c>created_ts__lt</c>).</summary>
	[AliasAs("created_ts__lt")]
	public DateTimeOffset? CreatedBefore { get; set; }

	/// <summary>Entities changed at or after this moment (<c>modified_ts__gte</c>).</summary>
	[AliasAs("modified_ts__gte")]
	public DateTimeOffset? ModifiedSince { get; set; }

	/// <summary>Entities changed before this moment (<c>modified_ts__lt</c>).</summary>
	[AliasAs("modified_ts__lt")]
	public DateTimeOffset? ModifiedBefore { get; set; }

	/// <summary>Entities published at or after this moment (<c>published_ts__gte</c>).</summary>
	[AliasAs("published_ts__gte")]
	public DateTimeOffset? PublishedSince { get; set; }

	/// <summary>Campaigns that started at or after this moment (<c>start_date__gte</c>).</summary>
	[AliasAs("start_date__gte")]
	public DateTimeOffset? StartedSince { get; set; }

	/// <summary>Campaigns that ended before this moment (<c>end_date__lt</c>).</summary>
	[AliasAs("end_date__lt")]
	public DateTimeOffset? EndedBefore { get; set; }

	/// <summary>Entities whose source created them at or after this moment (<c>source_created__gte</c>).</summary>
	[AliasAs("source_created__gte")]
	public DateTimeOffset? SourceCreatedSince { get; set; }

	/// <summary>Entities whose source last changed them at or after this moment (<c>source_modified__gte</c>).</summary>
	[AliasAs("source_modified__gte")]
	public DateTimeOffset? SourceModifiedSince { get; set; }

	/// <summary>A target industry from the STIX 2.1 vocabulary (<c>target_industry</c>).</summary>
	[AliasAs("target_industry")]
	public string? TargetIndustry { get; set; }

	/// <summary>A targeted region, country or US state, such as <c>europe</c> or <c>US-FL</c> (<c>target_locations</c>).</summary>
	[AliasAs("target_locations")]
	public string? TargetLocations { get; set; }

	/// <summary>A source region, country or US state (<c>source_locations</c>).</summary>
	[AliasAs("source_locations")]
	public string? SourceLocations { get; set; }

	/// <summary>Set to 1 for an exact <c>total_count</c> on large result sets, at some cost in speed (<c>full_count</c>).</summary>
	[AliasAs("full_count")]
	public int? FullCount { get; set; }

	/// <summary>ThreatStream OnPrem only: <see langword="true"/> for local entities only (<c>local_only</c>).</summary>
	[AliasAs("local_only")]
	public bool? LocalOnly { get; set; }

	/// <summary>
	/// ThreatStream OnPrem only: <see langword="true"/> for remote entities (stored on ThreatStream Cloud) only
	/// (<c>remote_api</c>). Never set it to <see langword="false"/>, which ThreatStream rejects.
	/// </summary>
	[AliasAs("remote_api")]
	public bool? RemoteApi { get; set; }
}
