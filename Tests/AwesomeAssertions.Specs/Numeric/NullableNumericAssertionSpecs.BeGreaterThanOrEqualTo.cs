using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class BeGreaterThanOrEqualTo
    {
        [Fact]
        public void A_float_can_never_be_greater_than_or_equal_to_NaN()
        {
            float? value = 3.4F;

            Action act = () => value.Should().BeGreaterThanOrEqualTo(float.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_greater_than_or_equal_to_another_float()
        {
            float? value = float.NaN;

            Action act = () => value.Should().BeGreaterThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_double_can_never_be_greater_than_or_equal_to_NaN()
        {
            double? value = 3.4;

            Action act = () => value.Should().BeGreaterThanOrEqualTo(double.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_greater_than_or_equal_to_another_double()
        {
            double? value = double.NaN;

            Action act = () => value.Should().BeGreaterThanOrEqualTo(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }
    }
}
