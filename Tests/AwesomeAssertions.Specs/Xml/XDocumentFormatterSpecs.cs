using System.Xml.Linq;
using AwesomeAssertions.Formatting;
using Xunit;

namespace AwesomeAssertions.Specs.Xml;

public class XDocumentFormatterSpecs
{
    [Fact]
    public void When_element_has_root_element_it_should_include_it_in_the_output()
    {
        // Act
        var document = XDocument.Parse(
            @"<configuration>
                     <startDate />
                     <endDate />
                  </configuration>");

        string result = Formatter.ToString(document);

        // Assert
        result.Should().Be("<configuration>…</configuration>");
    }

    [Fact]
    public void When_element_has_no_root_element_it_should_include_it_in_the_output()
    {
        // Act
        var document = new XDocument();

        string result = Formatter.ToString(document);

        // Assert
        result.Should().Be("[XML document without root element]");
    }
}
