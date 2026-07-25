using System.Xml;
using AwesomeAssertions.Common;
using AwesomeAssertions.Formatting;

namespace AwesomeAssertions.Xml;

/// <summary>
/// Formats a <see cref="System.Xml.XmlNode"/> value for display in assertion failure messages.
/// </summary>
public class XmlNodeFormatter : IValueFormatter
{
    /// <inheritdoc />
    public bool CanHandle(object value)
    {
        return value is XmlNode;
    }

    /// <inheritdoc />
    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        string outerXml = ((XmlNode)value).OuterXml;

        const int maxLength = 20;

        if (outerXml.Length > maxLength)
        {
            outerXml = outerXml.Substring(0, maxLength).TrimEnd() + "…";
        }

        formattedGraph.AddLine(outerXml.EscapePlaceholders());
    }
}
