using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="IComparable{T}"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class NullableNumericAssertions<T> : NullableNumericAssertions<T, NumericAssertions<T>, NullableNumericAssertions<T>>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullableNumericAssertions{T}"/> class.
    /// </summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    public NullableNumericAssertions(T? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    /// <inheritdoc/>
    protected override NumericAssertions<T> AsTAssertions() => new(Subject.GetValueOrDefault(), CurrentAssertionChain);
}

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="IComparable{T}"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public abstract class NullableNumericAssertions<T, TAssertions, TNullableAssertions> : NumericAssertionsBase<T, T?, TNullableAssertions>
    where T : struct, IComparable<T>
    where TAssertions : NumericAssertions<T, TAssertions>
    where TNullableAssertions : NullableNumericAssertions<T, TAssertions, TNullableAssertions>
{
    private readonly AssertionChain assertionChain;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableNumericAssertions{T, TAssertions, TNullableAssertions}"/> class.
    /// </summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    protected NullableNumericAssertions(T? value, AssertionChain assertionChain)
        : base(assertionChain)
    {
        Subject = value;
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Gets the object whose value is being asserted.
    /// </summary>
    public override T? Subject { get; }

    /// <summary>
    /// Asserts that a nullable numeric value is not <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndConstraint<TAssertions> HaveValue([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected a value{reason}.");

        return new AndConstraint<TAssertions>(AsTAssertions());
    }

    /// <summary>
    /// Returns the current instance as <typeparamref name="TAssertions"/>.
    /// </summary>
    /// <returns></returns>
    protected abstract TAssertions AsTAssertions();

    /// <summary>
    /// Asserts that a nullable numeric value is not <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndConstraint<TAssertions> NotBeNull([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return HaveValue(because, becauseArgs);
    }

    /// <summary>
    /// Asserts that a nullable numeric value is <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndConstraint<TNullableAssertions> NotHaveValue([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(!Subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect a value{reason}, but found {0}.", Subject);

        return new AndConstraint<TNullableAssertions>((TNullableAssertions)this);
    }

    /// <summary>
    /// Asserts that a nullable numeric value is <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndConstraint<TNullableAssertions> BeNull([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return NotHaveValue(because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the <paramref name="predicate" /> is satisfied.
    /// </summary>
    /// <param name="predicate">
    /// The predicate which must be satisfied
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    [return: NotNull]
    public AndConstraint<TNullableAssertions> Match(Expression<Func<T?, bool>> predicate,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNull(predicate);

        assertionChain
            .ForCondition(predicate.Compile()(Subject))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected value to match {0}{reason}, but found {1}.", predicate, Subject);

        return new AndConstraint<TNullableAssertions>((TNullableAssertions)this);
    }
}
