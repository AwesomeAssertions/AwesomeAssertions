using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

/// <summary>
/// Contains a number of methods to assert that a <see cref="DateTime"/> is in the expected state.
/// </summary>
/// <remarks>
/// You can use the <see cref="AwesomeAssertions.Extensions.FluentDateTimeExtensions"/>
/// for a more fluent way of specifying a <see cref="DateTime"/>.
/// </remarks>
[DebuggerNonUserCode]
public class DateTimeAssertions : DateTimeAssertions<DateTimeAssertions>
{
    public DateTimeAssertions(DateTime? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }
}

#pragma warning disable CS0659, S1206 // Ignore not overriding Object.GetHashCode()
#pragma warning disable CA1065 // Ignore throwing NotSupportedException from Equals
/// <summary>
/// Contains a number of methods to assert that a <see cref="DateTime"/> is in the expected state.
/// </summary>
/// <remarks>
/// You can use the <see cref="AwesomeAssertions.Extensions.FluentDateTimeExtensions"/>
/// for a more fluent way of specifying a <see cref="DateTime"/>.
/// </remarks>
[DebuggerNonUserCode]
public class DateTimeAssertions<TAssertions>
    where TAssertions : DateTimeAssertions<TAssertions>
{
    public DateTimeAssertions(DateTime? value, AssertionChain assertionChain)
    {
        CurrentAssertionChain = assertionChain;
        Subject = value;
    }

    /// <summary>
    /// Gets the object whose value is being asserted.
    /// </summary>
    public DateTime? Subject { get; }

    /// <summary>
    /// Provides access to the <see cref="AssertionChain"/> that this assertion class was initialized with.
    /// </summary>
    public AssertionChain CurrentAssertionChain { get; }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> is exactly equal to the <paramref name="expected"/> value.
    /// </summary>
    /// <param name="expected">The expected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> Be(DateTime expected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject == expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:date and time} to be {0}{reason}, but found {1}.",
                expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> is exactly equal to the <paramref name="expected"/> value.
    /// </summary>
    /// <param name="expected">The expected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> Be(DateTime? expected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject == expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:date and time} to be {0}{reason}, but found {1}.",
                expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> or <see cref="DateTime"/> is not equal to the <paramref name="unexpected"/> value.
    /// </summary>
    /// <param name="unexpected">The unexpected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBe(DateTime unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject != unexpected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:date and time} not to be {0}{reason}, but it is.", unexpected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> or <see cref="DateTime"/> is not equal to the <paramref name="unexpected"/> value.
    /// </summary>
    /// <param name="unexpected">The unexpected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBe(DateTime? unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject != unexpected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:date and time} not to be {0}{reason}, but it is.", unexpected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is within the specified time
    /// from the specified <paramref name="nearbyTime"/> value.
    /// </summary>
    /// <remarks>
    /// Use this assertion when, for example the database truncates datetimes to nearest 20ms. If you want to assert to the exact datetime,
    /// use <see cref="Be(DateTime, string, object[])"/>.
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
    public AndConstraint<TAssertions> BeCloseTo(DateTime nearbyTime, TimeSpan precision,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNegative(precision);

        long distanceToMinInTicks = (nearbyTime - DateTime.MinValue).Ticks;
        DateTime minimumValue = nearbyTime.AddTicks(-Math.Min(precision.Ticks, distanceToMinInTicks));

        long distanceToMaxInTicks = (DateTime.MaxValue - nearbyTime).Ticks;
        DateTime maximumValue = nearbyTime.AddTicks(Math.Min(precision.Ticks, distanceToMaxInTicks));

        TimeSpan? difference = (Subject - nearbyTime)?.Duration();

        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:the date and time} to be within {0} from {1}{reason}", precision, nearbyTime,
                chain => chain
                    .ForCondition(Subject is not null)
                    .FailWith(", but found <null>.")
                    .Then
                    .ForCondition(Subject >= minimumValue && Subject <= maximumValue)
                    .FailWith(", but {0} was off by {1}.", Subject, difference));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is not within the specified time
    /// from the specified <paramref name="distantTime"/> value.
    /// </summary>
    /// <remarks>
    /// Use this assertion when, for example the database truncates datetimes to nearest 20ms. If you want to assert to the exact datetime,
    /// use <see cref="NotBe(DateTime, string, object[])"/>.
    /// </remarks>
    /// <param name="distantTime">
    /// The time to compare the actual value with.
    /// </param>
    /// <param name="precision">
    /// The maximum amount of time which the two values must differ.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    public AndConstraint<TAssertions> NotBeCloseTo(DateTime distantTime, TimeSpan precision,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNegative(precision);

        long distanceToMinInTicks = (distantTime - DateTime.MinValue).Ticks;
        DateTime minimumValue = distantTime.AddTicks(-Math.Min(precision.Ticks, distanceToMinInTicks));

        long distanceToMaxInTicks = (DateTime.MaxValue - distantTime).Ticks;
        DateTime maximumValue = distantTime.AddTicks(Math.Min(precision.Ticks, distanceToMaxInTicks));

        CurrentAssertionChain
            .ForCondition(Subject < minimumValue || Subject > maximumValue)
            .BecauseOf(because, becauseArgs)
            .FailWith(
                "Did not expect {context:the date and time} to be within {0} from {1}{reason}, but it was {2}.",
                precision,
                distantTime, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is before the specified value.
    /// </summary>
    /// <param name="expected">The <see cref="DateTime"/>  that the current value is expected to be before.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeBefore(DateTime expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject < expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:the date and time} to be before {0}{reason}, but found {1}.", expected,
                Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is not before the specified value.
    /// </summary>
    /// <param name="unexpected">The <see cref="DateTime"/>  that the current value is not expected to be before.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeBefore(DateTime unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return BeOnOrAfter(unexpected, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is either on, or before the specified value.
    /// </summary>
    /// <param name="expected">The <see cref="DateTime"/>  that the current value is expected to be on or before.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeOnOrBefore(DateTime expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject <= expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:the date and time} to be on or before {0}{reason}, but found {1}.", expected,
                Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is neither on, nor before the specified value.
    /// </summary>
    /// <param name="unexpected">The <see cref="DateTime"/>  that the current value is not expected to be on nor before.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeOnOrBefore(DateTime unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return BeAfter(unexpected, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is after the specified value.
    /// </summary>
    /// <param name="expected">The <see cref="DateTime"/>  that the current value is expected to be after.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeAfter(DateTime expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject > expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:the date and time} to be after {0}{reason}, but found {1}.", expected,
                Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is not after the specified value.
    /// </summary>
    /// <param name="unexpected">The <see cref="DateTime"/>  that the current value is not expected to be after.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeAfter(DateTime unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return BeOnOrBefore(unexpected, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is either on, or after the specified value.
    /// </summary>
    /// <param name="expected">The <see cref="DateTime"/>  that the current value is expected to be on or after.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeOnOrAfter(DateTime expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject >= expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:the date and time} to be on or after {0}{reason}, but found {1}.", expected,
                Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  is neither on, nor after the specified value.
    /// </summary>
    /// <param name="unexpected">The <see cref="DateTime"/>  that the current value is expected not to be on nor after.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeOnOrAfter(DateTime unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return BeBefore(unexpected, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> has the <paramref name="expected"/> year.
    /// </summary>
    /// <param name="expected">The expected year of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveYear(int expected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the year part of {context:the date} to be {0}{reason}", expected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found <null>.")
                .Then
                .ForCondition(Subject.Value.Year == expected)
                .FailWith(", but found {0}.", Subject.Value.Year));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> does not have the <paramref name="unexpected"/> year.
    /// </summary>
    /// <param name="unexpected">The year that should not be in the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveYear(int unexpected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.HasValue)
            .FailWith("Did not expect the year part of {context:the date} to be {0}{reason}, but found a <null> DateTime.",
                unexpected)
            .Then
            .ForCondition(Subject.Value.Year != unexpected)
            .FailWith("Did not expect the year part of {context:the date} to be {0}{reason}, but it was.", unexpected,
                Subject.Value.Year);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> has the <paramref name="expected"/> month.
    /// </summary>
    /// <param name="expected">The expected month of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveMonth(int expected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the month part of {context:the date} to be {0}{reason}", expected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Month == expected)
                .FailWith(", but found {0}.", Subject.Value.Month));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> does not have the <paramref name="unexpected"/> month.
    /// </summary>
    /// <param name="unexpected">The month that should not be in the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveMonth(int unexpected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect the month part of {context:the date} to be {0}{reason}", unexpected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Month != unexpected)
                .FailWith(", but it was."));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  has the <paramref name="expected"/> day.
    /// </summary>
    /// <param name="expected">The expected day of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveDay(int expected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the day part of {context:the date} to be {0}{reason}", expected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Day == expected)
                .FailWith(", but found {0}.", Subject.Value.Day));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> does not have the <paramref name="unexpected"/> day.
    /// </summary>
    /// <param name="unexpected">The day that should not be in the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveDay(int unexpected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect the day part of {context:the date} to be {0}{reason}", unexpected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Day != unexpected)
                .FailWith(", but it was."));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  has the <paramref name="expected"/> hour.
    /// </summary>
    /// <param name="expected">The expected hour of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveHour(int expected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the hour part of {context:the time} to be {0}{reason}", expected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Hour == expected)
                .FailWith(", but found {0}.", Subject.Value.Hour));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> does not have the <paramref name="unexpected"/> hour.
    /// </summary>
    /// <param name="unexpected">The hour that should not be in the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveHour(int unexpected, [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect the hour part of {context:the time} to be {0}{reason}", unexpected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.", unexpected)
                .Then
                .ForCondition(Subject.Value.Hour != unexpected)
                .FailWith(", but it was.", unexpected, Subject.Value.Hour));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  has the <paramref name="expected"/> minute.
    /// </summary>
    /// <param name="expected">The expected minutes of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveMinute(int expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the minute part of {context:the time} to be {0}{reason}", expected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Minute == expected)
                .FailWith(", but found {0}.", Subject.Value.Minute));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> does not have the <paramref name="unexpected"/> minute.
    /// </summary>
    /// <param name="unexpected">The minute that should not be in the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveMinute(int unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect the minute part of {context:the time} to be {0}{reason}", unexpected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.", unexpected)
                .Then
                .ForCondition(Subject.Value.Minute != unexpected)
                .FailWith(", but it was.", unexpected, Subject.Value.Minute));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/>  has the <paramref name="expected"/> second.
    /// </summary>
    /// <param name="expected">The expected seconds of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> HaveSecond(int expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the seconds part of {context:the time} to be {0}{reason}", expected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Second == expected)
                .FailWith(", but found {0}.", Subject.Value.Second));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> does not have the <paramref name="unexpected"/> second.
    /// </summary>
    /// <param name="unexpected">The second that should not be in the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotHaveSecond(int unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect the seconds part of {context:the time} to be {0}{reason}", unexpected, chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Second != unexpected)
                .FailWith(", but it was."));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Returns a <see cref="DateTimeRangeAssertions{TAssertions}"/> object that can be used to assert that the current <see cref="DateTime"/>
    /// exceeds the specified <paramref name="timeSpan"/> compared to another <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The amount of time that the current <see cref="DateTime"/>  should exceed compared to another <see cref="DateTime"/>.
    /// </param>
    public DateTimeRangeAssertions<TAssertions> BeMoreThan(TimeSpan timeSpan)
    {
        return new DateTimeRangeAssertions<TAssertions>((TAssertions)this, CurrentAssertionChain, Subject, TimeSpanCondition.MoreThan,
            timeSpan);
    }

    /// <summary>
    /// Returns a <see cref="DateTimeRangeAssertions{TAssertions}"/> object that can be used to assert that the current <see cref="DateTime"/>
    /// is equal to or exceeds the specified <paramref name="timeSpan"/> compared to another <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The amount of time that the current <see cref="DateTime"/>  should be equal or exceed compared to
    /// another <see cref="DateTime"/>.
    /// </param>
    public DateTimeRangeAssertions<TAssertions> BeAtLeast(TimeSpan timeSpan)
    {
        return new DateTimeRangeAssertions<TAssertions>((TAssertions)this, CurrentAssertionChain, Subject, TimeSpanCondition.AtLeast,
            timeSpan);
    }

    /// <summary>
    /// Returns a <see cref="DateTimeRangeAssertions{TAssertions}"/> object that can be used to assert that the current <see cref="DateTime"/>
    /// differs exactly the specified <paramref name="timeSpan"/> compared to another <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The amount of time that the current <see cref="DateTime"/>  should differ exactly compared to another <see cref="DateTime"/>.
    /// </param>
    public DateTimeRangeAssertions<TAssertions> BeExactly(TimeSpan timeSpan)
    {
        return new DateTimeRangeAssertions<TAssertions>((TAssertions)this, CurrentAssertionChain, Subject, TimeSpanCondition.Exactly,
            timeSpan);
    }

    /// <summary>
    /// Returns a <see cref="DateTimeRangeAssertions{TAssertions}"/> object that can be used to assert that the current <see cref="DateTime"/>
    /// is within the specified <paramref name="timeSpan"/> compared to another <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The amount of time that the current <see cref="DateTime"/>  should be within another <see cref="DateTime"/>.
    /// </param>
    public DateTimeRangeAssertions<TAssertions> BeWithin(TimeSpan timeSpan)
    {
        return new DateTimeRangeAssertions<TAssertions>((TAssertions)this, CurrentAssertionChain, Subject, TimeSpanCondition.Within,
            timeSpan);
    }

    /// <summary>
    /// Returns a <see cref="DateTimeRangeAssertions{TAssertions}"/> object that can be used to assert that the current <see cref="DateTime"/>
    /// differs at maximum the specified <paramref name="timeSpan"/> compared to another <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The maximum amount of time that the current <see cref="DateTime"/>  should differ compared to another <see cref="DateTime"/>.
    /// </param>
    public DateTimeRangeAssertions<TAssertions> BeLessThan(TimeSpan timeSpan)
    {
        return new DateTimeRangeAssertions<TAssertions>((TAssertions)this, CurrentAssertionChain, Subject, TimeSpanCondition.LessThan,
            timeSpan);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> has the <paramref name="expected"/> date.
    /// </summary>
    /// <param name="expected">The expected date portion of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeSameDateAs(DateTime expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        DateTime expectedDate = expected.Date;

        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected the date part of {context:the date and time} to be {0}{reason}", expectedDate,
                chain => chain
                    .ForCondition(Subject.HasValue)
                    .FailWith(", but found a <null> DateTime.", expectedDate)
                    .Then
                    .ForCondition(Subject.Value.Date == expectedDate)
                    .FailWith(", but found {1}.", expectedDate, Subject.Value));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="DateTime"/> is not the <paramref name="unexpected"/> date.
    /// </summary>
    /// <param name="unexpected">The date that is not to match the date portion of the current value.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeSameDateAs(DateTime unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        DateTime unexpectedDate = unexpected.Date;

        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect the date part of {context:the date and time} to be {0}{reason}", unexpectedDate,
                chain => chain
                    .ForCondition(Subject.HasValue)
                    .FailWith(", but found a <null> DateTime.")
                    .Then
                    .ForCondition(Subject.Value.Date != unexpectedDate)
                    .FailWith(", but it was."));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the <see cref="DateTime"/> is one of the specified <paramref name="validValues"/>.
    /// </summary>
    /// <param name="validValues">
    /// The values that are valid.
    /// </param>
    public AndConstraint<TAssertions> BeOneOf(params DateTime?[] validValues)
    {
        return BeOneOf(validValues, string.Empty);
    }

    /// <summary>
    /// Asserts that the <see cref="DateTime"/> is one of the specified <paramref name="validValues"/>.
    /// </summary>
    /// <param name="validValues">
    /// The values that are valid.
    /// </param>
    public AndConstraint<TAssertions> BeOneOf(params DateTime[] validValues)
    {
        return BeOneOf(validValues.Cast<DateTime?>());
    }

    /// <summary>
    /// Asserts that the <see cref="DateTime"/> is one of the specified <paramref name="validValues"/>.
    /// </summary>
    /// <param name="validValues">
    /// The values that are valid.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeOneOf(IEnumerable<DateTime> validValues,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return BeOneOf(validValues.Cast<DateTime?>(), because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the <see cref="DateTime"/> is one of the specified <paramref name="validValues"/>.
    /// </summary>
    /// <param name="validValues">
    /// The values that are valid.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeOneOf(IEnumerable<DateTime?> validValues,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(validValues.Contains(Subject))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:date and time} to be one of {0}{reason}, but found {1}.", validValues, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the <see cref="DateTime"/> represents a value in the <paramref name="expectedKind"/>.
    /// </summary>
    /// <param name="expectedKind">
    /// The expected <see cref="DateTimeKind"/> that the current value must represent.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeIn(DateTimeKind expectedKind,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:the date and time} to be in " + expectedKind + "{reason}", chain => chain
                .ForCondition(Subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(Subject.Value.Kind == expectedKind)
                .FailWith(", but found " + Subject.Value.Kind + "."));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the <see cref="DateTime"/> does not represent a value in a specific kind.
    /// </summary>
    /// <param name="unexpectedKind">
    /// The <see cref="DateTimeKind"/> that the current value should not represent.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeIn(DateTimeKind unexpectedKind,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Did not expect {context:the date and time} to be in " + unexpectedKind + "{reason}", chain => chain
                .Given(() => Subject)
                .ForCondition(subject => subject.HasValue)
                .FailWith(", but found a <null> DateTime.")
                .Then
                .ForCondition(subject => subject.GetValueOrDefault().Kind != unexpectedKind)
                .FailWith(", but it was."));

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        throw new NotSupportedException("Equals is not part of Awesome Assertions. Did you mean Be() instead?");
}
