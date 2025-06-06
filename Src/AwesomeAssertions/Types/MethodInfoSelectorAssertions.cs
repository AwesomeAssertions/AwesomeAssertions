using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Types;

#pragma warning disable CS0659, S1206 // Ignore not overriding Object.GetHashCode()
#pragma warning disable CA1065 // Ignore throwing NotSupportedException from Equals
/// <summary>
/// Contains assertions for the <see cref="MethodInfo"/> objects returned by the parent <see cref="MethodInfoSelector"/>.
/// </summary>
[DebuggerNonUserCode]
public class MethodInfoSelectorAssertions
{
    private readonly AssertionChain assertionChain;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoSelectorAssertions"/> class.
    /// </summary>
    /// <param name="methods">The methods to assert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="methods"/> is <see langword="null"/>.</exception>
    public MethodInfoSelectorAssertions(AssertionChain assertionChain, params MethodInfo[] methods)
    {
        this.assertionChain = assertionChain;
        Guard.ThrowIfArgumentIsNull(methods);

        SubjectMethods = methods;
    }

    /// <summary>
    /// Provides access to the <see cref="AssertionChain"/> that this assertion class was initialized with.
    /// </summary>
    public AssertionChain CurrentAssertionChain { get; }

    /// <summary>
    /// Gets the object whose value is being asserted.
    /// </summary>
    public IEnumerable<MethodInfo> SubjectMethods { get; }

    /// <summary>
    /// Asserts that the selected methods are virtual.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> BeVirtual([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        MethodInfo[] nonVirtualMethods = GetAllNonVirtualMethodsFromSelection();

        assertionChain
            .ForCondition(nonVirtualMethods.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods to be virtual{reason}, but the following methods are not virtual:
                {{GetDescriptionsFor(nonVirtualMethods)}}
                """));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    /// <summary>
    /// Asserts that the selected methods are not virtual.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeVirtual([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        MethodInfo[] virtualMethods = GetAllVirtualMethodsFromSelection();

        assertionChain
            .ForCondition(virtualMethods.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods not to be virtual{reason}, but the following methods are virtual:
                {{GetDescriptionsFor(virtualMethods)}}
                """));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    private MethodInfo[] GetAllNonVirtualMethodsFromSelection()
    {
        return SubjectMethods.Where(method => method.IsNonVirtual()).ToArray();
    }

    private MethodInfo[] GetAllVirtualMethodsFromSelection()
    {
        return SubjectMethods.Where(method => !method.IsNonVirtual()).ToArray();
    }

    /// <summary>
    /// Asserts that the selected methods are async.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> BeAsync([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        MethodInfo[] nonAsyncMethods = SubjectMethods.Where(method => !method.IsAsync()).ToArray();

        assertionChain
            .ForCondition(nonAsyncMethods.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods to be async{reason}, but the following methods are not:
                {{GetDescriptionsFor(nonAsyncMethods)}}
                """));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    /// <summary>
    /// Asserts that the selected methods are not async.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeAsync([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        MethodInfo[] asyncMethods = SubjectMethods.Where(method => method.IsAsync()).ToArray();

        assertionChain
            .ForCondition(asyncMethods.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods not to be async{reason}, but the following methods are:
                {{GetDescriptionsFor(asyncMethods)}}
                """));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    /// <summary>
    /// Asserts that the selected methods are decorated with the specified <typeparamref name="TAttribute"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> BeDecoratedWith<TAttribute>(
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        return BeDecoratedWith<TAttribute>(_ => true, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the selected methods are decorated with an attribute of type <typeparamref name="TAttribute"/>
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
    public AndConstraint<MethodInfoSelectorAssertions> BeDecoratedWith<TAttribute>(
        Expression<Func<TAttribute, bool>> isMatchingAttributePredicate,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        Guard.ThrowIfArgumentIsNull(isMatchingAttributePredicate);

        MethodInfo[] methodsWithoutAttribute = GetMethodsWithout(isMatchingAttributePredicate);

        assertionChain
            .ForCondition(methodsWithoutAttribute.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods to be decorated with {0}{reason}, but the following methods are not:
                {{GetDescriptionsFor(methodsWithoutAttribute)}}
                """, typeof(TAttribute)));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    /// <summary>
    /// Asserts that the selected methods are not decorated with the specified <typeparamref name="TAttribute"/>.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeDecoratedWith<TAttribute>(
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        return NotBeDecoratedWith<TAttribute>(_ => true, because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the selected methods are not decorated with an attribute of type <typeparamref name="TAttribute"/>
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
    public AndConstraint<MethodInfoSelectorAssertions> NotBeDecoratedWith<TAttribute>(
        Expression<Func<TAttribute, bool>> isMatchingAttributePredicate,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
        where TAttribute : Attribute
    {
        Guard.ThrowIfArgumentIsNull(isMatchingAttributePredicate);

        MethodInfo[] methodsWithAttribute = GetMethodsWith(isMatchingAttributePredicate);

        assertionChain
            .ForCondition(methodsWithAttribute.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods to not be decorated with {0}{reason}, but the following methods are:
                {{GetDescriptionsFor(methodsWithAttribute)}}
                """, typeof(TAttribute)));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    /// <summary>
    /// Asserts that the selected methods have specified <paramref name="accessModifier"/>.
    /// </summary>
    /// <param name="accessModifier">The expected access modifier.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> Be(CSharpAccessModifier accessModifier,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        MethodInfo[] methods = SubjectMethods.Where(pi => pi.GetCSharpAccessModifier() != accessModifier).ToArray();

        assertionChain
            .ForCondition(methods.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods to be {{accessModifier}}{reason}, but the following methods are not:
                {{GetDescriptionsFor(methods)}}
                """));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    /// <summary>
    /// Asserts that the selected methods don't have specified <paramref name="accessModifier"/>
    /// </summary>
    /// <param name="accessModifier">The expected access modifier.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<MethodInfoSelectorAssertions> NotBe(CSharpAccessModifier accessModifier,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        MethodInfo[] methods = SubjectMethods.Where(pi => pi.GetCSharpAccessModifier() == accessModifier).ToArray();

        assertionChain
            .ForCondition(methods.Length == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(() => new FailReason(
                $$"""
                Expected all selected methods to not be {{accessModifier}}{reason}, but the following methods are:"
                {{GetDescriptionsFor(methods)}}
                """));

        return new AndConstraint<MethodInfoSelectorAssertions>(this);
    }

    private MethodInfo[] GetMethodsWithout<TAttribute>(Expression<Func<TAttribute, bool>> isMatchingPredicate)
        where TAttribute : Attribute
    {
        return SubjectMethods.Where(method => !method.IsDecoratedWith(isMatchingPredicate)).ToArray();
    }

    private MethodInfo[] GetMethodsWith<TAttribute>(Expression<Func<TAttribute, bool>> isMatchingPredicate)
        where TAttribute : Attribute
    {
        return SubjectMethods.Where(method => method.IsDecoratedWith(isMatchingPredicate)).ToArray();
    }

    private static string GetDescriptionsFor(IEnumerable<MethodInfo> methods)
    {
        IEnumerable<string> descriptions = methods.Select(method => MethodInfoAssertions.GetDescriptionFor(method));

        return string.Join(Environment.NewLine, descriptions);
    }

    /// <summary>
    /// Returns the type of the subject the assertion applies on.
    /// </summary>
#pragma warning disable CA1822 // Do not change signature of a public member
    protected string Context => "method";
#pragma warning restore CA1822

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        throw new NotSupportedException("Equals is not part of Awesome Assertions. Did you mean Be() instead?");
}
