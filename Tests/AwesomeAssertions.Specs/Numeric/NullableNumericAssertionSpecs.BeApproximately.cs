using System;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class BeApproximately
    {
        [Fact]
        public void When_approximating_a_nullable_double_with_a_negative_precision_it_should_throw()
        {
            double? value = 3.1415927;

            Action act = () => value.Should().BeApproximately(3.14, -0.1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_approximating_two_nullable_doubles_with_a_negative_precision_it_should_throw()
        {
            double? value = 3.1415927;
            double? expected = 3.14;

            Action act = () => value.Should().BeApproximately(expected, -0.1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_nullable_double_is_indeed_approximating_a_value_it_should_not_throw()
        {
            double? value = 3.1415927;

            value.Should().BeApproximately(3.14, 0.1);
        }

        [Fact]
        public void When_nullable_double_is_indeed_approximating_a_nullable_value_it_should_not_throw()
        {
            double? value = 3.1415927;
            double? expected = 3.142;

            value.Should().BeApproximately(expected, 0.1);
        }

        [Fact]
        public void When_nullable_double_is_null_approximating_a_nullable_null_value_it_should_not_throw()
        {
            double? value = null;
            double? expected = null;

            value.Should().BeApproximately(expected, 0.1);
        }

        [Fact]
        public void When_nullable_double_with_value_is_not_approximating_a_non_null_nullable_value_it_should_throw()
        {
            double? value = 13;
            double? expected = 12;

            Action act = () => value.Should().BeApproximately(expected, 0.1);

            act.Should().Throw<XunitException>().WithMessage("Expected*12.0*0.1*13.0*");
        }

        [Fact]
        public void When_nullable_double_is_null_approximating_a_non_null_nullable_value_it_should_throw()
        {
            double? value = null;
            double? expected = 12;

            Action act = () => value.Should().BeApproximately(expected, 0.1);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate 12.0 +/- 0.1, but it was <null>.");
        }

        [Fact]
        public void When_nullable_double_is_not_null_approximating_a_null_value_it_should_throw()
        {
            double? value = 12;
            double? expected = null;

            Action act = () => value.Should().BeApproximately(expected, 0.1);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate <null> +/- 0.1, but it was 12.0.");
        }

        [Fact]
        public void When_nullable_double_has_no_value_it_should_throw()
        {
            double? value = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                value.Should().BeApproximately(3.14, 0.001);
            };

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate 3.14 +/- 0.001, but it was <null>.");
        }

        [Fact]
        public void When_nullable_double_is_not_approximating_a_value_it_should_throw()
        {
            double? value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(1.0, 0.1);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to approximate 1.0 +/- 0.1, but 3.14* differed by*");
        }

        [Fact]
        public void A_double_cannot_approximate_NaN()
        {
            double? value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(double.NaN, 0.1);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void When_approximating_a_nullable_float_with_a_negative_precision_it_should_throw()
        {
            float? value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(3.14F, -0.1F);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_approximating_two_nullable_floats_with_a_negative_precision_it_should_throw()
        {
            float? value = 3.1415927F;
            float? expected = 3.14F;

            Action act = () => value.Should().BeApproximately(expected, -0.1F);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_nullable_float_is_indeed_approximating_a_value_it_should_not_throw()
        {
            float? value = 3.1415927F;

            value.Should().BeApproximately(3.14F, 0.1F);
        }

        [Fact]
        public void When_nullable_float_is_indeed_approximating_a_nullable_value_it_should_not_throw()
        {
            float? value = 3.1415927f;
            float? expected = 3.142f;

            value.Should().BeApproximately(expected, 0.1f);
        }

        [Fact]
        public void When_nullable_float_is_null_approximating_a_nullable_null_value_it_should_not_throw()
        {
            float? value = null;
            float? expected = null;

            value.Should().BeApproximately(expected, 0.1f);
        }

        [Fact]
        public void When_nullable_float_with_value_is_not_approximating_a_non_null_nullable_value_it_should_throw()
        {
            float? value = 13;
            float? expected = 12;

            Action act = () => value.Should().BeApproximately(expected, 0.1f);

            act.Should().Throw<XunitException>().WithMessage("Expected*12*0.1*13*");
        }

        [Fact]
        public void When_nullable_float_is_null_approximating_a_non_null_nullable_value_it_should_throw()
        {
            float? value = null;
            float? expected = 12;

            Action act = () => value.Should().BeApproximately(expected, 0.1f);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate 12F +/- 0.1F, but it was <null>.");
        }

        [Fact]
        public void When_nullable_float_is_not_null_approximating_a_null_value_it_should_throw()
        {
            float? value = 12;
            float? expected = null;

            Action act = () => value.Should().BeApproximately(expected, 0.1f);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate <null> +/- 0.1F, but it was 12F.");
        }

        [Fact]
        public void When_nullable_float_has_no_value_it_should_throw()
        {
            float? value = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                value.Should().BeApproximately(3.14F, 0.001F);
            };

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate 3.14F +/- 0.001F, but it was <null>.");
        }

        [Fact]
        public void When_nullable_float_is_not_approximating_a_value_it_should_throw()
        {
            float? value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(1.0F, 0.1F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to approximate *1* +/- *0.1* but 3.14* differed by*");
        }

        [Fact]
        public void A_float_cannot_approximate_NaN()
        {
            float? value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(float.NaN, 0.1F);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void When_approximating_a_nullable_decimal_with_a_negative_precision_it_should_throw()
        {
            decimal? value = 3.1415927m;

            Action act = () => value.Should().BeApproximately(3.14m, -0.1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_approximating_two_nullable_decimals_with_a_negative_precision_it_should_throw()
        {
            decimal? value = 3.1415927m;
            decimal? expected = 3.14m;

            Action act = () => value.Should().BeApproximately(expected, -0.1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_nullable_decimal_is_indeed_approximating_a_value_it_should_not_throw()
        {
            decimal? value = 3.1415927m;

            value.Should().BeApproximately(3.14m, 0.1m);
        }

        [Fact]
        public void When_nullable_decimal_is_indeed_approximating_a_nullable_value_it_should_not_throw()
        {
            decimal? value = 3.1415927m;
            decimal? expected = 3.142m;

            value.Should().BeApproximately(expected, 0.1m);
        }

        [Fact]
        public void When_nullable_decimal_is_null_approximating_a_nullable_null_value_it_should_not_throw()
        {
            decimal? value = null;
            decimal? expected = null;

            value.Should().BeApproximately(expected, 0.1m);
        }

        [Fact]
        public void When_nullable_decimal_with_value_is_not_approximating_a_non_null_nullable_value_it_should_throw()
        {
            decimal? value = 13;
            decimal? expected = 12;

            Action act = () => value.Should().BeApproximately(expected, 0.1m);

            act.Should().Throw<XunitException>().WithMessage("Expected*12*0.1*13*");
        }

        [Fact]
        public void When_nullable_decimal_is_null_approximating_a_non_null_nullable_value_it_should_throw()
        {
            decimal? value = null;
            decimal? expected = 12;

            Action act = () => value.Should().BeApproximately(expected, 0.1m);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate 12M +/- 0.1M, but it was <null>.");
        }

        [Fact]
        public void When_nullable_decimal_is_not_null_approximating_a_null_value_it_should_throw()
        {
            decimal? value = 12;
            decimal? expected = null;

            Action act = () => value.Should().BeApproximately(expected, 0.1m);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate <null> +/- 0.1M, but it was 12M.");
        }

        [Fact]
        public void When_nullable_decimal_has_no_value_it_should_throw()
        {
            decimal? value = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                value.Should().BeApproximately(3.14m, 0.001m);
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected value to approximate*3.14* +/-*0.001*, but it was <null>.");
        }

        [Fact]
        public void When_nullable_decimal_is_not_approximating_a_value_it_should_throw()
        {
            decimal? value = 3.1415927m;

            Action act = () => value.Should().BeApproximately(1.0m, 0.1m);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to approximate*1.0* +/-*0.1*, but 3.14* differed by*");
        }
    }

    public class NotBeApproximately
    {
        [Fact]
        public void When_not_approximating_a_nullable_double_with_a_negative_precision_it_should_throw()
        {
            double? value = 3.1415927;

            Action act = () => value.Should().NotBeApproximately(3.14, -0.1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_not_approximating_two_nullable_doubles_with_a_negative_precision_it_should_throw()
        {
            double? value = 3.1415927;
            double? expected = 3.14;

            Action act = () => value.Should().NotBeApproximately(expected, -0.1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_double_is_not_approximating_a_value_it_should_not_throw()
        {
            double? value = 3.1415927;

            value.Should().NotBeApproximately(1.0, 0.1);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_double_has_no_value_it_should_throw()
        {
            double? value = null;

            value.Should().NotBeApproximately(3.14, 0.001);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_double_is_indeed_approximating_a_value_it_should_throw()
        {
            double? value = 3.1415927;

            Action act = () => value.Should().NotBeApproximately(3.14, 0.1);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to not approximate 3.14 +/- 0.1, but 3.14*only differed by*");
        }

        [Fact]
        public void
            When_asserting_not_approximately_and_nullable_double_is_not_approximating_a_nullable_value_it_should_not_throw()
        {
            double? value = 3.1415927;
            double? expected = 1.0;

            value.Should().NotBeApproximately(expected, 0.1);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_double_is_not_approximating_a_null_value_it_should_throw()
        {
            double? value = 3.1415927;
            double? expected = null;

            value.Should().NotBeApproximately(expected, 0.1);
        }

        [Fact]
        public void
            When_asserting_not_approximately_and_null_double_is_not_approximating_a_nullable_double_value_it_should_throw()
        {
            double? value = null;
            double? expected = 20.0;

            value.Should().NotBeApproximately(expected, 0.1);
        }

        [Fact]
        public void When_asserting_not_approximately_and_null_double_is_not_approximating_a_null_value_it_should_not_throw()
        {
            double? value = null;
            double? expected = null;

            Action act = () => value.Should().NotBeApproximately(expected, 0.1);

            act.Should().Throw<XunitException>()
                .WithMessage("Expected*null*0.1*but*null*");
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_double_is_approximating_a_nullable_value_it_should_throw()
        {
            double? value = 3.1415927;
            double? expected = 3.1;

            Action act = () => value.Should().NotBeApproximately(expected, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void A_double_cannot_approximate_NaN()
        {
            double? value = 3.1415927F;

            Action act = () => value.Should().NotBeApproximately(double.NaN, 0.1);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void When_not_approximating_a_nullable_float_with_a_negative_precision_it_should_throw()
        {
            float? value = 3.1415927F;

            Action act = () => value.Should().NotBeApproximately(3.14F, -0.1F);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_not_approximating_two_nullable_floats_with_a_negative_precision_it_should_throw()
        {
            float? value = 3.1415927F;
            float? expected = 3.14F;

            Action act = () => value.Should().NotBeApproximately(expected, -0.1F);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_float_is_not_approximating_a_value_it_should_not_throw()
        {
            float? value = 3.1415927F;

            value.Should().NotBeApproximately(1.0F, 0.1F);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_float_has_no_value_it_should_throw()
        {
            float? value = null;

            value.Should().NotBeApproximately(3.14F, 0.001F);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_float_is_indeed_approximating_a_value_it_should_throw()
        {
            float? value = 3.1415927F;

            Action act = () => value.Should().NotBeApproximately(3.14F, 0.1F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to not approximate *3.14F* +/- *0.1F* but 3.14* only differed by*");
        }

        [Fact]
        public void
            When_asserting_not_approximately_and_nullable_float_is_not_approximating_a_nullable_value_it_should_not_throw()
        {
            float? value = 3.1415927F;
            float? expected = 1.0F;

            value.Should().NotBeApproximately(expected, 0.1F);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_float_is_not_approximating_a_null_value_it_should_throw()
        {
            float? value = 3.1415927F;
            float? expected = null;

            value.Should().NotBeApproximately(expected, 0.1F);
        }

        [Fact]
        public void
            When_asserting_not_approximately_and_null_float_is_not_approximating_a_nullable_float_value_it_should_throw()
        {
            float? value = null;
            float? expected = 20.0f;

            value.Should().NotBeApproximately(expected, 0.1F);
        }

        [Fact]
        public void When_asserting_not_approximately_and_null_float_is_not_approximating_a_null_value_it_should_not_throw()
        {
            float? value = null;
            float? expected = null;

            Action act = () => value.Should().NotBeApproximately(expected, 0.1F);

            act.Should().Throw<XunitException>("Expected*<null>*+/-*0.1F*<null>*");
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_float_is_approximating_a_nullable_value_it_should_throw()
        {
            float? value = 3.1415927F;
            float? expected = 3.1F;

            Action act = () => value.Should().NotBeApproximately(expected, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void A_float_cannot_approximate_NaN()
        {
            float? value = 3.1415927F;

            Action act = () => value.Should().NotBeApproximately(float.NaN, 0.1F);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void When_not_approximating_a_nullable_decimal_with_a_negative_precision_it_should_throw()
        {
            decimal? value = 3.1415927m;

            Action act = () => value.Should().NotBeApproximately(3.14m, -0.1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_not_approximating_two_nullable_decimals_with_a_negative_precision_it_should_throw()
        {
            decimal? value = 3.1415927m;
            decimal? expected = 3.14m;

            Action act = () => value.Should().NotBeApproximately(expected, -0.1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_decimal_is_not_approximating_a_value_it_should_not_throw()
        {
            decimal? value = 3.1415927m;

            value.Should().NotBeApproximately(1.0m, 0.1m);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_decimal_has_no_value_it_should_throw()
        {
            decimal? value = null;

            value.Should().NotBeApproximately(3.14m, 0.001m);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_decimal_is_indeed_approximating_a_value_it_should_throw()
        {
            decimal? value = 3.1415927m;

            Action act = () => value.Should().NotBeApproximately(3.14m, 0.1m);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to not approximate*3.14* +/-*0.1*, but*3.14*only differed by*");
        }

        [Fact]
        public void
            When_asserting_not_approximately_and_nullable_decimal_is_not_approximating_a_nullable_value_it_should_not_throw()
        {
            decimal? value = 3.1415927m;
            decimal? expected = 1.0m;

            value.Should().NotBeApproximately(expected, 0.1m);
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_decimal_is_not_approximating_a_null_value_it_should_throw()
        {
            decimal? value = 3.1415927m;
            decimal? expected = null;

            value.Should().NotBeApproximately(expected, 0.1m);
        }

        [Fact]
        public void
            When_asserting_not_approximately_and_null_decimal_is_not_approximating_a_nullable_decimal_value_it_should_throw()
        {
            decimal? value = null;
            decimal? expected = 20.0m;

            value.Should().NotBeApproximately(expected, 0.1m);
        }

        [Fact]
        public void When_asserting_not_approximately_and_null_decimal_is_not_approximating_a_null_value_it_should_not_throw()
        {
            decimal? value = null;
            decimal? expected = null;

            Action act = () => value.Should().NotBeApproximately(expected, 0.1m);

            act.Should().Throw<XunitException>()
                .WithMessage("Expected*<null>*0.1M*<null>*");
        }

        [Fact]
        public void When_asserting_not_approximately_and_nullable_decimal_is_approximating_a_nullable_value_it_should_throw()
        {
            decimal? value = 3.1415927m;
            decimal? expected = 3.1m;

            Action act = () => value.Should().NotBeApproximately(expected, 0.1m);

            act.Should().Throw<XunitException>();
        }
    }
}
