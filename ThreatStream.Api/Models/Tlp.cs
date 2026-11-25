using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models;

/// <summary>A Traffic Light Protocol designation, which limits how a tag, observable or entity may be shared.</summary>
public enum Tlp
{
	/// <summary>A designation this library does not know.</summary>
	Unknown = 0,

	/// <summary>TLP:RED, for the named recipients only (<c>red</c>).</summary>
	[JsonStringEnumMemberName("red")]
	Red = 1,

	/// <summary>TLP:AMBER, for the recipient's organisation and its clients (<c>amber</c>).</summary>
	[JsonStringEnumMemberName("amber")]
	Amber = 2,

	/// <summary>TLP:AMBER+STRICT, for the recipient's organisation only (<c>amber+strict</c>).</summary>
	[JsonStringEnumMemberName("amber+strict")]
	AmberStrict = 3,

	/// <summary>TLP:GREEN, for the recipient's community (<c>green</c>).</summary>
	[JsonStringEnumMemberName("green")]
	Green = 4,

	/// <summary>TLP:CLEAR, without restriction (<c>clear</c>).</summary>
	[JsonStringEnumMemberName("clear")]
	Clear = 5,

	/// <summary>TLP:WHITE, the TLP 1.0 name for <see cref="Clear"/> (<c>white</c>), still found on older objects.</summary>
	[JsonStringEnumMemberName("white")]
	White = 6
}
