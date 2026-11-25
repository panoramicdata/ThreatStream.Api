using ThreatStream.Api.Interfaces;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>Observable imports, with approval (<c>api/v2/intelligence/import/</c>) or without (<c>PATCH api/v2/intelligence/</c>).</summary>
	public IObservableImports ObservableImports => field ??= For<IObservableImports>();

	/// <summary>Observable imports from STIX files (<c>api/v1/stix_import/</c>).</summary>
	public IStixImports StixImports => field ??= For<IStixImports>();

	/// <summary>Import jobs (<c>api/v1/importsession/</c>).</summary>
	public IImportSessions ImportSessions => field ??= For<IImportSessions>();
}
