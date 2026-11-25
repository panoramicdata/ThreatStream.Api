using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.ThreatModels;

/// <summary>
/// The review state of a threat model entity (<c>publication_status</c>). An entity must be
/// <see cref="Published"/> to be visible to users outside the organisation that owns it.
/// </summary>
public enum ThreatModelPublicationStatus
{
	/// <summary>A status this library does not know.</summary>
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

	/// <summary>Reviewed but not yet published (<c>reviewed</c>).</summary>
	[JsonStringEnumMemberName("reviewed")]
	Reviewed = 4,

	/// <summary>Published, and so visible outside the owning organisation (<c>published</c>).</summary>
	[JsonStringEnumMemberName("published")]
	Published = 5
}
