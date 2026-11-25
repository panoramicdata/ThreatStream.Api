using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Malware threat model entities (<c>api/v1/malware/</c>).</summary>
	public IMalware Malware => field ??= For<IMalware>();

	/// <summary>Signature threat model entities and signature types (<c>api/v1/signature/</c>, <c>api/v1/signature_type/</c>).</summary>
	public ISignatures Signatures => field ??= For<ISignatures>();

	/// <summary>Threat bulletins and their attachments (<c>api/v1/tipreport/</c>).</summary>
	public IThreatBulletins ThreatBulletins => field ??= For<IThreatBulletins>();

	/// <summary>Tactics, techniques and procedures (<c>api/v1/ttp/</c>).</summary>
	public ITtps Ttps => field ??= For<ITtps>();

	/// <summary>Vulnerability threat model entities (<c>api/v1/vulnerability/</c>).</summary>
	public IVulnerabilities Vulnerabilities => field ??= For<IVulnerabilities>();
}
