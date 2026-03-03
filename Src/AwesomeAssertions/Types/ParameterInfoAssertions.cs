using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace AwesomeAssertions.Types;

/// <summary>
/// Contains a number of methods to assert that a <see cref="ParameterInfo"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public sealed class ParameterInfoAssertions : ReferenceTypeAssertions<ParameterInfo, ParameterInfoAssertions>
{
    private readonly AssertionChain assertionChain;

    protected override string Identifier => "parameter";

    private string SubjectDescription =>
        Subject.Position < 0
            ? $"return parameter {Subject.ParameterType.ToFormattedString()}"
            : $"parameter {Subject.ParameterType.ToFormattedString()} {Subject.Name}";

    public ParameterInfoAssertions(ParameterInfo subject, AssertionChain assertionChain)
        : base(subject, assertionChain)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Asserts that the selected parameter is decorated with the specified <typeparamref name="TAttribute"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <typeparam name="TAttribute">Expected attribute type.</typeparam>
    [return: NotNull]
    public AndWhichConstraint<ParameterInfoAssertions, TAttribute> BeDecoratedWith<TAttribute>(
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        IEnumerable<TAttribute> attributes = AssertDecoratedWith<TAttribute>(because, becauseArgs);
        return new(this, attributes);
    }

    /// <summary>
    /// Asserts that the selected parameter is decorated with an attribute of type <typeparamref name="TAttribute"/>
    /// that matches the specified <paramref name="isMatchingAttributePredicate"/>.
    /// </summary>
    /// <param name="isMatchingAttributePredicate">
    /// The predicate that the attribute must match.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="isMatchingAttributePredicate"/> is <see langword="null"/>.</exception>
    /// <typeparam name="TAttribute">Expected attribute type.</typeparam>
    [return: NotNull]
    public AndWhichConstraint<ParameterInfoAssertions, TAttribute> BeDecoratedWith<TAttribute>(
        Expression<Func<TAttribute, bool>> isMatchingAttributePredicate,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        Guard.ThrowIfArgumentIsNull(isMatchingAttributePredicate);

        IEnumerable<TAttribute> attributes = AssertDecoratedWith<TAttribute>(because, becauseArgs);

        if (assertionChain.Succeeded)
        {
            attributes = attributes.Where(isMatchingAttributePredicate.Compile()).ToArray();

            assertionChain
                .ForCondition(attributes.Any())
                .FailWith(() => new FailReason(
                    $"Expected {SubjectDescription} to be decorated with {{0}} matching {{1}}{{reason}}" +
                    ", but no attribute matching the predicate was found.", typeof(TAttribute), isMatchingAttributePredicate.Body));
        }

        return new(this, attributes);
    }

    private IEnumerable<TAttribute> AssertDecoratedWith<TAttribute>(string because, object[] becauseArgs)
        where TAttribute : Attribute
    {
        assertionChain
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject is not null)
                    .FailWith(
                        "Expected parameter to be decorated with {0}{reason}" +
                        ", but found {context:parameter} is <null>.", typeof(TAttribute));

        IEnumerable<TAttribute> attributes = [];

        if (assertionChain.Succeeded)
        {
            attributes = Subject.GetCustomAttributes<TAttribute>();

            assertionChain
                .ForCondition(attributes.Any())
                .FailWith(() => new FailReason(
                    $"Expected {SubjectDescription} to be decorated with {{0}}{{reason}}" +
                    ", but that attribute was not found.", typeof(TAttribute)));
        }

        return attributes;
    }

    /// <summary>
    /// Asserts that the selected parameter is not decorated with the specified <typeparamref name="TAttribute"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <typeparam name="TAttribute">Unexpected attribute type.</typeparam>
    [return: NotNull]
    public AndConstraint<ParameterInfoAssertions> NotBeDecoratedWith<TAttribute>(
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        assertionChain
           .BecauseOf(because, becauseArgs)
           .ForCondition(Subject is not null)
           .FailWith(
               "Did not expect parameter to be decorated with {0}{reason}" +
               ", but found {context:parameter} is <null>.", typeof(TAttribute));

        if (assertionChain.Succeeded)
        {
            IEnumerable<TAttribute> attributes = Subject.GetCustomAttributes<TAttribute>();

            assertionChain
                .ForCondition(!attributes.Any())
                .FailWith(() => new FailReason(
                    $"Did not expect {SubjectDescription} to be decorated with {{0}}{{reason}}" +
                    ", but that attribute was found.", typeof(TAttribute)));
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the selected parameter is not decorated with an attribute of type <typeparamref name="TAttribute"/>
    /// that matches the specified <paramref name="isMatchingAttributePredicate"/>.
    /// </summary>
    /// <param name="isMatchingAttributePredicate">
    /// The predicate that the attribute must match.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="isMatchingAttributePredicate"/> is <see langword="null"/>.</exception>
    /// <typeparam name="TAttribute">Unexpected attribute type.</typeparam>
    [return: NotNull]
    public AndConstraint<ParameterInfoAssertions> NotBeDecoratedWith<TAttribute>(
        Expression<Func<TAttribute, bool>> isMatchingAttributePredicate,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        Guard.ThrowIfArgumentIsNull(isMatchingAttributePredicate);

        assertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject is not null)
            .FailWith(
                "Did not expect parameter to be decorated with {0}{reason}" +
                ", but found {context:parameter} is <null>.", typeof(TAttribute));

        if (assertionChain.Succeeded)
        {
            IEnumerable<TAttribute> attributes = Subject.GetCustomAttributes<TAttribute>()
                .Where(isMatchingAttributePredicate.Compile()).ToArray();

            assertionChain
                .ForCondition(!attributes.Any())
                .FailWith(() => new FailReason(
                    $"Did not expect {SubjectDescription} to be decorated with {{0}} matching {{1}}{{reason}}" +
                    ", but that attribute was found.", typeof(TAttribute), isMatchingAttributePredicate.Body));
        }

        return new(this);
    }
}
