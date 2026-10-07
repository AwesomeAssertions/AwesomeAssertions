using System;
using Xunit;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    [Fact]
    public void When_chaining_constraints_with_and_should_not_throw()
    {
        int value = 2;
        int greaterValue = 3;
        int smallerValue = 1;

        value.Should().BePositive()
            .And.BeGreaterThan(smallerValue)
            .And.BeLessThan(greaterValue);
    }

    [Fact]
    public void Should_throw_a_helpful_error_when_accidentally_using_equals()
    {
        int value = 1;

        Action action = () => value.Should().Equals(1);

        action.Should().Throw<NotSupportedException>()
            .WithMessage("Equals is not part of Awesome Assertions. Did you mean Be() instead?");
    }
}
