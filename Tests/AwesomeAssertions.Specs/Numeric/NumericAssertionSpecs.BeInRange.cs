using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeInRange
    {
        [Fact]
        public void When_a_value_is_outside_a_range_it_should_throw()
        {
            float value = 3.99F;

            Action act = () => value.Should().BeInRange(4, 5, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to be between*4* and*5* because*failure message, but found*3.99*");
        }

        [Fact]
        public void When_a_value_is_inside_a_range_it_should_not_throw()
        {
            int value = 4;

            value.Should().BeInRange(3, 5);
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_in_range_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().BeInRange(0, 1);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }

        [Fact]
        public void NaN_is_never_in_range_of_two_floats()
        {
            float value = float.NaN;

            Action act = () => value.Should().BeInRange(4, 5);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be between*4* and*5*, but found*NaN*");
        }

        [Theory]
        [InlineData(float.NaN, 5F)]
        [InlineData(5F, float.NaN)]
        public void A_float_can_never_be_in_a_range_containing_NaN(float minimumValue, float maximumValue)
        {
            float value = 4.5F;

            Action act = () => value.Should().BeInRange(minimumValue, maximumValue);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_NaN_is_never_in_range_of_two_doubles()
        {
            double value = double.NaN;

            Action act = () => value.Should().BeInRange(4, 5);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be between*4* and*5*, but found*NaN*");
        }

        [Theory]
        [InlineData(double.NaN, 5)]
        [InlineData(5, double.NaN)]
        public void A_double_can_never_be_in_a_range_containing_NaN(double minimumValue, double maximumValue)
        {
            double value = 4.5D;

            Action act = () => value.Should().BeInRange(minimumValue, maximumValue);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }
    }

    public class NotBeInRange
    {
        [Fact]
        public void When_a_value_is_inside_an_unexpected_range_it_should_throw()
        {
            float value = 4.99F;

            Action act = () => value.Should().NotBeInRange(4, 5, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to not be between*4* and*5* because*failure message, but found*4.99*");
        }

        [Fact]
        public void When_a_value_is_outside_an_unexpected_range_it_should_not_throw()
        {
            float value = 3.99F;

            value.Should().NotBeInRange(4, 5);
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_not_in_range_to_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().NotBeInRange(0, 1);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }

        [Fact]
        public void NaN_is_never_inside_any_range_of_floats()
        {
            float value = float.NaN;

            value.Should().NotBeInRange(4, 5);
        }

        [Theory]
        [InlineData(float.NaN, 1F)]
        [InlineData(1F, float.NaN)]
        public void Cannot_use_NaN_in_a_range_of_floats(float minimumValue, float maximumValue)
        {
            float value = 4.5F;

            Action act = () => value.Should().NotBeInRange(minimumValue, maximumValue);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_inside_any_range_of_doubles()
        {
            double value = double.NaN;

            value.Should().NotBeInRange(4, 5);
        }

        [Theory]
        [InlineData(double.NaN, 1D)]
        [InlineData(1D, double.NaN)]
        public void Cannot_use_NaN_in_a_range_of_doubles(double minimumValue, double maximumValue)
        {
            double value = 4.5D;

            Action act = () => value.Should().NotBeInRange(minimumValue, maximumValue);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }
    }
}
