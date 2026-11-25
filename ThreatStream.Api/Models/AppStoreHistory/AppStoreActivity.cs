using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.AppStoreHistory;

/// <summary>One entry of your organisation's APP Store activity log (<c>api/v1/appstorehistory/</c>).</summary>
public sealed class AppStoreActivity : ThreatStreamResource
{
	/// <summary>
	/// The kind of activity, such as <c>Accepted Paid</c>, <c>Accepted Trial</c>, <c>Activation</c>, <c>Approved</c> or
	/// <c>Credential Change</c> (<c>edit_type</c>).
	/// </summary>
	[JsonPropertyName("edit_type")]
	public string? EditType { get; init; }

	/// <summary>The name of the APP Store service (<c>name</c>).</summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>The email address of the user who acted; absent for actions by vendors (<c>email</c>).</summary>
	[JsonPropertyName("email")]
	public string? Email { get; init; }

	/// <summary>When it happened (<c>timestamp</c>).</summary>
	[JsonPropertyName("timestamp")]
	public DateTimeOffset? Timestamp { get; init; }
}
