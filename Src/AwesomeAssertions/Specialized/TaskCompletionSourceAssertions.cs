using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Specialized;

#pragma warning disable CS0659, S1206 // Ignore not overriding Object.GetHashCode()
#pragma warning disable CA1065 // Ignore throwing NotSupportedException from Equals

#if NET6_0_OR_GREATER
public class TaskCompletionSourceAssertions : TaskCompletionSourceAssertionsBase
{
    private readonly TaskCompletionSource subject;

    public TaskCompletionSourceAssertions(TaskCompletionSource tcs, AssertionChain assertionChain)
        : this(tcs, assertionChain, new Clock())
    {
        CurrentAssertionChain = assertionChain;
    }

    public TaskCompletionSourceAssertions(TaskCompletionSource tcs, AssertionChain assertionChain, IClock clock)
        : base(clock)
    {
        subject = tcs;
        CurrentAssertionChain = assertionChain;
    }

    /// <summary>
    /// Provides access to the <see cref="AssertionChain"/> that this assertion class was initialized with.
    /// </summary>
    public AssertionChain CurrentAssertionChain { get; }

    /// <summary>
    /// Asserts that the <see cref="Task"/> of the current <see cref="TaskCompletionSource"/> will complete within the specified time.
    /// </summary>
    /// <param name="timeSpan">The allowed time span for the operation.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public async Task<AndConstraint<TaskCompletionSourceAssertions>> CompleteWithinAsync(
        TimeSpan timeSpan, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} to complete within {0}{reason}, but found <null>.", timeSpan);

        if (CurrentAssertionChain.Succeeded)
        {
            bool completesWithinTimeout = await CompletesWithinTimeoutAsync(subject!.Task, timeSpan);
            CurrentAssertionChain
                .ForCondition(completesWithinTimeout)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:task} to complete within {0}{reason}.", timeSpan);
        }

        return new AndConstraint<TaskCompletionSourceAssertions>(this);
    }

    /// <summary>
    /// Asserts that the <see cref="Task"/> of the current <see cref="TaskCompletionSource"/> will not complete within the specified time.
    /// </summary>
    /// <param name="timeSpan">The time span to wait for the operation.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public async Task<AndConstraint<TaskCompletionSourceAssertions>> NotCompleteWithinAsync(
        TimeSpan timeSpan, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} to not complete within {0}{reason}, but found <null>.", timeSpan);

        if (CurrentAssertionChain.Succeeded)
        {
            bool completesWithinTimeout = await CompletesWithinTimeoutAsync(subject!.Task, timeSpan);
            CurrentAssertionChain
                .ForCondition(!completesWithinTimeout)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:task} to not complete within {0}{reason}.", timeSpan);
        }

        return new AndConstraint<TaskCompletionSourceAssertions>(this);
    }
}
#endif

public class TaskCompletionSourceAssertions<T> : TaskCompletionSourceAssertionsBase
{
    private readonly TaskCompletionSource<T> subject;

    public TaskCompletionSourceAssertions(TaskCompletionSource<T> tcs, AssertionChain assertionChain)
        : this(tcs, assertionChain, new Clock())
    {
        CurrentAssertionChain = assertionChain;
    }

    public TaskCompletionSourceAssertions(TaskCompletionSource<T> tcs, AssertionChain assertionChain, IClock clock)
        : base(clock)
    {
        subject = tcs;
        CurrentAssertionChain = assertionChain;
    }

    /// <summary>
    /// Provides access to the <see cref="AssertionChain"/> that this assertion class was initialized with.
    /// </summary>
    public AssertionChain CurrentAssertionChain { get; }

    /// <summary>
    /// Asserts that the <see cref="Task"/> of the current <see cref="TaskCompletionSource{T}"/> will complete within the specified time.
    /// </summary>
    /// <param name="timeSpan">The allowed time span for the operation.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public async Task<AndWhichConstraint<TaskCompletionSourceAssertions<T>, T>> CompleteWithinAsync(
        TimeSpan timeSpan, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} to complete within {0}{reason}, but found <null>.", timeSpan);

        if (CurrentAssertionChain.Succeeded)
        {
            bool completesWithinTimeout = await CompletesWithinTimeoutAsync(subject!.Task, timeSpan);

            CurrentAssertionChain
                .ForCondition(completesWithinTimeout)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:task} to complete within {0}{reason}.", timeSpan);

#pragma warning disable CA1849 // Call async methods when in an async method
            T result = subject.Task.IsCompleted ? subject.Task.Result : default;
#pragma warning restore CA1849 // Call async methods when in an async method
            return new AndWhichConstraint<TaskCompletionSourceAssertions<T>, T>(this, result, CurrentAssertionChain, ".Result");
        }

        return new AndWhichConstraint<TaskCompletionSourceAssertions<T>, T>(this, default(T));
    }

    /// <summary>
    /// Asserts that the <see cref="Task"/> of the current <see cref="TaskCompletionSource{T}"/> will not complete within the specified time.
    /// </summary>
    /// <param name="timeSpan">The time span to wait for the operation.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public async Task<AndConstraint<TaskCompletionSourceAssertions<T>>> NotCompleteWithinAsync(
        TimeSpan timeSpan, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {context} to complete within {0}{reason}, but found <null>.", timeSpan);

        if (CurrentAssertionChain.Succeeded)
        {
            bool completesWithinTimeout = await CompletesWithinTimeoutAsync(subject!.Task, timeSpan);

            CurrentAssertionChain
                .ForCondition(!completesWithinTimeout)
                .BecauseOf(because, becauseArgs)
                .FailWith("Did not expect {context:task} to complete within {0}{reason}.", timeSpan);
        }

        return new AndConstraint<TaskCompletionSourceAssertions<T>>(this);
    }
}
