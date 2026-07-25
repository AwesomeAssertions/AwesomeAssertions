namespace AwesomeAssertions;

/// <summary>
/// Provides factory methods for <see cref="OccurrenceConstraint"/>s that require an event or item to
/// occur less than a given number of times.
/// </summary>
public static class LessThan
{
    /// <summary>
    /// Requires the occurrence to happen less than two times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is less than two.</returns>
    public static OccurrenceConstraint Twice() => new LessThanTimesConstraint(2);

    /// <summary>
    /// Requires the occurrence to happen less than three times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is less than three.</returns>
    public static OccurrenceConstraint Thrice() => new LessThanTimesConstraint(3);

    /// <summary>
    /// Requires the occurrence to happen less than the specified number of times.
    /// </summary>
    /// <param name="expected">The number of times the occurrence must stay below.</param>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is less than <paramref name="expected"/>.</returns>
    public static OccurrenceConstraint Times(int expected) => new LessThanTimesConstraint(expected);

    private sealed class LessThanTimesConstraint : OccurrenceConstraint
    {
        internal LessThanTimesConstraint(int expectedCount)
            : base(expectedCount)
        {
        }

        internal override string Mode => "less than";

        internal override bool Assert(int actual) => actual < ExpectedCount;
    }
}
