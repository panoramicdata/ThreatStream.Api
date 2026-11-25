using Refit;

namespace ThreatStream.Api.Models.Intelligence;

public sealed partial class ObservableListOptions
{
	/// <summary>Observables with a confidence of at least this (<c>confidence__gte</c>).</summary>
	[AliasAs("confidence__gte")]
	public int? MinConfidence { get; set; }

	/// <summary>Observables with a confidence of at most this (<c>confidence__lte</c>).</summary>
	[AliasAs("confidence__lte")]
	public int? MaxConfidence { get; set; }

	/// <summary>Observables whose source reported a confidence of at least this (<c>source_reported_confidence__gte</c>).</summary>
	[AliasAs("source_reported_confidence__gte")]
	public int? MinSourceReportedConfidence { get; set; }

	/// <summary>Observables whose machine learning score is at least this (<c>retina_confidence__gte</c>).</summary>
	[AliasAs("retina_confidence__gte")]
	public int? MinRetinaConfidence { get; set; }

	/// <summary>Observables first seen at or after this moment (<c>created_ts__gte</c>).</summary>
	[AliasAs("created_ts__gte")]
	public DateTimeOffset? CreatedSince { get; set; }

	/// <summary>Observables first seen before this moment (<c>created_ts__lt</c>).</summary>
	[AliasAs("created_ts__lt")]
	public DateTimeOffset? CreatedBefore { get; set; }

	/// <summary>Observables changed at or after this moment (<c>modified_ts__gte</c>).</summary>
	[AliasAs("modified_ts__gte")]
	public DateTimeOffset? ModifiedSince { get; set; }

	/// <summary>
	/// Observables changed before this moment (<c>modified_ts__lt</c>). For a continuous sync, set it to a minute ago, since
	/// ThreatStream may still be processing the latest changes.
	/// </summary>
	[AliasAs("modified_ts__lt")]
	public DateTimeOffset? ModifiedBefore { get; set; }

	/// <summary>
	/// Observables whose <c>update_id</c> is greater than this (<c>update_id__gt</c>), with <c>order_by=update_id</c> the way
	/// to page through more than 10,000 results; <see cref="ThreatStreamClient.EnumerateObservablesByUpdateIdAsync"/> sets it.
	/// </summary>
	[AliasAs("update_id__gt")]
	public long? UpdateIdAfter { get; set; }

	/// <summary>Observables whose source created them at or after this moment (<c>source_created__gte</c>).</summary>
	[AliasAs("source_created__gte")]
	public DateTimeOffset? SourceCreatedSince { get; set; }

	/// <summary>Observables whose source last changed them at or after this moment (<c>source_modified__gte</c>).</summary>
	[AliasAs("source_modified__gte")]
	public DateTimeOffset? SourceModifiedSince { get; set; }

	/// <summary>The IP address's country code (<c>country</c>).</summary>
	[AliasAs("country")]
	public string? Country { get; set; }

	/// <summary>The autonomous system number (<c>asn</c>).</summary>
	[AliasAs("asn")]
	public string? Asn { get; set; }

	/// <summary>The IP address a domain or URL resolved to (<c>ip</c>).</summary>
	[AliasAs("ip")]
	public string? Ip { get; set; }

	/// <summary>The registered owner of the IP address (<c>org</c>).</summary>
	[AliasAs("org")]
	public string? Org { get; set; }

	/// <summary>The reverse DNS name of the IP address (<c>rdns</c>).</summary>
	[AliasAs("rdns")]
	public string? Rdns { get; set; }

	/// <summary>
	/// A copy of these options for the page after <paramref name="updateId"/>: ordered by <c>update_id</c>, with the bound in
	/// <see cref="UpdateIdAfter"/>, or inside <see cref="Query"/> when there is one (ThreatStream ignores
	/// <c>update_id__gt</c> beside an advanced search).
	/// </summary>
	internal ObservableListOptions AfterUpdateId(long updateId)
	{
		var copy = (ObservableListOptions)MemberwiseClone();
		copy.OrderBy = "update_id";
		if (string.IsNullOrWhiteSpace(Query))
		{
			copy.UpdateIdAfter = updateId;
		}
		else
		{
			copy.UpdateIdAfter = null;
			copy.Query = $"({Query}) AND update_id>{updateId.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
		}

		return copy;
	}
}
