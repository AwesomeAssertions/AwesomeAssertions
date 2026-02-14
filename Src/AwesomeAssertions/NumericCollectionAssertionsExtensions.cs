#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using AwesomeAssertions.Collections;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions;

/// <summary>
/// Extensions for asserting numerical collections.
/// </summary>
public static class NumericCollectionAssertionsExtensions
{
    /// <summary>
    /// Asserts a numerical enumerable to approximate all values of another enumerable as close as possible.
    /// </summary>
    /// <param name="parent">The <see cref="GenericCollectionAssertions{T}"/> object that is being extended.</param>
    /// <param name="expectation">
    /// The expected values to compare the actual value with.
    /// </param>
    /// <param name="precision">
    /// The maximum amount of which two values at the same index may differ.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <typeparam name="T">Numerical type</typeparam>
    public static AndConstraint<GenericCollectionAssertions<T>> EqualApproximately<T>(this GenericCollectionAssertions<T> parent,
        IEnumerable<T> expectation, T precision, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where T : INumber<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(precision);

        AssertionChain currentAssertionChain = parent.CurrentAssertionChain.BecauseOf(because, becauseArgs);
        AssertApproximateSubjectEquality(currentAssertionChain, parent.Subject, expectation, precision, new NumericApproximateComparer<T>(precision));

        return new AndConstraint<GenericCollectionAssertions<T>>(parent);
    }

    /// <summary>
    /// Expects the current numerical collection not to contain all the same elements in the same order as the collection identified by
    /// <paramref name="unexpected"/> within the specified <paramref name="precision"/>.
    /// </summary>
    /// <param name="parent">The <see cref="GenericCollectionAssertions{T}"/> object that is being extended.</param>
    /// <param name="unexpected">
    /// The expected values to compare the actual value with.
    /// </param>
    /// <param name="precision">
    /// The maximum amount of which two values at the same index may differ.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <typeparam name="T">Numerical type</typeparam>
    public static AndConstraint<GenericCollectionAssertions<T>> NotEqualApproximately<T>(this GenericCollectionAssertions<T> parent,
        IEnumerable<T> unexpected, T precision, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where T : INumber<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(precision);
        Guard.ThrowIfArgumentIsNull(unexpected, nameof(unexpected), "Cannot compare collection with <null>.");

        AssertionChain currentAssertionChain = parent.CurrentAssertionChain.BecauseOf(because, becauseArgs);
        AssertSubjectInequality(currentAssertionChain, parent.Subject, unexpected, precision, new NumericApproximateComparer<T>(precision));

        return new AndConstraint<GenericCollectionAssertions<T>>(parent);
    }

    // Same as <see cref="GenericCollectionAssertions{TCollection, T, TAssertions}.AssertSubjectEquality{TExpectation}(IEnumerable{TExpectation}, Func{T, TExpectation, bool}, string, object[])" />,
    // but with different messages.
    private static void AssertApproximateSubjectEquality<T>(AssertionChain assertionChain, IEnumerable<T> subject, IEnumerable<T> expectation,
        T precision, IEqualityComparer<T> comparer)
        where T : INumber<T>
    {
        if (subject is null && expectation is null)
        {
            return;
        }

        Guard.ThrowIfArgumentIsNull(expectation, nameof(expectation), "Cannot compare collection with <null>.");

        ICollection<T> expectedItems = expectation.ConvertOrCastToCollection();

        assertionChain
            .WithExpectation("Expected {context:collection} to approximate {0} ±{1}{reason}, ", expectedItems, precision, chain => chain
                .ForCondition(subject is not null)
                .FailWith("but found <null>.", expectedItems, precision)
                .Then
                .Given(() => subject.ConvertOrCastToCollection())
                .AssertCollectionsHaveSameCount(expectedItems.Count)
                .Then
                .Given(actual => new CollectionWithDifference<T>
                {
                    Items = actual,
                    Diff = actual.FirstDifferenceWith(expectedItems, comparer.Equals),
                })
                .ForCondition(collectionWithDiff => collectionWithDiff.Diff.HasDifference)
                .FailWith(GetDifferenceFailureDescription));
    }

    private static FailReason GetDifferenceFailureDescription<T>(CollectionWithDifference<T> collectionWithDiff)
        where T : INumber<T>
    {
        try
        {
            checked
            {
                T actual = collectionWithDiff.Diff.First;
                T expected = collectionWithDiff.Diff.Second;
                T difference = actual < expected ? expected - actual : actual - expected;
                return new FailReason("but {0} differed by {1} at index {2}.", collectionWithDiff.Items, difference, collectionWithDiff.Diff.Index);
            }
        }
        catch (OverflowException)
        {
            // If the difference is too large we (e.g. sbyte x = -127, sbyte y = 128), we cannot represent the difference.
            return new FailReason("but {0} differed at index {1}.", collectionWithDiff.Items, collectionWithDiff.Diff.Index);
        }
    }

    private static void AssertSubjectInequality<T>(AssertionChain assertionChain, IEnumerable<T> subject, IEnumerable<T> unexpected, T precision, IEqualityComparer<T> approximateComparer)
    {
        assertionChain
            .WithExpectation("Did not expect collections to be approximately equal within ±{0}{reason}, ", precision, chain => chain
                .Given(() => subject)
                .ForCondition(subject => subject is not null)
                .FailWith("but found <null>.")
                .Then
                .ForCondition(subject => !ReferenceEquals(subject, unexpected))
                .FailWith("but they both reference the same object."))
            .Then
            .Given(() => subject.ConvertOrCastToCollection())
            .ForCondition(actualItems => !actualItems.SequenceEqual(unexpected, approximateComparer))
            .FailWith("Did not expect collections {0} and {1} to be approximately equal within ±{2}{reason}.",
                _ => unexpected, actualItems => actualItems, _ => precision);
    }

    /// <summary>
    /// Item for passing a collection along with its first difference in a given assertion chain.
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    private sealed class CollectionWithDifference<T>
    {
        public required ICollection<T> Items { get; init; }

        public required CollectionDifferenceItem<T, T> Diff { get; init; }
    }

    /// <summary>
    /// Comparer for checking that two numerical values are approximately equal within a given precision.
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="precision">Given precision for approximate comparison</param>
    private sealed class NumericApproximateComparer<T>(T precision) : IEqualityComparer<T>
        where T : INumber<T>
    {
        public bool Equals(T x, T y)
        {
            // explicitly check for equality, because we consider two same values  of T.PositiveInfinity, T.NegativeInfinity, or T.NaN to be equal.
            if (x.Equals(y))
            {
                return true;
            }

            try
            {
                checked
                {
                    // we cannot use T.Abs(x - y) here, because that wouldn't work when the difference overflows (e.g. uint x = 0; uint y = 1)
                    T difference = x < y ? y - x : x - y;
                    return difference <= precision;
                }
            }
            catch (OverflowException)
            {
                // If the difference is too large (e.g. sbyte x = -127, sbyte y = 128), we cannot compare it with the precision.
                // In this case, we assume that the values are not approximately equal.
                return false;
            }
        }

        public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
    }
}
#endif
