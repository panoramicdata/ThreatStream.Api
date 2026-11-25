namespace ThreatStream.Api;

public sealed partial class ThreatStreamClientOptions
{
	/// <inheritdoc />
	public override string ToString()
		=> $"ThreatStreamClientOptions {{ Region = {Region}, BaseUrl = {MaskUserInfo(BaseUrl)}, Username = {Username}, ApiKey = {Mask(ApiKey)}, ClientId = {ClientId}, ClientSecret = {Mask(ClientSecret)}, IsReadOnly = {IsReadOnly} }}";

	private static string Mask(string? secret) => string.IsNullOrEmpty(secret) ? "(none)" : "***";

	/// <summary>
	/// Masks any <c>user:password@</c> in a URL. Done on the text rather than a parsed <see cref="Uri"/>, since this must
	/// also redact a URL that fails to parse (such as one whose password contains an unescaped <c>@</c>).
	/// </summary>
	private static string MaskUserInfo(string? url)
	{
		if (url is null)
		{
			return string.Empty;
		}

		var start = url.IndexOf("://", StringComparison.Ordinal);
		if (start < 0)
		{
			return url;
		}

		start += 3;
		var end = url.IndexOfAny(['/', '?', '#'], start);
		var authority = end < 0 ? url[start..] : url[start..end];
		var at = authority.LastIndexOf('@');
		return at < 0 ? url : $"{url[..start]}***{url[(start + at)..]}";
	}
}
