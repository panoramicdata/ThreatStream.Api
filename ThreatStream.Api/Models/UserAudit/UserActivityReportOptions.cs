using Refit;

namespace ThreatStream.Api.Models.UserAudit;

/// <summary>
/// What a user activity report covers (<c>api/v1/useraction/</c> and <c>api/v1/apiuseraction/</c>). <c>limit</c> caps
/// the number of records (20 by default); <c>order_by</c> is typically <c>-created_ts</c>.
/// </summary>
public sealed class UserActivityReportOptions : ListOptions
{
	/// <summary>The start of the time range, inclusive (<c>created_ts__gte</c>), sent as UTC.</summary>
	[AliasAs("created_ts__gte")]
	public DateTimeOffset? CreatedFrom { get; set; }

	/// <summary>The end of the time range, inclusive (<c>created_ts__lte</c>), sent as UTC.</summary>
	[AliasAs("created_ts__lte")]
	public DateTimeOffset? CreatedUntil { get; set; }

	/// <summary>The report's file format (<c>format</c>).</summary>
	[AliasAs("format")]
	public UserActivityReportFormat? Format { get; set; }

	/// <summary>The IDs of the users to report on (<c>user_id__in</c>); every user of the organisation when <see langword="null"/>.</summary>
	[AliasAs("user_id__in")]
	public IReadOnlyList<long>? UserIds { get; set; }
}
