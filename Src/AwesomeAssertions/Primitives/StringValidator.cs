using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringValidator
{
    private readonly IStringComparisonStrategy comparisonStrategy;
    private readonly AssertionChain assertionChain;

    public StringValidator(AssertionChain assertionChain, IStringComparisonStrategy comparisonStrategy, string because,
        object[] becauseArgs)
    {
        this.comparisonStrategy = comparisonStrategy;
        this.assertionChain = assertionChain.BecauseOf(because, becauseArgs);
    }

    public void Validate(string subject, string expected) =>
        comparisonStrategy.ValidateAgainstMismatch(assertionChain, subject, expected);
}
