using System.Collections.Generic;
using AwesomeAssertions.Common.Mismatch;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringEndStrategy : IStringComparisonStrategy
{
    private readonly IEqualityComparer<string> comparer;
    private readonly string predicateDescription;

    public StringEndStrategy(IEqualityComparer<string> comparer, string predicateDescription)
    {
        this.comparer = comparer;
        this.predicateDescription = predicateDescription;
    }

    public string ExpectationDescription => $"Expected {{context:string}} to {predicateDescription} ";

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        assertionChain
            .ForCondition(subject!.Length >= expected.Length)
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but {{1}} is too short.", expected, subject);

        if (!assertionChain.Succeeded)
        {
            return;
        }

        var (subjectIndexOfMismatch, expectedIndexOfMismatch) = IndexOfLastMismatch(subject, expected, comparer);
        if (subjectIndexOfMismatch < 0)
        {
            return;
        }

        var failureMessage = MismatchRenderer.CreateFailureMessage(new MismatchRendererOptions
        {
            Subject = subject,
            Expected = expected,
            SubjectIndexOfMismatch = subjectIndexOfMismatch,
            ExpectedIndexOfMismatch = expectedIndexOfMismatch,
            ExpectationDescription = ExpectationDescription,
            IndexFormatter = index => $"before index {index}",
            AlignRight = true,
            TruncationStrategy = new InverseTruncationBasisDecorator(new StandardTruncationStrategy()),
        });

        assertionChain.FailWith(failureMessage);
    }

    /// <summary>
    /// Finds the last index at which the <paramref name="subject"/> does not match the <paramref name="expected"/>
    /// string anymore, accounting for the specified <paramref name="comparer"/>.
    /// </summary>
    /// <returns>
    /// The mismatch indexes for the subject and the expected, or (-1 -1) if no mismatch is found.
    /// </returns>
    private static (int subjectIndex, int expectedIndex) IndexOfLastMismatch(string subject, string expected, IEqualityComparer<string> comparer)
    {
        var subjectIndex = subject.Length - 1;
        var expectedIndex = expected.Length - 1;
        while (subjectIndex >= 0 && expectedIndex >= 0)
        {
            if (!comparer.Equals(subject[subjectIndex..(subjectIndex + 1)], expected[expectedIndex..(expectedIndex + 1)]))
            {
                return (subjectIndex, expectedIndex);
            }

            subjectIndex--;
            expectedIndex--;
        }

        return (-1, -1);
    }
}
