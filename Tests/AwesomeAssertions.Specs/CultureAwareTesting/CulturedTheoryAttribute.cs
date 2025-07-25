using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.CultureAwareTesting;

[XunitTestCaseDiscoverer("AwesomeAssertions.Specs.CultureAwareTesting.CulturedTheoryAttributeDiscoverer",
    "AwesomeAssertions.Specs")]
public sealed class CulturedTheoryAttribute : TheoryAttribute
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public CulturedTheoryAttribute(params string[] _) { }
#pragma warning restore CA1019 // Define accessors for attribute arguments
}
