using System.Runtime.CompilerServices;
using System.Text.Json;
using ThreatStream.Api.Models;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>
	/// Enumerates every object of a list operation: the first page from <paramref name="firstPage"/>, then each page its
	/// <c>meta.next</c> link names, until a page has no next link or no objects. Pages are fetched as the enumeration
	/// proceeds.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="firstPage">Fetches the first page, for example <c>ct =&gt; client.Feeds.ListAsync(options, ct)</c>.
	/// Its options (filters, <c>limit</c>, <c>order_by</c>) carry over, since ThreatStream's next links repeat them.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>Every object, page by page.</returns>
	/// <remarks>
	/// <para>
	/// ThreatStream's <c>meta.next</c> is a link relative to the host (<c>/api/v2/...</c>). It is resolved against
	/// <see cref="BaseAddress"/>; when the base address has a path prefix the link lacks, the prefix is added. A link to
	/// any other scheme, host or port is refused with <see cref="InvalidOperationException"/>, so credentials are never sent
	/// elsewhere; so is a next link that repeats, which would loop forever.
	/// </para>
	/// <para>
	/// Offset paging becomes slow and is capped for very large observable result sets (over 10,000); use
	/// <see cref="EnumerateByUpdateIdAsync{T}"/> for those.
	/// </para>
	/// </remarks>
	public async IAsyncEnumerable<T> EnumerateAsync<T>(
		Func<CancellationToken, Task<ThreatStreamPage<T>>> firstPage,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(firstPage);
		var page = await firstPage(cancellationToken).ConfigureAwait(false);
		var followed = new HashSet<string>(StringComparer.Ordinal);
		while (true)
		{
			foreach (var item in page.Objects)
			{
				yield return item;
			}

			var next = page.Meta?.Next;
			if (string.IsNullOrWhiteSpace(next) || page.Objects.Count == 0)
			{
				yield break;
			}

			if (!followed.Add(next))
			{
				throw new InvalidOperationException("ThreatStream returned a next page link that was already followed; stopping to avoid a loop.");
			}

			page = await GetPageAsync<T>(ResolveNext(next), cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Enumerates a large observable result set by <c>update_id</c>, as ThreatStream recommends above 10,000 results:
	/// <paramref name="getPage"/> is called with the highest <c>update_id</c> seen so far (starting at
	/// <paramref name="afterUpdateId"/>) and must query <c>update_id__gt</c> that value with <c>order_by=update_id</c>
	/// (inside a <c>q</c> expression, <c>update_id&gt;N</c>). Stops at the first empty page.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="getPage">Fetches the page of objects whose <c>update_id</c> is greater than the value given.</param>
	/// <param name="updateIdOf">Reads an object's <c>update_id</c>.</param>
	/// <param name="afterUpdateId">Where to start: 0 for everything, or the last <c>update_id</c> of a previous sync.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>Every object, page by page.</returns>
	/// <exception cref="InvalidOperationException">A page did not advance past the previous <c>update_id</c>, which means the
	/// query ignores it and would repeat forever.</exception>
	/// <remarks>For a continuous sync, also bound the query with <c>modified_ts__lt</c> set to a minute ago.</remarks>
	public static async IAsyncEnumerable<T> EnumerateByUpdateIdAsync<T>(
		Func<long, CancellationToken, Task<ThreatStreamPage<T>>> getPage,
		Func<T, long> updateIdOf,
		long afterUpdateId,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(getPage);
		ArgumentNullException.ThrowIfNull(updateIdOf);
		var last = afterUpdateId;
		while (true)
		{
			var page = await getPage(last, cancellationToken).ConfigureAwait(false);
			if (page.Objects.Count == 0)
			{
				yield break;
			}

			var highest = page.Objects.Max(updateIdOf);
			if (highest <= last)
			{
				throw new InvalidOperationException($"The page after update_id {last} did not advance; the query must filter on update_id__gt and order by update_id.");
			}

			foreach (var item in page.Objects)
			{
				yield return item;
			}

			last = highest;
		}
	}

	/// <summary>Resolves a <c>meta.next</c> link against <see cref="BaseAddress"/>, refusing any other origin.</summary>
	internal Uri ResolveNext(string next)
	{
		var prefix = BaseAddress.AbsolutePath.TrimEnd('/');
		var link = next.StartsWith('/') && !next.StartsWith("//", StringComparison.Ordinal) && prefix.Length > 0
			&& !next.StartsWith(prefix + "/", StringComparison.Ordinal)
				? prefix + next
				: next;
		return Uri.TryCreate(BaseAddress, link, out var uri)
			&& uri.Scheme == BaseAddress.Scheme
			&& string.Equals(uri.Authority, BaseAddress.Authority, StringComparison.OrdinalIgnoreCase)
				? uri
				: throw new InvalidOperationException("ThreatStream returned a next page link outside the client's base address; it was not followed.");
	}

	private async Task<ThreatStreamPage<T>> GetPageAsync<T>(Uri uri, CancellationToken cancellationToken)
	{
		using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
		if (await ThreatStreamErrorMapper.CreateAsync(response).ConfigureAwait(false) is { } error)
		{
			throw error;
		}

		var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
		await using (stream.ConfigureAwait(false))
		{
			return await JsonSerializer.DeserializeAsync<ThreatStreamPage<T>>(stream, ThreatStreamJson.Options, cancellationToken).ConfigureAwait(false)
				?? throw new InvalidOperationException("ThreatStream returned an empty page.");
		}
	}
}
