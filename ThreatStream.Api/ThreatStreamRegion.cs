namespace ThreatStream.Api;

/// <summary>Which ThreatStream deployment <see cref="ThreatStreamClient"/> talks to.</summary>
public enum ThreatStreamRegion
{
	/// <summary>The US cloud, <c>https://api.threatstream.com/</c> (the default).</summary>
	Us = 0,

	/// <summary>The EU cloud, <c>https://api-eu.threatstream.com/</c>.</summary>
	Eu = 1,

	/// <summary>An on-premises (OnPrem) or other deployment at <see cref="ThreatStreamClientOptions.BaseUrl"/>.</summary>
	Custom = 2
}
