using AwesomeAssertions.Formatting;
using Xunit;

namespace AwesomeAssertions.Specs.Formatting;

public class FormattingOptionsTests
{
    [Fact]
    public void When_cloning_all_properties_should_be_copied()
    {
        var sut = new FormattingOptions();
        sut.InitializeWithRandomData();

        var clone = sut.Clone();

        clone.Should().BeEquivalentTo(sut);
    }
}
