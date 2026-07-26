namespace AwesomeAssertions;

/// <summary>
/// Provides factory methods for <see cref="OccurrenceConstraint"/>s that require an event or item to
/// occur exactly a given number of times.
/// </summary>
public static class Exactly
{
    /// <summary>
    /// Requires the occurrence to happen exactly one time.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is exactly one.</returns>
    public static OccurrenceConstraint Once() => new ExactlyTimesConstraint(1);

    /// <summary>
    /// Requires the occurrence to happen exactly two times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is exactly two.</returns>
    public static OccurrenceConstraint Twice() => new ExactlyTimesConstraint(2);

    /// <summary>
    /// Requires the occurrence to happen exactly three times.
    /// </summary>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count is exactly three.</returns>
    public static OccurrenceConstraint Thrice() => new ExactlyTimesConstraint(3);

    /// <summary>
    /// Requires the occurrence to happen exactly the specified number of times.
    /// </summary>
    /// <param name="expected">The exact number of times the occurrence is expected to happen.</param>
    /// <returns>An <see cref="OccurrenceConstraint"/> that is satisfied when the actual count equals <paramref name="expected"/>.</returns>
    public static OccurrenceConstraint Times(int expected) => new ExactlyTimesConstraint(expected);

    private sealed class ExactlyTimesConstraint : OccurrenceConstraint
    {
        internal ExactlyTimesConstraint(int expectedCount)
            : base(expectedCount)
        {
        }

        internal override string Mode => "exactly";

        internal override bool Assert(int actual) => actual == ExpectedCount;
    }
}
