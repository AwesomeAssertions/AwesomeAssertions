using System.Xml.Linq;

namespace AwesomeAssertions.Formatting;

/// <summary>
/// Formats a <see cref="System.Xml.Linq.XDocument"/> value.
/// </summary>
public class XDocumentValueFormatter : IValueFormatter
{
    /// <inheritdoc />
    public bool CanHandle(object value)
    {
        return value is XDocument;
    }

    /// <inheritdoc />
    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var document = (XDocument)value;

        if (document.Root is not null)
        {
            formatChild("root", document.Root, formattedGraph);
        }
        else
        {
            formattedGraph.AddFragment("[XML document without root element]");
        }
    }
}
