using System;
using System.Linq;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Execution;

/// <summary>
/// Represents a chaining object returned from <see cref="AssertionChain"/> to continue the assertion using
/// an object returned by a selector.
/// </summary>
public class GivenSelector<T>
{
    private readonly AssertionChain assertionChain;
    private readonly T selector;

    internal GivenSelector(Func<T> selector, AssertionChain assertionChain)
    {
        this.assertionChain = assertionChain;

        this.selector = assertionChain.Succeeded ? selector() : default;
    }

    public bool Succeeded => assertionChain.Succeeded;

    /// <summary>
    /// Specify the condition that must be satisfied upon the subject selected through a prior selector.
    /// </summary>
    /// <param name="predicate">
    /// If <see langword="true"/> the assertion will be treated as successful and no exceptions will be thrown.
    /// </param>
    /// <remarks>
    /// The condition will not be evaluated if the prior assertion failed,
    /// nor will <see cref="FailWith(string,System.Func{T,object}[])"/> throw any exceptions.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public GivenSelector<T> ForCondition(Func<T, bool> predicate)
    {
        Guard.ThrowIfArgumentIsNull(predicate);

        if (assertionChain.Succeeded)
        {
            assertionChain.ForCondition(predicate(selector));
        }

        return this;
    }

    /// <summary>
    /// Allows combining one or more failing assertions using the other assertion methods that this library offers
    /// upon the subject selected through a prior selector.
    /// </summary>
    /// <remarks>
    /// This is only evaluated when all previous assertions in the chain have succeeded.
    /// </remarks>
    /// <param name="failingAssertion">The element inspector which must not be satisfied by the previously selected subject.</param>
    /// <returns>The current <see cref="AssertionChain"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failingAssertion"/> is <see langword="null"/>.</exception>
    public GivenSelector<T> NotSatisfy(Action<T> failingAssertion)
    {
        Guard.ThrowIfArgumentIsNull(failingAssertion);

        if (assertionChain.Succeeded)
        {
            assertionChain.NotSatisfy(() => failingAssertion(selector));
        }

        return this;
    }

    /// <summary>
    /// Allows combining one or more assertions using the other assertion methods that this library offers
    /// upon the subject selected through a prior selector.
    /// </summary>
    /// <remarks>
    /// This is only evaluated when all previous assertions in the chain have succeeded.
    /// </remarks>
    /// <param name="assertion">The element inspector which must be satisfied by the previously selected subject.</param>
    /// <returns>The current <see cref="AssertionChain"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    public GivenSelector<T> Satisfy(Action<T> assertion)
    {
        Guard.ThrowIfArgumentIsNull(assertion);

        if (assertionChain.Succeeded)
        {
            assertionChain.Satisfy(() => assertion(selector));
        }

        return this;
    }

    public GivenSelector<T> ForConstraint(OccurrenceConstraint constraint, Func<T, int> func)
    {
        Guard.ThrowIfArgumentIsNull(func);

        if (assertionChain.Succeeded)
        {
            assertionChain.ForConstraint(constraint, func(selector));
        }

        return this;
    }

    public GivenSelector<TOut> Given<TOut>(Func<T, TOut> selector)
    {
        Guard.ThrowIfArgumentIsNull(selector);

        return new GivenSelector<TOut>(() => selector(this.selector), assertionChain);
    }

    public ContinuationOfGiven<T> FailWith(string message)
    {
        return FailWith(message, Array.Empty<object>());
    }

    public ContinuationOfGiven<T> FailWith(string message, params Func<T, object>[] args)
    {
        if (assertionChain.PreviousAssertionSucceeded)
        {
            object[] mappedArguments = args.Select(a => a(selector)).ToArray();
            return FailWith(message, mappedArguments);
        }

        return new ContinuationOfGiven<T>(this);
    }

    public ContinuationOfGiven<T> FailWith(string message, params object[] args)
    {
        assertionChain.FailWith(message, args);
        return new ContinuationOfGiven<T>(this);
    }

    public ContinuationOfGiven<T> FailWith(Func<T, string> message)
    {
        assertionChain.FailWith(message(selector));
        return new ContinuationOfGiven<T>(this);
    }

    public ContinuationOfGiven<T> FailWith(Func<T, FailReason> failReason)
    {
        assertionChain.FailWith(() => failReason(selector));
        return new ContinuationOfGiven<T>(this);
    }
}
