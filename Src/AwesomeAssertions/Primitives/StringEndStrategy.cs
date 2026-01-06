using System;
using System.Collections.Generic;
using AwesomeAssertions.Common.Mismatch;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringEndStrategy : StringComparisonBaseStrategy, IStringComparisonStrategy
{
    private readonly IEqualityComparer<string> comparer;
    private readonly string predicateDescription;

    public StringEndStrategy(IEqualityComparer<string> comparer, string predicateDescription)
    {
        this.comparer = comparer;
        this.predicateDescription = predicateDescription;
    }

    protected override string ExpectationDescription => $"Expected {{context:string}} to {predicateDescription} ";

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        if (!ValidateAgainstNulls(assertionChain, subject, expected))
        {
            return;
        }

        var (mismatchInSubject, mismatchInExpectation) = IndexOfLastMismatch(subject, expected, comparer);

        // IndexOfLastMismatch returns -1 for expected index when no mismatch is found.
        if (mismatchInExpectation < 0)
        {
            return;
        }

        var failureMessage = MismatchRenderer.CreateFailureMessage(new MismatchRendererOptions
        {
            Subject = subject,
            Expected = expected,
            SubjectIndexOfMismatch = Math.Max(mismatchInSubject, 0), // We need to clamp the subject index to 0 to make it a valid index (i.e., ABC, 00ABC).
            ExpectedIndexOfMismatch = mismatchInExpectation,
            ExpectationDescription = ExpectationDescription,
            MismatchLocationDescription = $"before index {mismatchInSubject + 1}", // We always base the index on the subject. Since we are indicating that the mismatch occurs before this index, we need to offset it by 1.
            Alignment = Alignment.Right,
            StringPrintLength = assertionChain.FormattingOptions.StringPrintLength,
        });

        assertionChain.FailWith(failureMessage);
    }

    /// <summary>
    /// Finds the last index at which the <paramref name="subject"/> does not match the <paramref name="expected"/>
    /// string anymore, accounting for the specified <paramref name="comparer"/>.
    /// </summary>
    /// <returns>
    /// The mismatch indexes for the subject and the expected. No mismatch exists if the expected index is -1.
    /// </returns>
    private static (int mismatchInSubject, int mismatchInExpected) IndexOfLastMismatch(
        string subject,
        string expected,
        IEqualityComparer<string> comparer)
    {
        // We can't have a mismatch if the expectation is empty.
        if (expected.Length is 0 || comparer.Equals(subject, expected))
        {
            return (-1, -1);
        }

        var subjectIndex = subject.Length - 1;
        var expectedIndex = expected.Length - 1;

        // Walk backwards through both strings until we find a mismatch.
        while (subjectIndex >= 0 && expectedIndex >= 0)
        {
            if (!comparer.Equals(subject[subjectIndex..(subjectIndex + 1)], expected[expectedIndex..(expectedIndex + 1)]))
            {
                return (subjectIndex, expectedIndex);
            }

            subjectIndex--;
            expectedIndex--;
        }

        // If we reach here, one string is longer than the other and one of the indexes is -1.
        // At this point, a mismatch would only occur if the expectation was the longer string (the subject index would be -1).
        if (expectedIndex is -1)
        {
            return (-1, -1);
        }

        // The subject index should be -1 and expected index should be > -1. We need to return the subject index as is for offsetting to work correctly.
        // Therefore, the result can contain (-1, >=0).
        return (subjectIndex, expectedIndex);
    }
}
