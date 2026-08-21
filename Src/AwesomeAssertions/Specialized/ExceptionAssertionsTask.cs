using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Specialized;

/// <summary>
/// Represents the awaitable outcome of an asynchronous exception assertion, such as
/// <see cref="AsyncFunctionAssertions{TTask,TAssertions}.ThrowAsync{TException}"/>.
/// </summary>
/// <typeparam name="TException">The type of the thrown exception.</typeparam>
/// <remarks>
/// Awaiting an instance of this type yields the <see cref="ExceptionAssertions{TException}"/> of the thrown exception,
/// so it can be used wherever a <see cref="Task{TResult}"/> was expected before.
/// <para>
/// Since <typeparamref name="TException"/> is part of this type instead of being inferred from an argument,
/// assertions about the inner exception take a single type parameter, just like their synchronous counterparts.
/// </para>
/// </remarks>
[DebuggerNonUserCode]
public sealed class ExceptionAssertionsTask<TException>
    where TException : Exception
{
    private readonly Task<ExceptionAssertions<TException>> task;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionAssertionsTask{TException}"/> class.
    /// </summary>
    /// <param name="task">The task providing the <see cref="ExceptionAssertions{TException}"/> to continue on.</param>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    public ExceptionAssertionsTask(Task<ExceptionAssertions<TException>> task)
    {
        Guard.ThrowIfArgumentIsNull(task);

        this.task = task;
    }

    /// <summary>
    /// Converts the assertion into the <see cref="Task{TResult}"/> it wraps.
    /// </summary>
    /// <remarks>
    /// This conversion is not merely convenient, it carries source compatibility: it keeps code compiling that uses the
    /// outcome where a <see cref="Task"/> is expected, such as assigning the assertion chain to a <c>Func&lt;Task&gt;</c>
    /// or passing it to <see cref="Task.WhenAll(Task[])"/>. Removing it is a breaking change.
    /// </remarks>
    public static implicit operator Task<ExceptionAssertions<TException>>(ExceptionAssertionsTask<TException> assertions)
    {
        Guard.ThrowIfArgumentIsNull(assertions);

        return assertions.task;
    }

    /// <summary>
    /// Returns the <see cref="Task{TResult}"/> this assertion wraps, as the named alternative to the implicit conversion.
    /// </summary>
    /// <remarks>
    /// Needed to hand the outcome to code that requires an actual <see cref="Task{TResult}"/>. In particular, own
    /// extension methods on <c>Task&lt;ExceptionAssertions&lt;TException&gt;&gt;</c> are not found on this type, because
    /// extension method lookup never applies the implicit conversion declared above. Inserting <c>AsTask()</c> makes
    /// them available again.
    /// </remarks>
    public Task<ExceptionAssertions<TException>> AsTask() => task;

    /// <summary>
    /// Gets an awaiter used to await the <see cref="ExceptionAssertions{TException}"/> of the thrown exception.
    /// </summary>
    /// <remarks>
    /// This is what makes the assertion awaitable, as in <c>await action.Should().ThrowAsync&lt;TException&gt;()</c>.
    /// It satisfies the awaitable pattern the compiler looks for by convention; no interface is involved.
    /// </remarks>
    public TaskAwaiter<ExceptionAssertions<TException>> GetAwaiter() => task.GetAwaiter();

    /// <summary>
    /// Configures how the awaited continuation is scheduled.
    /// </summary>
    /// <param name="continueOnCapturedContext">
    /// <see langword="true"/> to attempt to marshal the continuation back to the captured original context;
    /// otherwise <see langword="false"/>.
    /// </param>
    public ConfiguredTaskAwaitable<ExceptionAssertions<TException>> ConfigureAwait(bool continueOnCapturedContext) =>
        task.ConfigureAwait(continueOnCapturedContext);

    /// <summary>
    /// Asserts that the thrown exception has a message that matches <paramref name="expectedWildcardPattern" />.
    /// </summary>
    /// <param name="expectedWildcardPattern">
    /// The wildcard pattern with which the exception message is matched, where * and ? have special meanings.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public ExceptionAssertionsTask<TException> WithMessage(
        string expectedWildcardPattern,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        return ContinueWith(assertions => assertions.WithMessage(expectedWildcardPattern, because, becauseArgs));
    }

    /// <summary>
    /// Asserts that the exception matches a particular condition.
    /// </summary>
    /// <param name="exceptionExpression">
    /// The condition that the exception must match.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public ExceptionAssertionsTask<TException> Where(
        Expression<Func<TException, bool>> exceptionExpression,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        return ContinueWith(assertions => assertions.Where(exceptionExpression, because, becauseArgs));
    }

    /// <summary>
    /// Asserts that the thrown exception contains an inner exception of type <typeparamref name="TInnerException" />.
    /// </summary>
    /// <typeparam name="TInnerException">The expected type of the inner exception.</typeparam>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public ExceptionAssertionsTask<TInnerException> WithInnerException<TInnerException>(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
        where TInnerException : Exception
    {
        return ContinueWith(assertions => assertions.WithInnerException<TInnerException>(because, becauseArgs));
    }

    /// <summary>
    /// Asserts that the thrown exception contains an inner exception of type <paramref name="innerException" />.
    /// </summary>
    /// <param name="innerException">The expected type of the inner exception.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public ExceptionAssertionsTask<Exception> WithInnerException(
        Type innerException,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        return ContinueWith(assertions => assertions.WithInnerException(innerException, because, becauseArgs));
    }

    /// <summary>
    /// Asserts that the thrown exception contains an inner exception of the exact type
    /// <typeparamref name="TInnerException" /> (and not a derived exception type).
    /// </summary>
    /// <typeparam name="TInnerException">The expected type of the inner exception.</typeparam>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public ExceptionAssertionsTask<TInnerException> WithInnerExceptionExactly<TInnerException>(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
        where TInnerException : Exception
    {
        return ContinueWith(assertions => assertions.WithInnerExceptionExactly<TInnerException>(because, becauseArgs));
    }

    /// <summary>
    /// Asserts that the thrown exception contains an inner exception of the exact type
    /// <paramref name="innerException" /> (and not a derived exception type).
    /// </summary>
    /// <param name="innerException">The expected type of the inner exception.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public ExceptionAssertionsTask<Exception> WithInnerExceptionExactly(
        Type innerException,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        return ContinueWith(assertions => assertions.WithInnerExceptionExactly(innerException, because, becauseArgs));
    }

    private ExceptionAssertionsTask<TResult> ContinueWith<TResult>(
        Func<ExceptionAssertions<TException>, ExceptionAssertions<TResult>> assertion)
        where TResult : Exception
    {
        return new ExceptionAssertionsTask<TResult>(AssertAsync(task, assertion));
    }

    private static async Task<ExceptionAssertions<TResult>> AssertAsync<TResult>(
        Task<ExceptionAssertions<TException>> task,
        Func<ExceptionAssertions<TException>, ExceptionAssertions<TResult>> assertion)
        where TResult : Exception
    {
        return assertion(await task);
    }
}
