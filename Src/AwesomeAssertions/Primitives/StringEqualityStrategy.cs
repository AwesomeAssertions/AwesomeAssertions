using System;
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

        if (ValidateAgainstSuperfluousWhitespace(assertionChain, subject, expected))
        {
            return;
        }

        int indexOfMismatch = GetIndexOfFirstMismatch(subject, expected);

        assertionChain
            .FailWith(() => new FailReason(MismatchRenderer.CreateFailureMessage(ExpectationDescription, subject, expected, indexOfMismatch)));
    }

    private bool ValidateAgainstSuperfluousWhitespace(AssertionChain assertion, string subject, string expected)
    {
        assertion
            .ForCondition(!(expected.Length > subject.Length && comparer.Equals(expected.TrimEnd(), subject)))
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but it misses some extra whitespace at the end.", expected)
            .Then
            .ForCondition(!(subject.Length > expected.Length && comparer.Equals(subject.TrimEnd(), expected)))
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but it has unexpected whitespace at the end.", expected);

        return !assertion.Succeeded;
    }

    /// <summary>
    /// Get index of the first mismatch between <paramref name="subject"/> and <paramref name="expected"/>.
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="expected"></param>
    /// <returns>Returns the index of the first mismatch, or -1 if the strings are equal.</returns>
    private int GetIndexOfFirstMismatch(string subject, string expected)
    {
        int indexOfMismatch = subject.IndexOfFirstMismatch(expected, comparer);

        if (indexOfMismatch != -1)
        {
            return indexOfMismatch;
        }

        // the mismatch is the first character of the longer string.
        return Math.Min(subject.Length, expected.Length);
    }
}
