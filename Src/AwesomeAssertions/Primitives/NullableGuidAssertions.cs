using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="Guid"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class NullableGuidAssertions : NullableGuidAssertions<NullableGuidAssertions>
{
    public NullableGuidAssertions(Guid? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }
}

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="Guid"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class NullableGuidAssertions<TAssertions> : GuidAssertions<TAssertions>
    where TAssertions : NullableGuidAssertions<TAssertions>
{
    private readonly AssertionChain assertionChain;

    public NullableGuidAssertions(Guid? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Asserts that a nullable <see cref="Guid"/> value is not <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveValue([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected a value{reason}.");

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a nullable <see cref="Guid"/> value is not <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeNull([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return HaveValue(because, becauseArgs);
    }

    /// <summary>
    /// Asserts that a nullable <see cref="Guid"/> value is <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveValue([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(!Subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect a value{reason}, but found {0}.", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a nullable <see cref="Guid"/> value is <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeNull([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return NotHaveValue(because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the value is equal to the specified <paramref name="expected"/> value.
    /// </summary>
    /// <param name="expected">The expected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> Be(Guid? expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject == expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:Guid} to be {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }
}
