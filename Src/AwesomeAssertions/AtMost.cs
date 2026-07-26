namespace AwesomeAssertions;

/// <summary>
/// Provides factory methods for <see cref="OccurrenceConstraint"/>s that require an event or item to
/// occur at most a given number of times.
/// </summary>
public static class AtMost
{
    /// <summary>
    /// Requires the occurrence to happen at most one time.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at most one.</returns>
    public static OccurrenceConstraint Once() => new AtMostTimesConstraint(1);

    /// <summary>
    /// Requires the occurrence to happen at most two times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at most two.</returns>
    public static OccurrenceConstraint Twice() => new AtMostTimesConstraint(2);

    /// <summary>
    /// Requires the occurrence to happen at most three times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at most three.</returns>
    public static OccurrenceConstraint Thrice() => new AtMostTimesConstraint(3);

    /// <summary>
    /// Requires the occurrence to happen at most the specified number of times.
    /// </summary>
    /// <param name="expected">The maximum number of times the occurrence is expected to happen.</param>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at most <paramref name="expected"/>.</returns>
    public static OccurrenceConstraint Times(int expected) => new AtMostTimesConstraint(expected);

    private sealed class AtMostTimesConstraint : OccurrenceConstraint
    {
        internal AtMostTimesConstraint(int expectedCount)
            : base(expectedCount)
        {
        }

        internal override string Mode => "at most";

        internal override bool Assert(int actual) => actual <= ExpectedCount;
    }
}
