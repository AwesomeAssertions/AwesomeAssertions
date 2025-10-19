using System.Collections.Generic;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringContainsStrategy : IStringComparisonStrategy
{
    private readonly IEqualityComparer<string> comparer;
    private readonly OccurrenceConstraint occurrenceConstraint;

    public StringContainsStrategy(IEqualityComparer<string> comparer, OccurrenceConstraint occurrenceConstraint)
    {
        this.comparer = comparer;
        this.occurrenceConstraint = occurrenceConstraint;
    }

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        int actual = subject.CountSubstring(expected, comparer);

        assertionChain
            .ForConstraint(occurrenceConstraint, actual)
            .FailWith(
                $"Expected {{context:string}} {{0}} to contain the equivalent of {{1}} {{expectedOccurrence}}{{reason}}, but found it {actual.Times()}.",
                subject, expected);
    }
}
