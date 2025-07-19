using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Xml;

namespace AwesomeAssertions;

[DebuggerNonUserCode]
public static class XmlAssertionExtensions
{
    public static XmlNodeAssertions Should([NotNull] this XmlNode actualValue)
    {
        return new XmlNodeAssertions(actualValue, AssertionChain.GetOrCreate());
    }

    public static XmlElementAssertions Should([NotNull] this XmlElement actualValue)
    {
        return new XmlElementAssertions(actualValue, AssertionChain.GetOrCreate());
    }
}
