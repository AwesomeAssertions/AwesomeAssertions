using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AwesomeAssertions.Common;
using AwesomeAssertions.Equivalency;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that an <see cref="IComparable{T}"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class ComparableTypeAssertions<T> : ComparableTypeAssertions<T, ComparableTypeAssertions<T>>
{
    public ComparableTypeAssertions(IComparable<T> value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }
}

/// <summary>
/// Contains a number of methods to assert that an <see cref="IComparable{T}"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class ComparableTypeAssertions<T, TAssertions> : ReferenceTypeAssertions<IComparable<T>, TAssertions>
    where TAssertions : ComparableTypeAssertions<T, TAssertions>
{
    private const int Equal = 0;
    private readonly AssertionChain assertionChain;

    public ComparableTypeAssertions(IComparable<T> value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Asserts that an object equals another object using its <see cref="object.Equals(object)" /> implementation.<br/>
    /// Verification whether <see cref="IComparable{T}.CompareTo(T)"/> returns 0 is not done here, you should use
    /// <see cref="BeRankedEquallyTo(T, string, object[])"/> to verify this.
    /// </summary>
    /// <param name="expected">The expected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> Be(T expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Equals(Subject, expected))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} to be equal to {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that an object is equivalent to another object.
    /// </summary>
    /// <remarks>
    /// Objects are equivalent when both object graphs have equally named properties with the same value,
    /// irrespective of the type of those objects. Two properties are also equal if one type can be converted to another and the result is equal.
    /// Notice that actual behavior is determined by the global defaults managed by <see cref="AssertionConfiguration"/>.
    /// </remarks>
    /// <param name="expectation">The expected element.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<TAssertions> BeEquivalentTo<TExpectation>(TExpectation expectation,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        return BeEquivalentTo(expectation, config => config, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that an object is equivalent to another object.
    /// </summary>
    /// <remarks>
    /// Objects are equivalent when both object graphs have equally named properties with the same value,
    /// irrespective of the type of those objects. Two properties are also equal if one type can be converted to another and the result is equal.
    /// </remarks>
    /// <param name="expectation">The expected element.</param>
    /// <param name="config">
    /// A reference to the <see cref="EquivalencyOptions{TExpectation}"/> configuration object that can be used
    /// to influence the way the object graphs are compared. You can also provide an alternative instance of the
    /// <see cref="EquivalencyOptions{TExpectation}"/> class. The global defaults are determined by the
    /// <see cref="AssertionConfiguration"/> class.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public AndConstraint<TAssertions> BeEquivalentTo<TExpectation>(TExpectation expectation,
        Func<EquivalencyOptions<TExpectation>, EquivalencyOptions<TExpectation>> config,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        Guard.ThrowIfArgumentIsNull(config);

        EquivalencyOptions<TExpectation> options = config(AssertionConfiguration.Current.Equivalency.CloneDefaults<TExpectation>());

        var context = new EquivalencyValidationContext(
            Node.From<TExpectation>(() => CurrentAssertionChain.CallerIdentifier), options)
        {
            Reason = new Reason(because, becauseArgs),
            TraceWriter = options.TraceWriter
        };

        var comparands = new Comparands
        {
            Subject = Subject,
            Expectation = expectation,
            CompileTimeType = typeof(TExpectation),
        };

        new EquivalencyValidator().AssertEquality(comparands, context);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that an object does not equal another object using its <see cref="object.Equals(object)" /> method.<br/>
    /// Verification whether <see cref="IComparable{T}.CompareTo(T)"/> returns non-zero is not done here, you should use
    /// <see cref="NotBeRankedEquallyTo(T, string, object[])"/> to verify this.
    /// </summary>
    /// <param name="unexpected">The unexpected value</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<TAssertions> NotBe(T unexpected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(!Equals(Subject, unexpected))
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {context:object} to be equal to {0}{reason}.", unexpected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the subject is ranked equal to another object. I.e. the result of <see cref="IComparable{T}.CompareTo"/> returns 0.
    /// To verify whether the objects are equal you must use <see cref="Be(T, string, object[])"/>.
    /// </summary>
    /// <param name="expected">
    /// The object to pass to the subject's <see cref="IComparable{T}.CompareTo"/> method.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<TAssertions> BeRankedEquallyTo(T expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(expected) == Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} {0} to be ranked as equal to {1}{reason}.", Subject, expected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the subject is not ranked equal to another object. I.e. the result of <see cref="IComparable{T}.CompareTo"/>returns non-zero.
    /// To verify whether the objects are not equal according to <see cref="object.Equals(object)"/> you must use <see cref="NotBe(T, string, object[])"/>.
    /// </summary>
    /// <param name="unexpected">
    /// The object to pass to the subject's <see cref="IComparable{T}.CompareTo"/> method.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeRankedEquallyTo(T unexpected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(unexpected) != Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} {0} not to be ranked as equal to {1}{reason}.", Subject, unexpected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the subject is less than another object according to its implementation of <see cref="IComparable{T}"/>.
    /// </summary>
    /// <param name="expected">
    /// The object to pass to the subject's <see cref="IComparable{T}.CompareTo"/> method.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeLessThan(T expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(expected) < Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} {0} to be less than {1}{reason}.", Subject, expected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the subject is less than or equal to another object according to its implementation of <see cref="IComparable{T}"/>.
    /// </summary>
    /// <param name="expected">
    /// The object to pass to the subject's <see cref="IComparable{T}.CompareTo"/> method.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeLessThanOrEqualTo(T expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(expected) <= Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} {0} to be less than or equal to {1}{reason}.", Subject, expected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the subject is greater than another object according to its implementation of <see cref="IComparable{T}"/>.
    /// </summary>
    /// <param name="expected">
    /// The object to pass to the subject's <see cref="IComparable{T}.CompareTo"/> method.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeGreaterThan(T expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(expected) > Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} {0} to be greater than {1}{reason}.", Subject, expected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the subject is greater than or equal to another object according to its implementation of <see cref="IComparable{T}"/>.
    /// </summary>
    /// <param name="expected">
    /// The object to pass to the subject's <see cref="IComparable{T}.CompareTo"/> method.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeGreaterThanOrEqualTo(T expected, [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(expected) >= Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} {0} to be greater than or equal to {1}{reason}.", Subject, expected);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a value is within a range.
    /// </summary>
    /// <remarks>
    /// Where the range is continuous or incremental depends on the actual type of the value.
    /// </remarks>
    /// <param name="minimumValue">
    /// The minimum valid value of the range.
    /// </param>
    /// <param name="maximumValue">
    /// The maximum valid value of the range.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeInRange(T minimumValue, T maximumValue,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(Subject.CompareTo(minimumValue) >= Equal && Subject.CompareTo(maximumValue) <= Equal)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} to be between {0} and {1}{reason}, but found {2}.",
                minimumValue, maximumValue, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a value is not within a range.
    /// </summary>
    /// <remarks>
    /// Where the range is continuous or incremental depends on the actual type of the value.
    /// </remarks>
    /// <param name="minimumValue">
    /// The minimum valid value of the range.
    /// </param>
    /// <param name="maximumValue">
    /// The maximum valid value of the range.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> NotBeInRange(T minimumValue, T maximumValue,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(!(Subject.CompareTo(minimumValue) >= Equal && Subject.CompareTo(maximumValue) <= Equal))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} to not be between {0} and {1}{reason}, but found {2}.",
                minimumValue, maximumValue, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that a value is one of the specified <paramref name="validValues"/>.
    /// </summary>
    /// <param name="validValues">
    /// The values that are valid.
    /// </param>
    public AndConstraint<TAssertions> BeOneOf(params T[] validValues)
    {
        return BeOneOf(validValues, string.Empty);
    }

    /// <summary>
    /// Asserts that a value is one of the specified <paramref name="validValues"/>.
    /// </summary>
    /// <param name="validValues">
    /// The values that are valid.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])"/> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<TAssertions> BeOneOf(IEnumerable<T> validValues,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .ForCondition(validValues.Any(val => Equals(Subject, val)))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:object} to be one of {0}{reason}, but found {1}.", validValues, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Returns the type of the subject the assertion applies on.
    /// </summary>
    protected override string Identifier => "object";
}
