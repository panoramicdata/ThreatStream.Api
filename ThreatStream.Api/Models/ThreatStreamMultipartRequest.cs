namespace ThreatStream.Api.Models;

/// <summary>
/// The base of request models sent as <c>multipart/form-data</c> (imports, sandbox submissions, STIX imports, whitelist
/// uploads, threat bulletin attachments). Pass a derived model as the <c>[Body]</c> of an interface method and the client
/// sends each property as one form part.
/// </summary>
/// <remarks>
/// <para>
/// Each public property is one part, named by its <see cref="System.Text.Json.Serialization.JsonPropertyNameAttribute"/>
/// (else its snake_case name); <see langword="null"/> properties are omitted, as are those marked
/// <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute"/>. A <see cref="ThreatStreamFile"/> is sent as a file part;
/// a string as given; a boolean, number, enum or date as one formatted value (lowercase booleans, enum wire names, UTC ISO
/// 8601 dates); a list of such values as one comma-separated value (ThreatStream's form for ID lists); any other object
/// (such as a list of tags) as JSON text.
/// </para>
/// <para>
/// Every part is buffered, so the request can be retried after a rate limit or a renewed OAuth token.
/// </para>
/// </remarks>
public abstract class ThreatStreamMultipartRequest
{
	/// <summary>
	/// Extra form fields sent after the typed properties, for parameters this library does not model. A key must not repeat
	/// a typed property's field name.
	/// </summary>
	[System.Text.Json.Serialization.JsonIgnore]
	public IDictionary<string, string> AdditionalFields { get; init; } = new Dictionary<string, string>();
}
