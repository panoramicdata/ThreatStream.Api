using Refit;
using ThreatStream.Api.Models.Confidence;

namespace ThreatStream.Api.Interfaces;

/// <summary>Confidence scores of observables (<c>api/v1/inteldetails/confidence_trend/</c>).</summary>
public interface IConfidence
{
	/// <summary>
	/// Gets the lowest, average and highest confidence ThreatStream has calculated for an observable, as shown on its
	/// details page (<c>GET api/v1/inteldetails/confidence_trend/</c>, with <c>type</c> left at its default,
	/// <c>confidence</c>).
	/// </summary>
	/// <param name="value">The observable's value, such as an IP address, domain, URL, email address or hash.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The confidence scores; each is <see langword="null"/> when ThreatStream has none for the value.</returns>
	/// <remarks>
	/// Not one of the endpoints with a documented rate limit, so ThreatStream's default for those applies: one request
	/// per minute per user.
	/// </remarks>
	[Get("api/v1/inteldetails/confidence_trend/")]
	Task<ConfidenceTrend> GetTrendAsync([AliasAs("value")] string value, CancellationToken cancellationToken);
}
