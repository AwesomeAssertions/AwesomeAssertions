using System.Collections.Generic;
using AwesomeAssertions.Common;
using AwesomeAssertions.Common.Mismatch;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringEqualityStrategy : IStringComparisonStrategy
{
    private readonly IEqualityComparer<string> comparer;
    private readonly string predicateDescription;

    public StringEqualityStrategy(IEqualityComparer<string> comparer, string predicateDescription)
    {
        this.comparer = comparer;
        this.predicateDescription = predicateDescription;
    }

    public string ExpectationDescription => $"Expected {{context:string}} to {predicateDescription} ";

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        if (comparer.Equals(subject, expected))
        {
            return;
        }

        var indexOfMismatch = subject.IndexOfFirstMismatch(expected, comparer);

        var failureMessage = MismatchRenderer.CreateFailureMessage(new MismatchRendererOptions
        {
            Subject = subject,
            Expected = expected,
            SubjectIndexOfMismatch = indexOfMismatch,
            ExpectedIndexOfMismatch = indexOfMismatch,
            ExpectationDescription = ExpectationDescription
        });

        assertionChain.FailWith(failureMessage);
    }
}
