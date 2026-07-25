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

    /// <summary>
    /// Gets a value indicating whether the previous assertion in the chain was successful.
    /// </summary>
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
    /// Specifies that the occurrence <paramref name="constraint"/> must satisfy the number
    /// (provided by <paramref name="func"/>) for the assertion to succeed.
    /// </summary>
    /// <param name="constraint">
    /// The occurrence constraint (such as those produced by <c>AtLeast</c>, <c>AtMost</c> or <c>Exactly</c>) to evaluate.
    /// </param>
    /// <param name="func">
    /// A function that returns the number of occurrences found for the selected subject.
    /// </param>
    /// <remarks>
    /// The constraint will not be evaluated if the prior assertion failed,
    /// nor will <see cref="FailWith(string,System.Func{T,object}[])"/> throw any exceptions.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public GivenSelector<T> ForConstraint(OccurrenceConstraint constraint, Func<T, int> func)
    {
        Guard.ThrowIfArgumentIsNull(func);

        if (assertionChain.Succeeded)
        {
            assertionChain.ForConstraint(constraint, func(selector));
        }

        return this;
    }

    /// <summary>
    /// Allows continuing the assertion on the object returned by <paramref name="selector"/>, which is only invoked when
    /// the previous assertion in the chain succeeded.
    /// </summary>
    /// <param name="selector">A function that returns the object on which the continued assertion is executed.</param>
    /// <returns>
    /// A <see cref="GivenSelector{TOut}"/> that can be used to continue the assertion on the selected object.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public GivenSelector<TOut> Given<TOut>(Func<T, TOut> selector)
    {
        Guard.ThrowIfArgumentIsNull(selector);

        return new GivenSelector<TOut>(() => selector(this.selector), assertionChain);
    }

    /// <summary>
    /// Records a failure with the specified <paramref name="message"/> when the condition specified through
    /// <see cref="ForCondition"/> was not met.
    /// </summary>
    /// <param name="message">
    /// The failure message. May contain specialized placeholders such as <em>{reason}</em> and <em>{context}</em>.
    /// </param>
    public ContinuationOfGiven<T> FailWith(string message)
    {
        return FailWith(message, Array.Empty<object>());
    }

    /// <summary>
    /// Records a failure with the specified <paramref name="message"/> when the condition specified through
    /// <see cref="ForCondition"/> was not met.
    /// </summary>
    /// <param name="message">
    /// The failure message. May contain numbered <see cref="string.Format(string,object[])"/>-style placeholders as well
    /// as specialized placeholders.
    /// </param>
    /// <param name="args">
    /// Zero or more functions that provide the objects to format using the placeholders in <paramref name="message"/>,
    /// based on the selected subject. Each function is only invoked when the assertion actually failed.
    /// </param>
    public ContinuationOfGiven<T> FailWith(string message, params Func<T, object>[] args)
    {
        assertionChain.FailWith(() => new FailReason(
            message,
            args.Select(a => a(selector)).ToArray()));
        return new ContinuationOfGiven<T>(this);
    }

    /// <summary>
    /// Records a failure with the specified <paramref name="message"/> when the condition specified through
    /// <see cref="ForCondition"/> was not met.
    /// </summary>
    /// <param name="message">
    /// The failure message. May contain numbered <see cref="string.Format(string,object[])"/>-style placeholders as well
    /// as specialized placeholders.
    /// </param>
    /// <param name="args">
    /// Zero or more objects to format using the placeholders in <paramref name="message"/>.
    /// </param>
    public ContinuationOfGiven<T> FailWith(string message, params object[] args)
    {
        assertionChain.FailWith(message, args);
        return new ContinuationOfGiven<T>(this);
    }

    /// <summary>
    /// Records a failure with the message produced by <paramref name="message"/> for the selected subject when the
    /// condition specified through <see cref="ForCondition"/> was not met.
    /// </summary>
    /// <param name="message">
    /// A function that produces the failure message for the selected subject. It is only invoked when the assertion
    /// actually failed.
    /// </param>
    public ContinuationOfGiven<T> FailWith(Func<T, string> message)
    {
        assertionChain.FailWith(message(selector));
        return new ContinuationOfGiven<T>(this);
    }

    /// <summary>
    /// Records a failure using the <see cref="FailReason"/> produced by <paramref name="failReason"/> for the selected
    /// subject when the condition specified through <see cref="ForCondition"/> was not met.
    /// </summary>
    /// <param name="failReason">
    /// A function that produces the <see cref="FailReason"/> describing the failure for the selected subject. It is only
    /// invoked when the assertion actually failed.
    /// </param>
    public ContinuationOfGiven<T> FailWith(Func<T, FailReason> failReason)
    {
        assertionChain.FailWith(() => failReason(selector));
        return new ContinuationOfGiven<T>(this);
    }
}
