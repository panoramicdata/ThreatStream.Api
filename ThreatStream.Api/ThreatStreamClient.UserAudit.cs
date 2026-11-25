using ThreatStream.Api.Interfaces;
using ThreatStream.Api.Models.UserAudit;

namespace ThreatStream.Api;

public sealed partial class ThreatStreamClient
{
	/// <summary>User interface activity reports (<c>api/v1/useraction/</c>).</summary>
	public IUserActions UserActions => field ??= For<IUserActions>();

	/// <summary>API activity reports (<c>api/v1/apiuseraction/</c>).</summary>
	public IApiUserActions ApiUserActions => field ??= For<IApiUserActions>();

	/// <summary>Asynchronous jobs and their results (<c>api/v1/generic_async_result/</c>).</summary>
	public IAsyncResults AsyncResults => field ??= For<IAsyncResults>();

	/// <summary>
	/// Starts an asynchronous job, polls it (<c>GET api/v1/generic_async_result/{id}/</c>) every
	/// <paramref name="pollInterval"/> until it is <c>done</c>, then downloads its result
	/// (<c>GET api/v1/generic_async_result/{id}/get_result/</c>).
	/// </summary>
	/// <param name="start">Starts the job, for example
	/// <c>ct =&gt; client.UserActions.StartReportAsync(options, ct)</c>.</param>
	/// <param name="pollInterval">How long to wait between polls. The job endpoint has no documented rate limit, so
	/// ThreatStream's default of one request per minute per user applies: a minute is a sensible interval.</param>
	/// <param name="cancellationToken">Cancels the wait; give it a timeout to bound how long a job may run.</param>
	/// <returns>The job's result as text: CSV or JSON for a user activity report.</returns>
	/// <exception cref="InvalidOperationException">The job failed: its status is <c>failed</c> or <c>error</c>, or its
	/// response status is 400 or higher.</exception>
	/// <remarks>Any status other than <c>done</c>, <c>failed</c> or <c>error</c> (such as <c>inprogress</c>) is polled again.</remarks>
	public async Task<string> RunAsyncJobAsync(
		Func<CancellationToken, Task<GenericAsyncResult>> start,
		TimeSpan pollInterval,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(start);
		ArgumentOutOfRangeException.ThrowIfLessThan(pollInterval, TimeSpan.Zero);
		var job = await start(cancellationToken).ConfigureAwait(false);
		while (!IsFinished(job))
		{
			await Task.Delay(pollInterval, cancellationToken).ConfigureAwait(false);
			job = await AsyncResults.GetAsync(job.Id, cancellationToken).ConfigureAwait(false);
		}

		return await AsyncResults.GetResultAsync(job.Id, cancellationToken).ConfigureAwait(false);
	}

	private static bool IsFinished(GenericAsyncResult job)
	{
		var status = job.Status?.Trim();
		if (HasStatus(status, "failed") || HasStatus(status, "error") || job.ResponseStatus >= 400)
		{
			throw new InvalidOperationException($"ThreatStream asynchronous job {job.Id} failed (status '{job.Status}', response status {job.ResponseStatus}).");
		}

		return HasStatus(status, "done");
	}

	private static bool HasStatus(string? status, string expected) => string.Equals(status, expected, StringComparison.OrdinalIgnoreCase);
}
