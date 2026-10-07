using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeLessThanOrEqualTo
    {
        [Fact]
        public void When_a_value_is_less_than_or_equal_to_greater_value_it_should_not_throw()
        {
            int value = 1;
            int greaterValue = 2;

            value.Should().BeLessThanOrEqualTo(greaterValue);
        }

        [Fact]
        public void When_a_value_is_less_than_or_equal_to_same_value_it_should_not_throw()
        {
            int value = 2;
            int sameValue = 2;

            value.Should().BeLessThanOrEqualTo(sameValue);
        }

        [Fact]
        public void When_a_value_is_less_than_or_equal_to_smaller_value_it_should_throw()
        {
            int value = 2;
            int smallerValue = 1;

            Action act = () => value.Should().BeLessThanOrEqualTo(smallerValue);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_value_is_less_than_or_equal_to_smaller_value_it_should_throw_with_descriptive_message()
        {
            int value = 2;
            int smallerValue = 1;

            Action act = () =>
                value.Should().BeLessThanOrEqualTo(smallerValue, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to be less than or equal to 1 because we want to test the failure message, but found 2.");
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_less_than_or_equal_to_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().BeLessThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }

        [Fact]
        public void NaN_is_never_less_than_or_equal_to_another_float()
        {
            Action act = () => float.NaN.Should().BeLessThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_float_can_never_be_less_than_or_equal_to_NaN()
        {
            Action act = () => 3.4F.Should().BeLessThanOrEqualTo(float.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_less_than_or_equal_to_another_double()
        {
            Action act = () => double.NaN.Should().BeLessThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_double_can_never_be_less_than_or_equal_to_NaN()
        {
            Action act = () => 3.4D.Should().BeLessThanOrEqualTo(double.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void Chaining_after_one_assertion()
        {
            int value = 1;
            int greaterValue = 2;

            value.Should().BeLessThanOrEqualTo(greaterValue).And.Be(1);
        }
    }
}
