namespace AwesomeAssertions.Equivalency;

/// <summary>
///  Convenient implementation of <see cref="IEquivalencyStep"/> that will only invoke
/// </summary>
public abstract class EquivalencyStep<T> : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency valueChildNodes)
    {
        if (!typeof(T).IsAssignableFrom(comparands.GetExpectedType(context.Options)))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        return OnHandle(comparands, context, valueChildNodes);
    }

    /// <summary>
    /// Implements <see cref="IEquivalencyStep.Handle"/>, but only gets called when the expected type matches <typeparamref name="T"/>.
    /// </summary>
    protected abstract EquivalencyResult OnHandle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency nestedValidator);
}
