using System;
using AwesomeAssertions.Common;

namespace AwesomeAssertions;

/// <summary>
/// Represents the base class for constraints that specify how many times an event or item is expected to occur,
/// such as those created by <see cref="Exactly"/>, <see cref="AtLeast"/>, <see cref="AtMost"/>,
/// <see cref="MoreThan"/> and <see cref="LessThan"/>.
/// </summary>
public abstract class OccurrenceConstraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OccurrenceConstraint"/> class.
    /// </summary>
    /// <param name="expectedCount">The number of times the occurrence is expected to happen.</param>
    protected OccurrenceConstraint(int expectedCount)
    {
        if (expectedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedCount), "Expected count cannot be negative.");
        }

        ExpectedCount = expectedCount;
    }

    internal int ExpectedCount { get; }

    internal abstract string Mode { get; }

    internal abstract bool Assert(int actual);

    internal void RegisterContextData(Action<string, object> register)
    {
        register("expectedOccurrence", $"{Mode} {ExpectedCount.Times()}");
    }
}
