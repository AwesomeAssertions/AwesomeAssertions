using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class Be
    {
        [Fact]
        public void A_value_is_equal_to_the_same_value()
        {
            int value = 1;
            int sameValue = 1;

            value.Should().Be(sameValue);
        }

        [Fact]
        public void A_value_is_not_equal_to_another_value()
        {
            int value = 1;
            int differentValue = 2;

            Action act = () => value.Should().Be(differentValue, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be 2 because we want to test the failure message, but found 1.");
        }

        [Fact]
        public void A_value_is_equal_to_the_same_nullable_value()
        {
            int value = 2;
            int? nullableValue = 2;

            value.Should().Be(nullableValue);
        }

        [Fact]
        public void A_value_is_not_equal_to_null()
        {
            int value = 2;
            int? nullableValue = null;

            Action act = () => value.Should().Be(nullableValue);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected*<null>, but found 2.");
        }

        [Fact]
        public void Null_is_not_equal_to_another_nullable_value()
        {
            int? value = 2;

            Action action = () => ((int?)null).Should().Be(value);

            action
                .Should().Throw<XunitException>()
                .WithMessage("Expected*2, but found <null>.");
        }

        [InlineData(0, 0)]
        [InlineData(null, null)]
        [Theory]
        public void A_nullable_value_is_equal_to_the_same_nullable_value(int? subject, int? expected)
        {
            subject.Should().Be(expected);
        }

        [InlineData(0, 1)]
        [InlineData(0, null)]
        [InlineData(null, 0)]
        [Theory]
        public void A_nullable_value_is_not_equal_to_another_nullable_value(int? subject, int? expected)
        {
            Action act = () => subject.Should().Be(expected);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Null_is_not_equal_to_another_value()
        {
            int? subject = null;
            int expected = 1;

            Action act = () => subject.Should().Be(expected);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_asserting_that_a_float_value_is_equal_to_a_different_value_it_should_throw()
        {
            float value = 3.5F;

            Action act = () => value.Should().Be(3.4F, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to be *3.4* because we want to test the failure message, but found *3.5*");
        }

        [Fact]
        public void When_asserting_that_a_float_value_is_equal_to_the_same_value_it_should_not_throw()
        {
            float value = 3.5F;

            value.Should().Be(3.5F);
        }

        [Fact]
        public void When_asserting_that_a_null_float_value_is_equal_to_some_value_it_should_throw()
        {
            float? value = null;

            Action act = () => value.Should().Be(3.5F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be *3.5* but found <null>.");
        }

        [Fact]
        public void When_asserting_that_a_double_value_is_equal_to_a_different_value_it_should_throw()
        {
            double value = 3.5;

            Action act = () => value.Should().Be(3.4, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to be 3.4 because we want to test the failure message, but found 3.5*.");
        }

        [Fact]
        public void When_asserting_that_a_double_value_is_equal_to_the_same_value_it_should_not_throw()
        {
            double value = 3.5;

            value.Should().Be(3.5);
        }

        [Fact]
        public void When_asserting_that_a_null_double_value_is_equal_to_some_value_it_should_throw()
        {
            double? value = null;

            Action act = () => value.Should().Be(3.5);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be 3.5, but found <null>.");
        }

        [Fact]
        public void When_asserting_that_a_decimal_value_is_equal_to_a_different_value_it_should_throw()
        {
            decimal value = 3.5m;

            Action act = () => value.Should().Be(3.4m, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to be*3.4* because we want to test the failure message, but found*3.5*");
        }

        [Fact]
        public void When_asserting_that_a_decimal_value_is_equal_to_the_same_value_it_should_not_throw()
        {
            decimal value = 3.5m;

            value.Should().Be(3.5m);
        }

        [Fact]
        public void When_asserting_that_a_null_decimal_value_is_equal_to_some_value_it_should_throw()
        {
            decimal? value = null;
            decimal someValue = 3.5m;

            Action act = () => value.Should().Be(someValue);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be*3.5*, but found <null>.");
        }

        [Fact]
        public void Nan_is_never_equal_to_a_normal_float()
        {
            float value = float.NaN;

            Action act = () => value.Should().Be(3.4F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be *3.4F, but found NaN*");
        }

        [Fact]
        public void NaN_can_be_compared_to_NaN_when_its_a_float()
        {
            float value = float.NaN;

            value.Should().Be(float.NaN);
        }

        [Fact]
        public void Nan_is_never_equal_to_a_normal_double()
        {
            double value = double.NaN;

            Action act = () => value.Should().Be(3.4D);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be *3.4, but found NaN*");
        }

        [Fact]
        public void NaN_can_be_compared_to_NaN_when_its_a_double()
        {
            double value = double.NaN;

            value.Should().Be(double.NaN);
        }
    }

    public class NotBe
    {
        [InlineData(1, 2)]
        [InlineData(null, 2)]
        [Theory]
        public void A_nullable_value_is_not_equal_to_another_value(int? subject, int unexpected)
        {
            subject.Should().NotBe(unexpected);
        }

        [Fact]
        public void A_value_is_not_different_from_the_same_value()
        {
            int value = 1;
            int sameValue = 1;

            Action act = () => value.Should().NotBe(sameValue, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage("Did not expect value to be 1 because we want to test the failure message.");
        }

        [InlineData(null, null)]
        [InlineData(0, 0)]
        [Theory]
        public void A_nullable_value_is_not_different_from_the_same_value(int? subject, int? unexpected)
        {
            Action act = () => subject.Should().NotBe(unexpected);

            act.Should().Throw<XunitException>();
        }

        [InlineData(0, 1)]
        [InlineData(0, null)]
        [InlineData(null, 0)]
        [Theory]
        public void A_nullable_value_is_different_from_another_value(int? subject, int? unexpected)
        {
            subject.Should().NotBe(unexpected);
        }
    }

    public class Bytes
    {
        [Fact]
        public void When_asserting_a_byte_value_it_should_treat_is_any_numeric_value()
        {
            byte value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_sbyte_value_it_should_treat_is_any_numeric_value()
        {
            sbyte value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_short_value_it_should_treat_is_any_numeric_value()
        {
            short value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_an_ushort_value_it_should_treat_is_any_numeric_value()
        {
            ushort value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_an_uint_value_it_should_treat_is_any_numeric_value()
        {
            uint value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_long_value_it_should_treat_is_any_numeric_value()
        {
            long value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_an_ulong_value_it_should_treat_is_any_numeric_value()
        {
            ulong value = 2;

            value.Should().Be(2);
        }
    }

    public class NullableBytes
    {
        [Fact]
        public void When_asserting_a_nullable_byte_value_it_should_treat_is_any_numeric_value()
        {
            byte? value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_nullable_sbyte_value_it_should_treat_is_any_numeric_value()
        {
            sbyte? value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_nullable_short_value_it_should_treat_is_any_numeric_value()
        {
            short? value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_nullable_ushort_value_it_should_treat_is_any_numeric_value()
        {
            ushort? value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_nullable_uint_value_it_should_treat_is_any_numeric_value()
        {
            uint? value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_nullable_long_value_it_should_treat_is_any_numeric_value()
        {
            long? value = 2;

            value.Should().Be(2);
        }

        [Fact]
        public void When_asserting_a_nullable_nullable_ulong_value_it_should_treat_is_any_numeric_value()
        {
            ulong? value = 2;

            value.Should().Be(2);
        }
    }
}
