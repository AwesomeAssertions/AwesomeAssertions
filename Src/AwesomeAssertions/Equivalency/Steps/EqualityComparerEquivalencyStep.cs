using System;
using System.Collections.Generic;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Equivalency.Steps;

/// <summary>
/// Asserts the equivalency of objects of type <typeparamref name="T"/> using a user-supplied
/// <see cref="IEqualityComparer{T}"/> instead of the default structural comparison.
/// </summary>
public class EqualityComparerEquivalencyStep<T> : IEquivalencyStep
{
    private readonly IEqualityComparer<T> comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityComparerEquivalencyStep{T}"/> class.
    /// </summary>
    /// <param name="comparer">The equality comparer used to compare objects of type <typeparamref name="T"/>.</param>
    public EqualityComparerEquivalencyStep(IEqualityComparer<T> comparer)
    {
        this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    /// <inheritdoc />
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency valueChildNodes)
    {
        var expectedType = context.Options.UseRuntimeTyping ? comparands.RuntimeType : comparands.CompileTimeType;

        if (expectedType != typeof(T))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        if (comparands.Subject is null || comparands.Expectation is null)
        {
            // The later check for `comparands.Subject is T` leads to a failure even if the expectation is null.
            return EquivalencyResult.ContinueWithNext;
        }

        AssertionChain.GetOrCreate()
            .For(context)
            .BecauseOf(context.Reason.FormattedMessage, context.Reason.Arguments)
            .ForCondition(comparands.Subject is T)
            .FailWith("Expected {context:object} to be of type {0}{reason}, but found {1}", typeof(T), comparands.Subject)
            .Then
            .Given(() => comparer.Equals((T)comparands.Subject, (T)comparands.Expectation))
            .ForCondition(isEqual => isEqual)
            .FailWith("Expected {context:object} to be equal to {1} according to {0}{reason}, but {2} was not.",
                comparer.ToString(), comparands.Expectation, comparands.Subject);

        return EquivalencyResult.EquivalencyProven;
    }

    /// <summary>
    /// Returns a human-readable description of the comparer and the type it is applied to.
    /// </summary>
    public override string ToString()
    {
        return $"Use {comparer} for objects of type {typeof(T).ToFormattedString()}";
    }
}
