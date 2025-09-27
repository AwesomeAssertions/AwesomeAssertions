using System;
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
        if (expected.Length is 0 || comparer.Equals(subject, expected))
        {
            return;
        }

        var indexes = IndexOfLastMismatch(subject, expected, comparer);

        bool hasMismatch = expected.Length > subject.Length || indexes.index2 >= 0;

        if (!hasMismatch)
        {
            return;
        }

        var failureMessage = MismatchRenderer.CreateFailureMessage(new MismatchRendererOptions
        {
            Subject = subject,
            Expected = expected,
            SubjectIndexOfMismatch = Math.Max(indexes.index1, 0),
            ExpectedIndexOfMismatch = Math.Max(indexes.index2, 0),
            ExpectationDescription = ExpectationDescription,
            IndexFormatter = _ => $"before index {Math.Max(indexes.index1, indexes.index2)}",
            AlignRight = true
        });

        assertionChain.FailWith(failureMessage);
    }

    /// <summary>
    /// Finds the last index at which the <paramref name="string1"/> does not match the <paramref name="string2"/>
    /// string anymore, accounting for the specified <paramref name="comparer"/>.
    /// </summary>
    /// <returns>
    /// The mismatch indexes for the subject and the expected, or (-1 -1) if no mismatch is found.
    /// </returns>
    private static (int index1, int index2) IndexOfLastMismatch(
        string string1,
        string string2,
        IEqualityComparer<string> comparer)
    {
        var index1 = string1.Length - 1;
        var index2 = string2.Length - 1;

        while (index1 >= 0 && index2 >= 0)
        {
            if (!comparer.Equals(string1[index1..(index1 + 1)], string2[index2..(index2 + 1)]))
            {
                return (index1, index2);
            }

            index1--;
            index2--;
        }

        return (index1, index2);
    }
}
