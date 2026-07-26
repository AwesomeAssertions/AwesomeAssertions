using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Specialized;

/// <summary>
/// Contains a number of methods to assert that a synchronous function yields the expected result.
/// </summary>
[DebuggerNonUserCode]
public class FunctionAssertions<T> : DelegateAssertions<Func<T>, FunctionAssertions<T>>
{
    private readonly AssertionChain assertionChain;

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionAssertions{T}"/> class.
    /// </summary>
    /// <param name="subject">The <see cref="Func{T}"/> to assert on.</param>
    /// <param name="extractor">The strategy used to extract exceptions of a specific type from a thrown exception.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion and is used to report failures.
    /// </param>
    public FunctionAssertions(Func<T> subject, IExtractExceptions extractor, AssertionChain assertionChain)
        : base(subject, extractor, assertionChain)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionAssertions{T}"/> class.
    /// </summary>
    /// <param name="subject">The <see cref="Func{T}"/> to assert on.</param>
    /// <param name="extractor">The strategy used to extract exceptions of a specific type from a thrown exception.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion and is used to report failures.
    /// </param>
    /// <param name="clock">The clock used to measure elapsed time.</param>
    public FunctionAssertions(Func<T> subject, IExtractExceptions extractor, AssertionChain assertionChain, IClock clock)
        : base(subject, extractor, assertionChain, clock)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Invokes the current <see cref="Func{T}"/> subject.
    /// </summary>
    protected override void InvokeSubject()
    {
        Subject();
    }

    /// <inheritdoc />
    protected override string Identifier => "function";

    /// <summary>
    /// Asserts that the current <see cref="Func{T}"/> does not throw any exception.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndWhichConstraint<FunctionAssertions<T>, T> NotThrow([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} not to throw{reason}, but found <null>.");

        T result = default;

        if (assertionChain.Succeeded)
        {
            try
            {
                result = Subject!();
            }
            catch (Exception exception)
            {
                assertionChain
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Did not expect any exception{reason}, but found {0}.", exception);

                result = default;
            }
        }

        return new AndWhichConstraint<FunctionAssertions<T>, T>(this, result, assertionChain, ".Result");
    }

    /// <summary>
    /// Asserts that the current <see cref="Func{T}"/> stops throwing any exception
    /// after a specified amount of time.
    /// </summary>
    /// <remarks>
    /// The <see cref="Func{T}"/> is invoked. If it raises an exception,
    /// the invocation is repeated until it either stops raising any exceptions
    /// or the specified wait time is exceeded.
    /// </remarks>
    /// <param name="waitTime">
    /// The time after which the <see cref="Func{T}"/> should have stopped throwing any exception.
    /// </param>
    /// <param name="pollInterval">
    /// The time between subsequent invocations of the <see cref="Func{T}"/>.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="waitTime"/> or <paramref name="pollInterval"/> are negative.</exception>
    [return: NotNull]
    public AndWhichConstraint<FunctionAssertions<T>, T> NotThrowAfter(TimeSpan waitTime, TimeSpan pollInterval,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} not to throw any exceptions after {0}{reason}, but found <null>.", waitTime);

        T result = default;

        if (assertionChain.Succeeded)
        {
            result = NotThrowAfter(Subject, Clock, waitTime, pollInterval, because, becauseArgs);
        }

        return new AndWhichConstraint<FunctionAssertions<T>, T>(this, result, assertionChain, ".Result");
    }

    private TResult NotThrowAfter<TResult>(Func<TResult> subject, IClock clock, TimeSpan waitTime, TimeSpan pollInterval,
        string because, object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNegative(waitTime);
        Guard.ThrowIfArgumentIsNegative(pollInterval);

        TimeSpan? invocationEndTime = null;
        Exception exception = null;
        ITimer timer = clock.StartTimer();

        while (invocationEndTime is null || invocationEndTime < waitTime)
        {
            try
            {
                return subject();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            clock.Delay(pollInterval);
            invocationEndTime = timer.Elapsed;
        }

        assertionChain
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect any exceptions after {0}{reason}, but found {1}.", waitTime, exception);

        return default;
    }
}
