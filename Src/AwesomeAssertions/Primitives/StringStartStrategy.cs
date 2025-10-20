using System.Collections.Generic;
using AwesomeAssertions.Common;
using AwesomeAssertions.Common.Mismatch;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringStartStrategy : StringComparisonBaseStrategy, IStringComparisonStrategy
{
    private readonly IEqualityComparer<string> comparer;
    private readonly string predicateDescription;

    public StringStartStrategy(IEqualityComparer<string> comparer, string predicateDescription)
    {
        this.comparer = comparer;
        this.predicateDescription = predicateDescription;
    }

    protected override string ExpectationDescription => $"Expected {{context:string}} to {predicateDescription}";

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        if (!ValidateAgainstNulls(assertionChain, subject, expected))
        {
            return;
        }

        if (comparer.Equals(subject, expected))
        {
            return;
        }

        int indexOfMismatch = subject.IndexOfFirstMismatch(expected, comparer);

        if (indexOfMismatch < 0 || indexOfMismatch >= expected.Length)
        {
            return;
        }

        var failureMessage = MismatchRenderer.CreateFailureMessage(new MismatchRendererOptions
        {
            Subject = subject,
            Expected = expected,
            SubjectIndexOfMismatch = indexOfMismatch,
            ExpectedIndexOfMismatch = indexOfMismatch,
            ExpectationDescription = ExpectationDescription,
            MismatchLocationDescription = $"at index {indexOfMismatch}",
        });

        assertionChain.FailWith(failureMessage);
    }
}
