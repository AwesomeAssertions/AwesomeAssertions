using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="DateTimeOffset"/> is in the expected state.
/// </summary>
/// <remarks>
/// You can use the <see cref="AwesomeAssertions.Extensions.FluentDateTimeExtensions"/>
/// for a more fluent way of specifying a <see cref="DateTime"/>.
/// </remarks>
[DebuggerNonUserCode]
public class NullableDateTimeOffsetAssertions : NullableDateTimeOffsetAssertions<NullableDateTimeOffsetAssertions>
{
    public NullableDateTimeOffsetAssertions(DateTimeOffset? expected, AssertionChain assertionChain)
        : base(expected, assertionChain)
    {
    }
}

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="DateTimeOffset"/> is in the expected state.
/// </summary>
/// <remarks>
/// You can use the <see cref="AwesomeAssertions.Extensions.FluentDateTimeExtensions"/>
/// for a more fluent way of specifying a <see cref="DateTime"/>.
/// </remarks>
[DebuggerNonUserCode]
public class NullableDateTimeOffsetAssertions<TAssertions> : DateTimeOffsetAssertions<TAssertions>
    where TAssertions : NullableDateTimeOffsetAssertions<TAssertions>
{
    private readonly AssertionChain assertionChain;

    public NullableDateTimeOffsetAssertions(DateTimeOffset? expected, AssertionChain assertionChain)
        : base(expected, assertionChain)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Asserts that a nullable <see cref="DateTimeOffset"/> value is not <see langword="null"/>.
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
            .FailWith("Expected {context:variable} to have a value{reason}, but found {0}", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a nullable <see cref="DateTimeOffset"/> value is not <see langword="null"/>.
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
    /// Asserts that a nullable <see cref="DateTimeOffset"/> value is <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveValue([StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(!Subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {context:variable} to have a value{reason}, but found {0}", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a nullable <see cref="DateTimeOffset"/> value is <see langword="null"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeNull([StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        return NotHaveValue(because, becauseArgs);
    }
}
