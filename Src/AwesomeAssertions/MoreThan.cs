namespace AwesomeAssertions;

/// <summary>
/// Provides factory methods for <see cref="OccurrenceConstraint"/>s that require an event or item to
/// occur more than a given number of times.
/// </summary>
public static class MoreThan
{
    /// <summary>
    /// Requires the occurrence to happen more than one time.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is more than one.</returns>
    public static OccurrenceConstraint Once() => new MoreThanTimesConstraint(1);

    /// <summary>
    /// Requires the occurrence to happen more than two times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is more than two.</returns>
    public static OccurrenceConstraint Twice() => new MoreThanTimesConstraint(2);

    /// <summary>
    /// Requires the occurrence to happen more than three times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is more than three.</returns>
    public static OccurrenceConstraint Thrice() => new MoreThanTimesConstraint(3);

    /// <summary>
    /// Requires the occurrence to happen more than the specified number of times.
    /// </summary>
    /// <param name="expected">The number of times the occurrence must exceed.</param>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is more than <paramref name="expected"/>.</returns>
    public static OccurrenceConstraint Times(int expected) => new MoreThanTimesConstraint(expected);

    private sealed class MoreThanTimesConstraint : OccurrenceConstraint
    {
        internal MoreThanTimesConstraint(int expectedCount)
            : base(expectedCount)
        {
        }

        internal override string Mode => "more than";

        internal override bool Assert(int actual) => actual > ExpectedCount;
    }
}
