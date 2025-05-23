using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringValidatorSupportingNull
{
    private readonly IStringComparisonStrategy comparisonStrategy;
    private AssertionChain assertionChain;

    public StringValidatorSupportingNull(AssertionChain assertionChain, IStringComparisonStrategy comparisonStrategy,
        [StringSyntax("CompositeFormat")] string because, object[] becauseArgs)
    {
        this.comparisonStrategy = comparisonStrategy;
        this.assertionChain = assertionChain.BecauseOf(because, becauseArgs);
    }

    public void Validate(string subject, string expected)
    {
        if (expected?.IsLongOrMultiline() == true ||
            subject?.IsLongOrMultiline() == true)
        {
            assertionChain = assertionChain.UsingLineBreaks;
        }

        comparisonStrategy.ValidateAgainstMismatch(assertionChain, subject, expected);
    }
}
