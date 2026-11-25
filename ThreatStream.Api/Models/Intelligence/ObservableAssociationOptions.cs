using Refit;

namespace ThreatStream.Api.Models.Intelligence;

/// <summary>
/// What <c>GET api/v2/intelligence/associated_with_intelligence/</c> looks up: an observable value or a list of
/// observable IDs (set one), as for the threat model types' <c>associated_with_intelligence</c> lookups.
/// </summary>
public sealed class ObservableAssociationOptions : ListOptions
{
	/// <summary>An observable value, such as <c>bad.example</c> (<c>value</c>).</summary>
	[AliasAs("value")]
	public string? Value { get; set; }

	/// <summary>Observable IDs, sent comma-separated (<c>ids</c>).</summary>
	[AliasAs("ids")]
	public IReadOnlyList<long>? Ids { get; set; }
}
