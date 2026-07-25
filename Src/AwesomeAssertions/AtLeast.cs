namespace AwesomeAssertions;

/// <summary>
/// Provides factory methods for <see cref="OccurrenceConstraint"/>s that require an event or item to
/// occur at least a given number of times.
/// </summary>
public static class AtLeast
{
    /// <summary>
    /// Requires the occurrence to happen at least one time.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at least one.</returns>
    public static OccurrenceConstraint Once() => new AtLeastTimesConstraint(1);

    /// <summary>
    /// Requires the occurrence to happen at least two times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at least two.</returns>
    public static OccurrenceConstraint Twice() => new AtLeastTimesConstraint(2);

    /// <summary>
    /// Requires the occurrence to happen at least three times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at least three.</returns>
    public static OccurrenceConstraint Thrice() => new AtLeastTimesConstraint(3);

    /// <summary>
    /// Requires the occurrence to happen at least the specified number of times.
    /// </summary>
    /// <param name="expected">The minimum number of times the occurrence is expected to happen.</param>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is at least <paramref name="expected"/>.</returns>
    public static OccurrenceConstraint Times(int expected) => new AtLeastTimesConstraint(expected);

    private sealed class AtLeastTimesConstraint : OccurrenceConstraint
    {
        internal AtLeastTimesConstraint(int expectedCount)
            : base(expectedCount)
        {
        }

        internal override string Mode => "at least";

        internal override bool Assert(int actual) => actual >= ExpectedCount;
    }
}
