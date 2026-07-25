namespace AwesomeAssertions.Equivalency.Steps;

/// <summary>
/// Proves the equivalency of the subject and expectation when they refer to the same object instance.
/// </summary>
public class ReferenceEqualityEquivalencyStep : IEquivalencyStep
{
    /// <inheritdoc />
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency valueChildNodes)
    {
        return ReferenceEquals(comparands.Subject, comparands.Expectation)
            ? EquivalencyResult.EquivalencyProven
            : EquivalencyResult.ContinueWithNext;
    }
}
