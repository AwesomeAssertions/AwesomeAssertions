using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeOneOf
    {
        [Fact]
        public void When_a_value_is_not_one_of_the_specified_values_it_should_throw()
        {
            int value = 3;

            Action act = () => value.Should().BeOneOf(4, 5);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be one of {4, 5}, but found 3.");
        }

        [Fact]
        public void When_a_value_is_not_one_of_the_specified_values_it_should_throw_with_descriptive_message()
        {
            int value = 3;

            Action act = () => value.Should().BeOneOf([4, 5], "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be one of {4, 5} because*failure message, but found 3.");
        }

        [Fact]
        public void When_a_value_is_one_of_the_specified_values_it_should_succeed()
        {
            int value = 4;

            value.Should().BeOneOf(4, 5);
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_one_of_to_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().BeOneOf(0, 1);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }

        [Fact]
        public void Two_floats_that_are_NaN_can_be_compared()
        {
            float value = float.NaN;

            value.Should().BeOneOf(float.NaN, 4.5F);
        }

        [Fact]
        public void Floats_are_never_equal_to_NaN()
        {
            float value = float.NaN;

            Action act = () => value.Should().BeOneOf(1.5F, 4.5F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected*1.5F*found*NaN*");
        }

        [Fact]
        public void Two_doubles_that_are_NaN_can_be_compared()
        {
            double value = double.NaN;

            value.Should().BeOneOf(double.NaN, 4.5F);
        }

        [Fact]
        public void Doubles_are_never_equal_to_NaN()
        {
            double value = double.NaN;

            Action act = () => value.Should().BeOneOf(1.5D, 4.5D);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected*1.5*found NaN*");
        }
    }
}
