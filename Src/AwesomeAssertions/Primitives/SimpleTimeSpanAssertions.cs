using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="TimeSpan"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class SimpleTimeSpanAssertions : SimpleTimeSpanAssertions<SimpleTimeSpanAssertions>
{
    public SimpleTimeSpanAssertions(TimeSpan? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }
}

#pragma warning disable CS0659, S1206 // Ignore not overriding Object.GetHashCode()
#pragma warning disable CA1065 // Ignore throwing NotSupportedException from Equals
/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="TimeSpan"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class SimpleTimeSpanAssertions<TAssertions>
    where TAssertions : SimpleTimeSpanAssertions<TAssertions>
{
    public SimpleTimeSpanAssertions(TimeSpan? value, AssertionChain assertionChain)
    {
        CurrentAssertionChain = assertionChain;
        Subject = value;
    }

    /// <summary>
    /// Gets the object whose value is being asserted.
    /// </summary>
    public TimeSpan? Subject { get; }

    /// <summary>
    /// Provides access to the <see cref="AssertionChain"/> that this assertion class was initialized with.
    /// </summary>
    public AssertionChain CurrentAssertionChain { get; }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is greater than zero.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BePositive([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject > TimeSpan.Zero)
            .FailWith("Expected {context:time} to be positive{reason}, but found {0}.", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is less than zero.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeNegative([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject < TimeSpan.Zero)
            .FailWith("Expected {context:time} to be negative{reason}, but found {0}.", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is equal to the
    /// specified <paramref name="expected"/> time.
    /// </summary>
    /// <param name="expected">The expected time difference</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> Be(TimeSpan expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(expected == Subject)
            .FailWith("Expected {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is not equal to the
    /// specified <paramref name="unexpected"/> time.
    /// </summary>
    /// <param name="unexpected">The unexpected time difference</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBe(TimeSpan unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(unexpected != Subject)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {0}{reason}.", unexpected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is less than the
    /// specified <paramref name="expected"/> time.
    /// </summary>
    /// <param name="expected">The time difference to which the current value will be compared</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeLessThan(TimeSpan expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject < expected)
            .FailWith("Expected {context:time} to be less than {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is less than or equal to the
    /// specified <paramref name="expected"/> time.
    /// </summary>
    /// <param name="expected">The time difference to which the current value will be compared</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeLessThanOrEqualTo(TimeSpan expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject <= expected)
            .FailWith("Expected {context:time} to be less than or equal to {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is greater than the
    /// specified <paramref name="expected"/> time.
    /// </summary>
    /// <param name="expected">The time difference to which the current value will be compared</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeGreaterThan(TimeSpan expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject > expected)
            .FailWith("Expected {context:time} to be greater than {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the time difference of the current <see cref="TimeSpan"/> is greater than or equal to the
    /// specified <paramref name="expected"/> time.
    /// </summary>
    /// <param name="expected">The time difference to which the current value will be compared</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeGreaterThanOrEqualTo(TimeSpan expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject >= expected)
            .FailWith("Expected {context:time} to be greater than or equal to {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="TimeSpan"/> is within the specified time
    /// from the specified <paramref name="nearbyTime"/> value.
    /// </summary>
    /// <remarks>
    /// Use this assertion when, for example the database truncates datetimes to nearest 20ms. If you want to assert to the exact datetime,
    /// use <see cref="Be"/>.
    /// </remarks>
    /// <param name="nearbyTime">
    /// The expected time to compare the actual value with.
    /// </param>
    /// <param name="precision">
    /// The maximum amount of time which the two values may differ.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    public AndConstraint<TAssertions> BeCloseTo(TimeSpan nearbyTime, TimeSpan precision,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNegative(precision);

        TimeSpan minimumValue = nearbyTime - precision;
        TimeSpan maximumValue = nearbyTime + precision;

        CurrentAssertionChain
            .ForCondition(Subject >= minimumValue && Subject.Value <= maximumValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:time} to be within {0} from {1}{reason}, but found {2}.",
                precision,
                nearbyTime, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="TimeSpan"/> is not within the specified time
    /// from the specified <paramref name="distantTime"/> value.
    /// </summary>
    /// <remarks>
    /// Use this assertion when, for example the database truncates datetimes to nearest 20ms. If you want to assert to the exact datetime,
    /// use <see cref="NotBe"/>.
    /// </remarks>
    /// <param name="distantTime">
    /// The time to compare the actual value with.
    /// </param>
    /// <param name="precision">
    /// The maximum amount of time which the two values may differ.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    public AndConstraint<TAssertions> NotBeCloseTo(TimeSpan distantTime, TimeSpan precision,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNegative(precision);

        TimeSpan minimumValue = distantTime - precision;
        TimeSpan maximumValue = distantTime + precision;

        CurrentAssertionChain
            .ForCondition(Subject < minimumValue || Subject > maximumValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:time} to not be within {0} from {1}{reason}, but found {2}.",
                precision,
                distantTime, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        throw new NotSupportedException("Equals is not part of Awesome Assertions. Did you mean Be() instead?");
}
