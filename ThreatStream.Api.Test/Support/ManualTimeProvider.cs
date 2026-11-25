namespace ThreatStream.Api.Test.Support;

/// <summary>A clock that only moves when a test advances it.</summary>
internal sealed class ManualTimeProvider : TimeProvider
{
	private DateTimeOffset _now = new(2026, 11, 25, 12, 0, 0, TimeSpan.Zero);

	public override DateTimeOffset GetUtcNow() => _now;

	public void Advance(TimeSpan by) => _now += by;
}
