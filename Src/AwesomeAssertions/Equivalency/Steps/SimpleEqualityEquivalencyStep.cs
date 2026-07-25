using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Equivalency.Steps;

/// <summary>
/// Asserts the equality of the subject and expectation using their <see cref="object.Equals(object)"/> implementation
/// when the comparison is non-recursive and the current node is not the root.
/// </summary>
public class SimpleEqualityEquivalencyStep : IEquivalencyStep
{
    /// <inheritdoc />
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency valueChildNodes)
    {
        if (!context.Options.IsRecursive && !context.CurrentNode.IsRoot)
        {
            AssertionChain.GetOrCreate()
                .For(context)
                .ReuseOnce();

            comparands.Subject.Should().Be(comparands.Expectation, context.Reason.FormattedMessage, context.Reason.Arguments);

            return EquivalencyResult.EquivalencyProven;
        }

        return EquivalencyResult.ContinueWithNext;
    }
}
