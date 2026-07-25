using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Xml;

namespace AwesomeAssertions;

/// <summary>
/// Contains extension methods for asserting the state of <see cref="XmlNode"/> and <see cref="XmlElement"/> objects.
/// </summary>
[DebuggerNonUserCode]
public static class XmlAssertionExtensions
{
    /// <summary>
    /// Returns an <see cref="XmlNodeAssertions"/> object that can be used to assert the
    /// current <see cref="XmlNode"/>.
    /// </summary>
    /// <param name="actualValue">The <see cref="XmlNode"/> to assert on.</param>
    /// <returns>An <see cref="XmlNodeAssertions"/> object for asserting on <paramref name="actualValue"/>.</returns>
    [return: NotNull]
    public static XmlNodeAssertions Should([NotNull] this XmlNode actualValue)
    {
        return new XmlNodeAssertions(actualValue, AssertionChain.GetOrCreate());
    }

    /// <summary>
    /// Returns an <see cref="XmlElementAssertions"/> object that can be used to assert the
    /// current <see cref="XmlElement"/>.
    /// </summary>
    /// <param name="actualValue">The <see cref="XmlElement"/> to assert on.</param>
    /// <returns>An <see cref="XmlElementAssertions"/> object for asserting on <paramref name="actualValue"/>.</returns>
    [return: NotNull]
    public static XmlElementAssertions Should([NotNull] this XmlElement actualValue)
    {
        return new XmlElementAssertions(actualValue, AssertionChain.GetOrCreate());
    }
}
