using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeApproximately
    {
        [Fact]
        public void When_approximating_a_float_with_a_negative_precision_it_should_throw()
        {
            float value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(3.14F, -0.1F);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_float_is_not_approximating_a_range_it_should_throw()
        {
            float value = 3.1415927F;

            Action act = () => value.Should().BeApproximately(3.14F, 0.001F, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to approximate *3.14* +/- *0.001* because*failure message, but *3.1415927* differed by *0.001592*");
        }

        [Fact]
        public void When_float_is_indeed_approximating_a_value_it_should_not_throw()
        {
            float value = 3.1415927F;

            value.Should().BeApproximately(3.14F, 0.1F);
        }

        [InlineData(9F)]
        [InlineData(11F)]
        [Theory]
        public void When_float_is_approximating_a_value_on_boundaries_it_should_not_throw(float value)
        {
            value.Should().BeApproximately(10F, 1F);
        }

        [InlineData(9F)]
        [InlineData(11F)]
        [Theory]
        public void When_float_is_not_approximating_a_value_on_boundaries_it_should_throw(float value)
        {
            Action act = () => value.Should().BeApproximately(10F, 0.9F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_approximating_a_float_towards_nan_it_should_not_throw()
        {
            float value = float.NaN;

            Action act = () => value.Should().BeApproximately(3.14F, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_approximating_positive_infinity_float_towards_positive_infinity_it_should_not_throw()
        {
            float value = float.PositiveInfinity;

            value.Should().BeApproximately(float.PositiveInfinity, 0.1F);
        }

        [Fact]
        public void When_approximating_negative_infinity_float_towards_negative_infinity_it_should_not_throw()
        {
            float value = float.NegativeInfinity;

            value.Should().BeApproximately(float.NegativeInfinity, 0.1F);
        }

        [Fact]
        public void When_float_is_not_approximating_positive_infinity_it_should_throw()
        {
            float value = float.PositiveInfinity;

            Action act = () => value.Should().BeApproximately(float.MaxValue, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_float_is_not_approximating_negative_infinity_it_should_throw()
        {
            float value = float.NegativeInfinity;

            Action act = () => value.Should().BeApproximately(float.MinValue, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void NaN_can_never_be_close_to_any_float()
        {
            float value = float.NaN;

            Action act = () => value.Should().BeApproximately(float.MinValue, 0.1F);

            act.Should().Throw<XunitException>().WithMessage("*NaN*");
        }

        [Fact]
        public void A_float_can_never_be_close_to_NaN()
        {
            float value = float.MinValue;

            Action act = () => value.Should().BeApproximately(float.NaN, 0.1F);

            act.Should().Throw<ArgumentException>().WithMessage("*NaN*");
        }

        [Fact]
        public void When_a_nullable_float_has_no_value_it_should_throw()
        {
            float? value = null;

            Action act = () => value.Should().BeApproximately(3.14F, 0.001F);

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to approximate*3.14* +/-*0.001*, but it was <null>.");
        }

        [Fact]
        public void When_approximating_a_double_with_a_negative_precision_it_should_throw()
        {
            double value = 3.1415927;

            Action act = () => value.Should().BeApproximately(3.14, -0.1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_double_is_not_approximating_a_range_it_should_throw()
        {
            double value = 3.1415927;

            Action act = () => value.Should().BeApproximately(3.14, 0.001, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to approximate 3.14 +/- 0.001 because*failure message, but 3.1415927 differed by 0.001592*");
        }

        [Fact]
        public void When_double_is_indeed_approximating_a_value_it_should_not_throw()
        {
            double value = 3.1415927;

            value.Should().BeApproximately(3.14, 0.1);
        }

        [Fact]
        public void When_approximating_a_double_towards_nan_it_should_not_throw()
        {
            double value = double.NaN;

            Action act = () => value.Should().BeApproximately(3.14F, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_approximating_positive_infinity_double_towards_positive_infinity_it_should_not_throw()
        {
            double value = double.PositiveInfinity;

            value.Should().BeApproximately(double.PositiveInfinity, 0.1);
        }

        [Fact]
        public void When_approximating_negative_infinity_double_towards_negative_infinity_it_should_not_throw()
        {
            double value = double.NegativeInfinity;

            value.Should().BeApproximately(double.NegativeInfinity, 0.1);
        }

        [Fact]
        public void When_double_is_not_approximating_positive_infinity_it_should_throw()
        {
            double value = double.PositiveInfinity;

            Action act = () => value.Should().BeApproximately(double.MaxValue, 0.1);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_double_is_not_approximating_negative_infinity_it_should_throw()
        {
            double value = double.NegativeInfinity;

            Action act = () => value.Should().BeApproximately(double.MinValue, 0.1);

            act.Should().Throw<XunitException>();
        }

        [InlineData(9D)]
        [InlineData(11D)]
        [Theory]
        public void When_double_is_approximating_a_value_on_boundaries_it_should_not_throw(double value)
        {
            value.Should().BeApproximately(10D, 1D);
        }

        [InlineData(9D)]
        [InlineData(11D)]
        [Theory]
        public void When_double_is_not_approximating_a_value_on_boundaries_it_should_throw(double value)
        {
            Action act = () => value.Should().BeApproximately(10D, 0.9D);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void NaN_can_never_be_close_to_any_double()
        {
            double value = double.NaN;

            Action act = () => value.Should().BeApproximately(double.MinValue, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void A_double_can_never_be_close_to_NaN()
        {
            double value = double.MinValue;

            Action act = () => value.Should().BeApproximately(double.NaN, 0.1F);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_approximating_a_decimal_with_a_negative_precision_it_should_throw()
        {
            decimal value = 3.1415927M;

            Action act = () => value.Should().BeApproximately(3.14m, -0.1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_decimal_is_not_approximating_a_range_it_should_throw()
        {
            decimal value = 3.5011m;

            Action act = () => value.Should().BeApproximately(3.5m, 0.001m, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected value to approximate*3.5* +/-*0.001* because*failure message, but *3.5011* differed by*0.0011*");
        }

        [Fact]
        public void When_decimal_is_indeed_approximating_a_value_it_should_not_throw()
        {
            decimal value = 3.5011m;

            value.Should().BeApproximately(3.5m, 0.01m);
        }

        [Fact]
        public void When_decimal_is_approximating_a_value_on_lower_boundary_it_should_not_throw()
        {
            decimal value = 9m;

            value.Should().BeApproximately(10m, 1m);
        }

        [Fact]
        public void When_decimal_is_approximating_a_value_on_upper_boundary_it_should_not_throw()
        {
            decimal value = 11m;

            value.Should().BeApproximately(10m, 1m);
        }

        [Fact]
        public void When_decimal_is_not_approximating_a_value_on_lower_boundary_it_should_throw()
        {
            decimal value = 9m;

            Action act = () => value.Should().BeApproximately(10m, 0.9m);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_decimal_is_not_approximating_a_value_on_upper_boundary_it_should_throw()
        {
            decimal value = 11m;

            Action act = () => value.Should().BeApproximately(10m, 0.9m);

            act.Should().Throw<XunitException>();
        }
    }

    public class NotBeApproximately
    {
        [Fact]
        public void When_not_approximating_a_float_with_a_negative_precision_it_should_throw()
        {
            float value = 3.1415927F;

            Action act = () => value.Should().NotBeApproximately(3.14F, -0.1F);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_float_is_approximating_a_range_and_should_not_approximate_it_should_throw()
        {
            float value = 3.1415927F;

            Action act = () => value.Should().NotBeApproximately(3.14F, 0.1F, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to not approximate *3.14* +/- *0.1* because*failure message, but *3.1415927* only differed by *0.001592*");
        }

        [Fact]
        public void When_float_is_not_approximating_a_value_and_should_not_approximate_it_should_not_throw()
        {
            float value = 3.1415927F;

            value.Should().NotBeApproximately(3.14F, 0.001F);
        }

        [Fact]
        public void When_approximating_a_float_towards_nan_and_should_not_approximate_it_should_throw()
        {
            float value = float.NaN;

            Action act = () => value.Should().NotBeApproximately(3.14F, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_not_approximating_a_float_towards_positive_infinity_and_should_not_approximate_it_should_not_throw()
        {
            float value = float.PositiveInfinity;

            value.Should().NotBeApproximately(float.MaxValue, 0.1F);
        }

        [Fact]
        public void When_not_approximating_a_float_towards_negative_infinity_and_should_not_approximate_it_should_not_throw()
        {
            float value = float.NegativeInfinity;

            value.Should().NotBeApproximately(float.MinValue, 0.1F);
        }

        [Fact]
        public void
            When_approximating_positive_infinity_float_towards_positive_infinity_and_should_not_approximate_it_should_throw()
        {
            float value = float.PositiveInfinity;

            Action act = () => value.Should().NotBeApproximately(float.PositiveInfinity, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void
            When_not_approximating_negative_infinity_float_towards_negative_infinity_and_should_not_approximate_it_should_throw()
        {
            float value = float.NegativeInfinity;

            Action act = () => value.Should().NotBeApproximately(float.NegativeInfinity, 0.1F);

            act.Should().Throw<XunitException>();
        }

        [InlineData(9F)]
        [InlineData(11F)]
        [Theory]
        public void When_float_is_not_approximating_a_value_on_boundaries_it_should_not_throw(float value)
        {
            value.Should().NotBeApproximately(10F, 0.9F);
        }

        [InlineData(9F)]
        [InlineData(11F)]
        [Theory]
        public void When_float_is_approximating_a_value_on_boundaries_it_should_throw(float value)
        {
            Action act = () => value.Should().NotBeApproximately(10F, 1F);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_nullable_float_has_no_value_and_should_not_approximate_it_should_not_throw()
        {
            float? value = null;

            value.Should().NotBeApproximately(3.14F, 0.001F);
        }

        [Fact]
        public void NaN_can_never_be_close_to_any_float()
        {
            float value = float.NaN;

            Action act = () => value.Should().NotBeApproximately(float.MinValue, 0.1F);

            act.Should().Throw<XunitException>().WithMessage("*NaN*");
        }

        [Fact]
        public void A_float_can_never_be_close_to_NaN()
        {
            float value = float.MinValue;

            Action act = () => value.Should().NotBeApproximately(float.NaN, 0.1F);

            act.Should().Throw<ArgumentException>().WithMessage("*NaN*");
        }

        [Fact]
        public void When_not_approximating_a_double_with_a_negative_precision_it_should_throw()
        {
            double value = 3.1415927;

            Action act = () => value.Should().NotBeApproximately(3.14, -0.1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_double_is_approximating_a_range_and_should_not_approximate_it_should_throw()
        {
            double value = 3.1415927;

            Action act = () => value.Should().NotBeApproximately(3.14, 0.1, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to not approximate *3.14* +/- *0.1* because*failure message, but *3.1415927* only differed by *0.001592*");
        }

        [Fact]
        public void When_double_is_not_approximating_a_value_and_should_not_approximate_it_should_not_throw()
        {
            double value = 3.1415927;

            value.Should().NotBeApproximately(3.14, 0.001);
        }

        [Fact]
        public void When_approximating_a_double_towards_nan_and_should_not_approximate_it_should_throw()
        {
            double value = double.NaN;

            Action act = () => value.Should().NotBeApproximately(3.14, 0.1);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_not_approximating_a_double_towards_positive_infinity_and_should_not_approximate_it_should_not_throw()
        {
            double value = double.PositiveInfinity;

            value.Should().NotBeApproximately(double.MaxValue, 0.1);
        }

        [Fact]
        public void When_not_approximating_a_double_towards_negative_infinity_and_should_not_approximate_it_should_not_throw()
        {
            double value = double.NegativeInfinity;

            value.Should().NotBeApproximately(double.MinValue, 0.1);
        }

        [Fact]
        public void
            When_approximating_positive_infinity_double_towards_positive_infinity_and_should_not_approximate_it_should_throw()
        {
            double value = double.PositiveInfinity;

            Action act = () => value.Should().NotBeApproximately(double.PositiveInfinity, 0.1);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void
            When_not_approximating_negative_infinity_double_towards_negative_infinity_and_should_not_approximate_it_should_throw()
        {
            double value = double.NegativeInfinity;

            Action act = () => value.Should().NotBeApproximately(double.NegativeInfinity, 0.1);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_nullable_double_has_no_value_and_should_not_approximate_it_should_throw()
        {
            double? value = null;

            value.Should().NotBeApproximately(3.14, 0.001);
        }

        [InlineData(9D)]
        [InlineData(11D)]
        [Theory]
        public void When_double_is_not_approximating_a_value_on_boundaries_it_should_not_throw(double value)
        {
            value.Should().NotBeApproximately(10D, 0.9D);
        }

        [InlineData(9D)]
        [InlineData(11D)]
        [Theory]
        public void When_double_is_approximating_a_value_on_boundaries_it_should_throw(double value)
        {
            Action act = () => value.Should().NotBeApproximately(10D, 1D);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void NaN_can_never_be_close_to_any_double()
        {
            double value = double.NaN;

            Action act = () => value.Should().NotBeApproximately(double.MinValue, 0.1F);

            act.Should().Throw<XunitException>().WithMessage("*NaN*");
        }

        [Fact]
        public void A_double_can_never_be_close_to_NaN()
        {
            double value = double.MinValue;

            Action act = () => value.Should().NotBeApproximately(double.NaN, 0.1F);

            act.Should().Throw<ArgumentException>().WithMessage("*NaN*");
        }

        [Fact]
        public void When_not_approximating_a_decimal_with_a_negative_precision_it_should_throw()
        {
            decimal value = 3.1415927m;

            Action act = () => value.Should().NotBeApproximately(3.14m, -0.1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("precision")
                .WithMessage("*must be non-negative*");
        }

        [Fact]
        public void When_decimal_is_approximating_a_range_and_should_not_approximate_it_should_throw()
        {
            decimal value = 3.5011m;

            Action act = () => value.Should().NotBeApproximately(3.5m, 0.1m, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to not approximate *3.5* +/- *0.1* because*failure message, but *3.5011* only differed by *0.0011*");
        }

        [Fact]
        public void When_decimal_is_not_approximating_a_value_and_should_not_approximate_it_should_not_throw()
        {
            decimal value = 3.5011m;

            value.Should().NotBeApproximately(3.5m, 0.001m);
        }

        [Fact]
        public void When_a_nullable_decimal_has_no_value_and_should_not_approximate_it_should_throw()
        {
            decimal? value = null;

            value.Should().NotBeApproximately(3.5m, 0.001m);
        }

        [Fact]
        public void When_decimal_is_not_approximating_a_value_on_lower_boundary_it_should_not_throw()
        {
            decimal value = 9m;

            value.Should().NotBeApproximately(10m, 0.9m);
        }

        [Fact]
        public void When_decimal_is_not_approximating_a_value_on_upper_boundary_it_should_not_throw()
        {
            decimal value = 11m;

            value.Should().NotBeApproximately(10m, 0.9m);
        }

        [Fact]
        public void When_decimal_is_approximating_a_value_on_lower_boundary_it_should_throw()
        {
            decimal value = 9m;

            Action act = () => value.Should().NotBeApproximately(10m, 1m);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_decimal_is_approximating_a_value_on_upper_boundary_it_should_throw()
        {
            decimal value = 11m;

            Action act = () => value.Should().NotBeApproximately(10m, 1m);

            act.Should().Throw<XunitException>();
        }
    }
}
