using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BePositive
    {
        [Fact]
        public void When_a_positive_value_is_positive_it_should_not_throw()
        {
            float value = 1F;

            value.Should().BePositive();
        }

        [Fact]
        public void When_a_negative_value_is_positive_it_should_throw()
        {
            double value = -1D;

            Action act = () => value.Should().BePositive();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_zero_value_is_positive_it_should_throw()
        {
            int value = 0;

            Action act = () => value.Should().BePositive();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void NaN_is_never_a_positive_float()
        {
            float value = float.NaN;

            Action act = () => value.Should().BePositive();

            act.Should().Throw<XunitException>().WithMessage("*but found NaN*");
        }

        [Fact]
        public void NaN_is_never_a_positive_double()
        {
            double value = double.NaN;

            Action act = () => value.Should().BePositive();

            act.Should().Throw<XunitException>().WithMessage("*but found NaN*");
        }

        [Fact]
        public void When_a_negative_value_is_positive_it_should_throw_with_descriptive_message()
        {
            int value = -1;

            Action act = () => value.Should().BePositive("we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be positive because we want to test the failure message, but found -1.");
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_positive_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().BePositive();

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }
    }
}
