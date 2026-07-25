using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace AwesomeAssertions.Specialized;

/// <summary>
/// Contains a number of methods to assert that a method yields the expected result.
/// </summary>
[DebuggerNonUserCode]
public abstract class DelegateAssertionsBase<TDelegate, TAssertions>
    : ReferenceTypeAssertions<TDelegate, DelegateAssertionsBase<TDelegate, TAssertions>>
    where TDelegate : Delegate
    where TAssertions : DelegateAssertionsBase<TDelegate, TAssertions>
{
    private readonly AssertionChain assertionChain;

    private protected IExtractExceptions Extractor { get; }

    private protected DelegateAssertionsBase(TDelegate @delegate, IExtractExceptions extractor, AssertionChain assertionChain,
        IClock clock)
        : base(@delegate, assertionChain)
    {
        this.assertionChain = assertionChain;
        Extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    private protected IClock Clock { get; }

    /// <summary>
    /// Asserts that the provided <paramref name="exception"/> contains an exception of type <typeparamref name="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception that is expected.</typeparam>
    /// <param name="exception">The exception that was thrown by the subject, or <see langword="null"/> if none was thrown.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <returns>
    /// Returns an object that allows asserting additional members of the thrown exception.
    /// </returns>
    protected ExceptionAssertions<TException> ThrowInternal<TException>(
        Exception exception,
        [StringSyntax("CompositeFormat")] string because, object[] becauseArgs)
        where TException : Exception
    {
        TException[] expectedExceptions = Extractor.OfType<TException>(exception).ToArray();

        assertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected a <{0}> to be thrown{reason}, ", typeof(TException), chain => chain
                .ForCondition(exception is not null)
                .FailWith("but no exception was thrown.")
                .Then
                .ForCondition(expectedExceptions.Length > 0)
                .FailWith("but found <{0}>:" + Environment.NewLine + "{1}.",
                    exception?.GetType(),
                    exception));

        return new ExceptionAssertions<TException>(expectedExceptions, assertionChain);
    }

    /// <summary>
    /// Asserts that no exception was thrown by the subject.
    /// </summary>
    /// <param name="exception">The exception that was thrown by the subject, or <see langword="null"/> if none was thrown.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <returns>An <see cref="AndConstraint{T}"/> which can be used to chain assertions.</returns>
    [return: NotNull]
    protected AndConstraint<TAssertions> NotThrowInternal(Exception exception, [StringSyntax("CompositeFormat")] string because,
        object[] becauseArgs)
    {
        assertionChain
            .ForCondition(exception is null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect any exception{reason}, but found {0}.", exception);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the provided <paramref name="exception"/> does not contain an exception of type <typeparamref name="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception that is not expected.</typeparam>
    /// <param name="exception">The exception that was thrown by the subject, or <see langword="null"/> if none was thrown.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <returns>An <see cref="AndConstraint{T}"/> which can be used to chain assertions.</returns>
    [return: NotNull]
    protected AndConstraint<TAssertions> NotThrowInternal<TException>(Exception exception,
        [StringSyntax("CompositeFormat")] string because, object[] becauseArgs)
        where TException : Exception
    {
        IEnumerable<TException> exceptions = Extractor.OfType<TException>(exception);

        assertionChain
            .ForCondition(!exceptions.Any())
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {0}{reason}, but found {1}.", typeof(TException), exception);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }
}
