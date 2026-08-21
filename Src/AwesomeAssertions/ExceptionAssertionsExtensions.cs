using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Specialized;

namespace AwesomeAssertions;

/// <summary>
/// Provides extension methods for asserting on exceptions, including the results of asynchronous exception assertions.
/// </summary>
/// <remarks>
/// Assertions that apply to every thrown exception are members of <see cref="ExceptionAssertions{TException}"/> and
/// <see cref="ExceptionAssertionsTask{TException}"/> themselves. This class hosts those that apply to a subset of the
/// exception types only, such as the ones requiring an <see cref="ArgumentException"/>, because a member cannot
/// narrow the type parameter of the class that declares it.
/// </remarks>
public static class ExceptionAssertionsExtensions
{
    /// <summary>
    /// Continues asserting on the exception provided by <paramref name="task"/>.
    /// </summary>
    /// <typeparam name="TException">The type of the thrown exception.</typeparam>
    /// <param name="task">The task providing the <see cref="ExceptionAssertions{TException}"/> to continue on.</param>
    /// <remarks>
    /// Needed to continue on a <see cref="Task{TResult}"/> that one of the <c>ThrowAsync</c> assertions did not
    /// produce, such as the outcome of your own helper method. Unlike the constructor of
    /// <see cref="ExceptionAssertionsTask{TException}"/>, this infers <typeparamref name="TException"/>, so it does
    /// not have to be spelled out.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    public static ExceptionAssertionsTask<TException> AsExceptionAssertionsTask<TException>(
        this Task<ExceptionAssertions<TException>> task)
        where TException : Exception
    {
        return new ExceptionAssertionsTask<TException>(task);
    }

    /// <summary>
    /// Asserts that the thrown exception has a parameter which name matches <paramref name="paramName" /> case insensitive.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="parent">The <see cref="ExceptionAssertions{TException}"/> containing the thrown exception.</param>
    /// <param name="paramName">The expected name of the parameter</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="paramName"/> is <langword>null</langword> or empty.</exception>
    public static ExceptionAssertions<TException> WithParameterName<TException>(
        this ExceptionAssertions<TException> parent,
        string paramName,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
        where TException : ArgumentException
    {
        Guard.ThrowIfArgumentIsNullOrEmpty(paramName);

        AssertionChain
            .GetOrCreate()
            .BecauseOf(because, becauseArgs)
            .ForCondition(paramName.Equals(parent.Which.ParamName, StringComparison.OrdinalIgnoreCase))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected exception with parameter name {0}{reason}, but found {1}.", paramName, parent.Which.ParamName);

        return parent;
    }

    /// <summary>
    /// Asserts that the thrown exception has a parameter which name matches <paramref name="paramName" />.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="assertions">The <see cref="ExceptionAssertionsTask{TException}"/> containing the thrown exception.</param>
    /// <param name="paramName">The expected name of the parameter</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> is <see langword="null"/>.</exception>
    public static ExceptionAssertionsTask<TException> WithParameterName<TException>(
        this ExceptionAssertionsTask<TException> assertions,
        string paramName,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
        where TException : ArgumentException
    {
        Guard.ThrowIfArgumentIsNull(assertions);

        return new ExceptionAssertionsTask<TException>(
            WithParameterNameAsync(assertions.AsTask(), paramName, because, becauseArgs));
    }

    private static async Task<ExceptionAssertions<TException>> WithParameterNameAsync<TException>(
        Task<ExceptionAssertions<TException>> task,
        string paramName,
        string because,
        object[] becauseArgs)
        where TException : ArgumentException
    {
        return (await task).WithParameterName(paramName, because, becauseArgs);
    }
}
