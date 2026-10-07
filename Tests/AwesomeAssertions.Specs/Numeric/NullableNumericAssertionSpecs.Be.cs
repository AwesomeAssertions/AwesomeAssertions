using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class Be
    {
        [Fact]
        public void Should_succeed_when_asserting_nullable_numeric_value_equals_an_equal_value()
        {
            int? nullableIntegerA = 1;
            int? nullableIntegerB = 1;

            nullableIntegerA.Should().Be(nullableIntegerB);
        }

        [Fact]
        public void Should_succeed_when_asserting_nullable_numeric_null_value_equals_null()
        {
            int? nullableIntegerA = null;
            int? nullableIntegerB = null;

            nullableIntegerA.Should().Be(nullableIntegerB);
        }

        [Fact]
        public void Should_fail_when_asserting_nullable_numeric_value_equals_a_different_value()
        {
            int? nullableIntegerA = 1;
            int? nullableIntegerB = 2;

            Action act = () => nullableIntegerA.Should().Be(nullableIntegerB);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_descriptive_message_when_asserting_nullable_numeric_value_equals_a_different_value()
        {
            int? nullableIntegerA = 1;
            int? nullableIntegerB = 2;

            Action act = () =>
                nullableIntegerA.Should().Be(nullableIntegerB, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected*2 because*failure message, but found 1.");
        }

        [Fact]
        public void Nan_is_never_equal_to_a_normal_float()
        {
            float? value = float.NaN;

            Action act = () => value.Should().Be(3.4F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be *3.4F, but found NaN*");
        }

        [Fact]
        public void NaN_can_be_compared_to_NaN_when_its_a_float()
        {
            float? value = float.NaN;

            value.Should().Be(float.NaN);
        }

        [Fact]
        public void Nan_is_never_equal_to_a_normal_double()
        {
            double? value = double.NaN;

            Action act = () => value.Should().Be(3.4D);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be *3.4, but found NaN*");
        }

        [Fact]
        public void NaN_can_be_compared_to_NaN_when_its_a_double()
        {
            double? value = double.NaN;

            value.Should().Be(double.NaN);
        }
    }
}
