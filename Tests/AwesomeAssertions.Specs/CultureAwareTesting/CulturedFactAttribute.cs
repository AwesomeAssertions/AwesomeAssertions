using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.CultureAwareTesting;

[XunitTestCaseDiscoverer("AwesomeAssertions.Specs.CultureAwareTesting.CulturedFactAttributeDiscoverer", "AwesomeAssertions.Specs")]
public sealed class CulturedFactAttribute : FactAttribute
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public CulturedFactAttribute(params string[] _) { }
#pragma warning restore CA1019 // Define accessors for attribute arguments
}
