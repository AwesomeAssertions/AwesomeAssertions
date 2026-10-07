using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeGreaterThanOrEqualTo
    {
        [Fact]
        public void When_a_value_is_greater_than_or_equal_to_smaller_value_it_should_not_throw()
        {
            int value = 2;
            int smallerValue = 1;

            value.Should().BeGreaterThanOrEqualTo(smallerValue);
        }

        [Fact]
        public void When_a_value_is_greater_than_or_equal_to_same_value_it_should_not_throw()
        {
            int value = 2;
            int sameValue = 2;

            value.Should().BeGreaterThanOrEqualTo(sameValue);
        }

        [Fact]
        public void When_a_value_is_greater_than_or_equal_to_greater_value_it_should_throw()
        {
            int value = 2;
            int greaterValue = 3;

            Action act = () => value.Should().BeGreaterThanOrEqualTo(greaterValue);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_value_is_greater_than_or_equal_to_greater_value_it_should_throw_with_descriptive_message()
        {
            int value = 2;
            int greaterValue = 3;

            Action act =
                () => value.Should()
                    .BeGreaterThanOrEqualTo(greaterValue, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to be greater than or equal to 3 because we want to test the failure message, but found 2.");
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_greater_than_or_equal_to_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().BeGreaterThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }

        [Fact]
        public void NaN_is_never_greater_than_or_equal_to_another_float()
        {
            Action act = () => float.NaN.Should().BeGreaterThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_float_cannot_be_greater_than_or_equal_to_NaN()
        {
            Action act = () => 3.4F.Should().BeGreaterThanOrEqualTo(float.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_greater_or_equal_to_another_double()
        {
            Action act = () => double.NaN.Should().BeGreaterThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_double_can_never_be_greater_or_equal_to_NaN()
        {
            Action act = () => 3.4D.Should().BeGreaterThanOrEqualTo(double.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void Chaining_after_one_assertion()
        {
            int value = 2;
            int smallerValue = 1;

            value.Should().BeGreaterThanOrEqualTo(smallerValue).And.Be(2);
        }
    }
}
