using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Extensions;

namespace AwesomeAssertions.Primitives;

#pragma warning disable CS0659, S1206 // Ignore not overriding Object.GetHashCode()
#pragma warning disable CA1065 // Ignore throwing NotSupportedException from Equals
/// <summary>
/// Contains a number of methods to assert that two <see cref="DateTime"/> objects differ in the expected way.
/// </summary>
/// <remarks>
/// You can use the <see cref="FluentDateTimeExtensions"/> and <see cref="FluentTimeSpanExtensions"/>
/// for a more fluent way of specifying a <see cref="DateTime"/> or a <see cref="TimeSpan"/>.
/// </remarks>
[DebuggerNonUserCode]
public class DateTimeOffsetRangeAssertions<TAssertions>
    where TAssertions : DateTimeOffsetAssertions<TAssertions>
{
    #region Private Definitions

    private readonly TAssertions parentAssertions;
    private readonly TimeSpanPredicate predicate;

    private readonly Dictionary<TimeSpanCondition, TimeSpanPredicate> predicates = new()
    {
        [TimeSpanCondition.MoreThan] = new TimeSpanPredicate((ts1, ts2) => ts1 > ts2, "more than"),
        [TimeSpanCondition.AtLeast] = new TimeSpanPredicate((ts1, ts2) => ts1 >= ts2, "at least"),
        [TimeSpanCondition.Exactly] = new TimeSpanPredicate((ts1, ts2) => ts1 == ts2, "exactly"),
        [TimeSpanCondition.Within] = new TimeSpanPredicate((ts1, ts2) => ts1 <= ts2, "within"),
        [TimeSpanCondition.LessThan] = new TimeSpanPredicate((ts1, ts2) => ts1 < ts2, "less than")
    };

    private readonly DateTimeOffset? subject;
    private readonly TimeSpan timeSpan;

    #endregion

    protected internal DateTimeOffsetRangeAssertions(TAssertions parentAssertions, AssertionChain assertionChain,
        DateTimeOffset? subject,
        TimeSpanCondition condition,
        TimeSpan timeSpan)
    {
        this.parentAssertions = parentAssertions;
        CurrentAssertionChain = assertionChain;
        this.subject = subject;
        this.timeSpan = timeSpan;

        predicate = predicates[condition];
    }

    /// <summary>
    /// Provides access to the <see cref="AssertionChain"/> that this assertion class was initialized with.
    /// </summary>
    public AssertionChain CurrentAssertionChain { get; }

    /// <summary>
    /// Asserts that a <see cref="DateTimeOffset"/> occurs a specified amount of time before another <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="target">
    /// The <see cref="DateTimeOffset"/> to compare the subject with.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<TAssertions> Before(DateTimeOffset target,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:the date and time} to be " + predicate.DisplayText +
                " {0} before {1}{reason}, but found a <null> DateTime.", timeSpan, target);

        if (CurrentAssertionChain.Succeeded)
        {
            TimeSpan actual = target - subject.Value;

            CurrentAssertionChain
                .ForCondition(predicate.IsMatchedBy(actual, timeSpan))
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    "Expected {context:the date and time} {0} to be " + predicate.DisplayText +
                    " {1} before {2}{reason}, but it is " + PositionRelativeToTarget(subject.Value, target) + " by {3}.",
                    subject, timeSpan, target, actual.Duration());
        }

        return new AndConstraint<TAssertions>(parentAssertions);
    }

    /// <summary>
    /// Asserts that a <see cref="DateTimeOffset"/> occurs a specified amount of time after another <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="target">
    /// The <see cref="DateTimeOffset"/> to compare the subject with.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<TAssertions> After(DateTimeOffset target,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(subject.HasValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:the date and time} to be " + predicate.DisplayText +
                " {0} after {1}{reason}, but found a <null> DateTime.", timeSpan, target);

        if (CurrentAssertionChain.Succeeded)
        {
            TimeSpan actual = subject.Value - target;

            CurrentAssertionChain
                .ForCondition(predicate.IsMatchedBy(actual, timeSpan))
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    "Expected {context:the date and time} {0} to be " + predicate.DisplayText +
                    " {1} after {2}{reason}, but it is " + PositionRelativeToTarget(subject.Value, target) + " by {3}.",
                    subject, timeSpan, target, actual.Duration());
        }

        return new AndConstraint<TAssertions>(parentAssertions);
    }

    private static string PositionRelativeToTarget(DateTimeOffset actual, DateTimeOffset target)
    {
        return (actual - target) >= TimeSpan.Zero ? "ahead" : "behind";
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        throw new NotSupportedException("Equals is not part of Awesome Assertions. Did you mean Before() or After() instead?");
}
