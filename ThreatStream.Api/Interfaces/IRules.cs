using Refit;
using ThreatStream.Api.Models;
using ThreatStream.Api.Models.Rules;

namespace ThreatStream.Api.Interfaces;

/// <summary>Keyword-match rules (<c>api/v1/rule/</c>). An organization can configure up to 300 rules.</summary>
public interface IRules
{
	/// <summary>Lists your organization's rules, most recently configured first (<c>GET api/v1/rule/</c>).</summary>
	/// <param name="options">Paging, ordering and (OnPrem) remote rules, or <see langword="null"/> for the 20 most recent local rules.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>One page of rules; page through them with <see cref="ThreatStreamClient.EnumerateAsync{T}"/>.</returns>
	/// <remarks>
	/// Not one of the endpoints with a documented rate limit, so ThreatStream's default for those applies: one request
	/// per minute per user.
	/// </remarks>
	[Get("api/v1/rule/")]
	Task<ThreatStreamPage<Rule>> ListAsync([Query] RuleListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a rule (<c>POST api/v1/rule/</c>).</summary>
	/// <param name="request">The rule: a name, keywords and at least one <c>Match...</c> flag.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The new rule, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>
	/// <b>Side effect:</b> the rule starts matching, tagging and associating new intelligence, and may notify users.
	/// Requires the permission to change rules. Refused by a read-only client.
	/// </remarks>
	[Post("api/v1/rule/")]
	Task<Rule?> CreateAsync([Body] RuleCreateRequest request, CancellationToken cancellationToken);

	/// <summary>Changes a rule (<c>PATCH api/v1/rule/{id}/</c>).</summary>
	/// <param name="id">The rule's ID.</param>
	/// <param name="request">The fields to change; the others are left as they are.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>The rule as changed, or <see langword="null"/> when ThreatStream answers without a body.</returns>
	/// <remarks>
	/// <para>
	/// <b>Side effect:</b> changes what the rule matches and applies. Requires the permission to change rules. Refused by a
	/// read-only client.
	/// </para>
	/// <para>
	/// The reference's method table lists <c>PATCH api/v1/rule/</c>, but its example patches <c>api/v1/rule/56/</c>, so this
	/// method addresses one rule by ID. The example's <c>-F tags: "[...]"</c> form body is malformed; this method sends
	/// JSON, as the reference's create example does.
	/// </para>
	/// </remarks>
	[Patch("api/v1/rule/{id}/")]
	Task<Rule?> UpdateAsync(long id, [Body] RuleUpdateRequest request, CancellationToken cancellationToken);

	/// <summary>Deletes a rule (<c>DELETE api/v1/rule/?id__in={id}</c>).</summary>
	/// <param name="id">The rule's ID.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	/// <returns>A task that completes when ThreatStream has accepted the deletion.</returns>
	/// <remarks>
	/// <para>
	/// <b>Side effect:</b> the rule is deleted permanently. Requires the permission to change rules. Refused by a
	/// read-only client.
	/// </para>
	/// <para>
	/// The reference's method table lists <c>DELETE api/v1/rule/</c>; its example selects the rule with an <c>id__in</c>
	/// filter on the collection, which this method sends with exactly one ID. It deliberately takes a single ID rather than
	/// a list: a collection DELETE whose list was empty would carry no filter and could delete every rule.
	/// </para>
	/// </remarks>
	[Delete("api/v1/rule/")]
	Task DeleteAsync([AliasAs("id__in")] long id, CancellationToken cancellationToken);
}
