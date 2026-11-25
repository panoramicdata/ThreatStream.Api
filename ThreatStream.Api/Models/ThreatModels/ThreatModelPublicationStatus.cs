using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>The review stage of a threat model entity (<c>publication_status</c>, or <c>status</c> on a threat bulletin).</summary>
public enum ThreatModelPublicationStatus
{
	/// <summary>A stage this library does not know.</summary>
	Unknown = 0,

	/// <summary>Newly created (<c>new</c>).</summary>
	[JsonStringEnumMemberName("new")]
	New = 1,

	/// <summary>Waiting for review (<c>pending_review</c>).</summary>
	[JsonStringEnumMemberName("pending_review")]
	PendingReview = 2,

	/// <summary>A review has been requested (<c>review_requested</c>).</summary>
	[JsonStringEnumMemberName("review_requested")]
	ReviewRequested = 3,

	/// <summary>Reviewed (<c>reviewed</c>).</summary>
	[JsonStringEnumMemberName("reviewed")]
	Reviewed = 4,

	/// <summary>Published, and so visible outside the organisation (<c>published</c>).</summary>
	[JsonStringEnumMemberName("published")]
	Published = 5
}
