using System.Text.Json.Serialization;

namespace ThreatStream.Api.Models.Whitelist;

/// <summary>The kind of value an import whitelist entry holds (<c>value_type</c>).</summary>
public enum WhitelistValueType
{
	/// <summary>A kind this library does not know.</summary>
	Unknown = 0,

	/// <summary>An IP range in CIDR notation (<c>cidr</c>).</summary>
	[JsonStringEnumMemberName("cidr")]
	Cidr = 1,

	/// <summary>A domain, optionally with a leading <c>*</c> wildcard (<c>domain</c>).</summary>
	[JsonStringEnumMemberName("domain")]
	Domain = 2,

	/// <summary>An email address (<c>email</c>).</summary>
	[JsonStringEnumMemberName("email")]
	Email = 3,

	/// <summary>An IP address (<c>ip</c>).</summary>
	[JsonStringEnumMemberName("ip")]
	Ip = 4,

	/// <summary>An MD5 hash (<c>md5</c>).</summary>
	[JsonStringEnumMemberName("md5")]
	Md5 = 5,

	/// <summary>A URL (<c>url</c>).</summary>
	[JsonStringEnumMemberName("url")]
	Url = 6,

	/// <summary>An HTTP user agent (<c>user-agent</c>).</summary>
	[JsonStringEnumMemberName("user-agent")]
	UserAgent = 7
}
