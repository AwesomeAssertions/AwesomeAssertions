using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Collections;

[DebuggerNonUserCode]
public class SubsequentOrderingAssertions<T>
    : GenericCollectionAssertions<IEnumerable<T>, T>
{
    private readonly IOrderedEnumerable<T> previousOrderedEnumerable;
    private bool subsequentOrdering;

    public SubsequentOrderingAssertions(IEnumerable<T> actualValue, IOrderedEnumerable<T> previousOrderedEnumerable, AssertionChain assertionChain)
        : base(actualValue, assertionChain)
    {
        this.previousOrderedEnumerable = previousOrderedEnumerable;
    }

    /// <summary>
    /// Asserts that a subsequence is ordered in ascending order according to the value of the specified
    /// <paramref name="propertyExpression"/>.
    /// </summary>
    /// <param name="propertyExpression">
    /// A lambda expression that references the property that should be used to determine the expected ordering.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <remarks>
    /// Empty and single element collections are considered to be ordered both in ascending and descending order at the same time.
    /// </remarks>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInAscendingOrder<TSelector>(
        Expression<Func<T, TSelector>> propertyExpression,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return ThenBeInAscendingOrder(propertyExpression, GetComparer<TSelector>(), because, becauseArgs);
    }

    /// <summary>
    /// Asserts that a subsequence is ordered in ascending order according to the value of the specified
    /// <paramref name="propertyExpression"/> and <see cref="IComparer{T}"/> implementation.
    /// </summary>
    /// <param name="propertyExpression">
    /// A lambda expression that references the property that should be used to determine the expected ordering.
    /// </param>
    /// <param name="comparer">
    /// The object that should be used to determine the expected ordering.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <remarks>
    /// Empty and single element collections are considered to be ordered both in ascending and descending order at the same time.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInAscendingOrder<TSelector>(
        Expression<Func<T, TSelector>> propertyExpression, IComparer<TSelector> comparer,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNull(comparer, nameof(comparer),
            "Cannot assert collection ordering without specifying a comparer.");

        return ThenBeOrderedBy(propertyExpression, comparer, SortOrder.Ascending, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that a subsequence is ordered in descending order according to the value of the specified
    /// <paramref name="propertyExpression"/>.
    /// </summary>
    /// <param name="propertyExpression">
    /// A lambda expression that references the property that should be used to determine the expected ordering.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <remarks>
    /// Empty and single element collections are considered to be ordered both in ascending and descending order at the same time.
    /// </remarks>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInDescendingOrder<TSelector>(
        Expression<Func<T, TSelector>> propertyExpression,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return ThenBeInDescendingOrder(propertyExpression, GetComparer<TSelector>(), because, becauseArgs);
    }

    /// <summary>
    /// Asserts that a subsequence is ordered in descending order according to the value of the specified
    /// <paramref name="propertyExpression"/> and <see cref="IComparer{T}"/> implementation.
    /// </summary>
    /// <param name="propertyExpression">
    /// A lambda expression that references the property that should be used to determine the expected ordering.
    /// </param>
    /// <param name="comparer">
    /// The object that should be used to determine the expected ordering.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <remarks>
    /// Empty and single element collections are considered to be ordered both in ascending and descending order at the same time.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInDescendingOrder<TSelector>(
        Expression<Func<T, TSelector>> propertyExpression, IComparer<TSelector> comparer,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNull(comparer, nameof(comparer),
            "Cannot assert collection ordering without specifying a comparer.");

        return ThenBeOrderedBy(propertyExpression, comparer, SortOrder.Descending, because, becauseArgs);
    }

    private AndConstraint<SubsequentOrderingAssertions<T>> ThenBeOrderedBy<TSelector>(
        Expression<Func<T, TSelector>> propertyExpression,
        IComparer<TSelector> comparer,
        SortOrder direction,
        [StringSyntax("CompositeFormat")] string because,
        object[] becauseArgs)
    {
        subsequentOrdering = true;
        return BeOrderedBy(propertyExpression, comparer, direction, because, becauseArgs);
    }

    internal sealed override IOrderedEnumerable<T> GetOrderedEnumerable<TSelector>(
        Expression<Func<T, TSelector>> propertyExpression,
        IComparer<TSelector> comparer,
        SortOrder direction,
        ICollection<T> unordered)
    {
        if (subsequentOrdering)
        {
            Func<T, TSelector> keySelector = propertyExpression.Compile();

            IOrderedEnumerable<T> expectation = direction == SortOrder.Ascending
                ? previousOrderedEnumerable.ThenBy(keySelector, comparer)
                : previousOrderedEnumerable.ThenByDescending(keySelector, comparer);

            return expectation;
        }

        return base.GetOrderedEnumerable(propertyExpression, comparer, direction, unordered);
    }
}
