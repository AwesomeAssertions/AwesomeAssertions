using AwesomeAssertions.Configuration;
using Xunit;

namespace AwesomeAssertions.Specs.Configuration;

public class GlobalFormattingOptionsTests
{
    [Fact]
    public void When_cloning_all_properties_should_be_copied()
    {
        var sut = new GlobalFormattingOptions();
        sut.InitializeWithRandomData();

        var clone = sut.Clone();

        clone.Should().BeEquivalentTo(sut);
    }
}
