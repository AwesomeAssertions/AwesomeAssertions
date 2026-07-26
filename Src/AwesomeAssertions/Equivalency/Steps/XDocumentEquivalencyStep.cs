using System.Xml.Linq;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Equivalency.Steps;

/// <summary>
/// Asserts the equivalency of two <see cref="XDocument"/> instances.
/// </summary>
public class XDocumentEquivalencyStep : EquivalencyStep<XDocument>
{
    /// <inheritdoc />
    protected override EquivalencyResult OnHandle(Comparands comparands,
        IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency nestedValidator)
    {
        var subject = (XDocument)comparands.Subject;
        var expectation = (XDocument)comparands.Expectation;

        AssertionChain.GetOrCreate().For(context).ReuseOnce();

        subject.Should().BeEquivalentTo(expectation, context.Reason.FormattedMessage, context.Reason.Arguments);

        return EquivalencyResult.EquivalencyProven;
    }
}
