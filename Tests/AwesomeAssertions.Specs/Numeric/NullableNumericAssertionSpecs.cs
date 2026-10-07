using Xunit;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    [Fact]
    public void Should_support_chaining_constraints_with_and()
    {
        int? nullableInteger = 1;

        nullableInteger.Should().HaveValue()
            .And.BePositive();
    }
}
