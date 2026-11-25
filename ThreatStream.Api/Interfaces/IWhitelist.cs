using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Whitelist;

namespace ThreatStream.Api.Interfaces;

/// <summary>
/// Your organization's import whitelist (<c>api/v1/orgwhitelist/</c>): values that imports never turn into observables.
/// </summary>
/// <remarks>
/// The reference's method table lists POST, PATCH and DELETE on <c>api/v1/orgwhitelist/</c> itself, but its examples
/// add entries through <c>bulk/</c> or <c>upload/</c>, and change or delete one entry at <c>{id}/</c>; these methods
/// follow the examples. Every change is a side effect on what your organization imports, and is refused by a read-only
/// client. None of these endpoints has a documented rate limit, so ThreatStream's default applies: one request per minute
/// per user.
/// </remarks>
public interface IWhitelist
{
	/// <summary>Lists whitelist entries, most recently added first (<c>GET api/v1/orgwhitelist/?format=json</c>).</summary>
	/// <param name="options">Paging and notes, or <see langword="null"/> for the 10 most recent entries with notes.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of entries; page through them with <see cref="ThreatStreamClient.EnumerateAsync{T}"/>.</returns>
	[Get("api/v1/orgwhitelist/?format=json")]
	Task<ThreatStreamPage<WhitelistEntry>> ListAsync([Query] WhitelistListOptions? options, CancellationToken cancellationToken);

	/// <summary>Exports whitelist entries as CSV text (<c>GET api/v1/orgwhitelist/?format=csv</c>).</summary>
	/// <param name="options">Paging and notes, or <see langword="null"/> for ThreatStream's defaults.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The CSV text as ThreatStream sends it.</returns>
	[Get("api/v1/orgwhitelist/?format=csv")]
	Task<string> ExportCsvAsync([Query] WhitelistListOptions? options, CancellationToken cancellationToken);

	/// <summary>Adds entries to the whitelist (<c>POST api/v1/orgwhitelist/bulk/</c>).</summary>
	/// <param name="request">The entries to add.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has accepted the entries.</returns>
	/// <remarks>
	/// <b>Side effect:</b> later imports skip the values. Refused by a read-only client. The reference documents no
	/// response body.
	/// </remarks>
	[Post("api/v1/orgwhitelist/bulk/")]
	Task AddAsync([Body] WhitelistAddRequest request, CancellationToken cancellationToken);

	/// <summary>Adds the entries of a CSV file to the whitelist, or replaces the whitelist with them (<c>POST api/v1/orgwhitelist/upload/</c>).</summary>
	/// <param name="request">The CSV file, sent as <c>multipart/form-data</c>.</param>
	/// <param name="removeExisting">
	/// <see langword="false"/> to add the file's entries; <see langword="true"/> to <b>delete every existing entry</b> and
	/// replace the whole whitelist with the file's (<c>remove_existing</c>).
	/// </param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has accepted the file.</returns>
	/// <remarks>
	/// <b>Side effect:</b> with <paramref name="removeExisting"/> <see langword="true"/> this is destructive. Refused by a
	/// read-only client. As in the reference's examples, <c>remove_existing</c> travels in the query string rather than the
	/// form. The reference documents no response body.
	/// </remarks>
	[Post("api/v1/orgwhitelist/upload/")]
	Task UploadAsync([Body] WhitelistUploadRequest request, [AliasAs("remove_existing")] bool removeExisting, CancellationToken cancellationToken);

	/// <summary>Changes the note on a whitelist entry (<c>PATCH api/v1/orgwhitelist/{id}/</c>).</summary>
	/// <param name="entryId">The entry's ID, from <see cref="ListAsync"/>.</param>
	/// <param name="request">The new note.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The entry as changed, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks><b>Side effect:</b> changes the entry. Refused by a read-only client.</remarks>
	[Patch("api/v1/orgwhitelist/{entryId}/")]
	Task<WhitelistEntry?> UpdateAsync(long entryId, [Body] WhitelistUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Removes an entry from the whitelist (<c>DELETE api/v1/orgwhitelist/{id}/</c>).</summary>
	/// <param name="entryId">The entry's ID, from <see cref="ListAsync"/>.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has removed the entry.</returns>
	/// <remarks><b>Side effect:</b> later imports no longer skip the value. Refused by a read-only client.</remarks>
	[Delete("api/v1/orgwhitelist/{entryId}/")]
	Task DeleteAsync(long entryId, CancellationToken cancellationToken);
}
