using System.Xml;
using AwesomeAssertions.Common;
using AwesomeAssertions.Formatting;

namespace AwesomeAssertions.Xml;

public class XmlNodeFormatter : IValueFormatter
{
    public bool CanHandle(object value)
    {
        return value is XmlNode;
    }

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
