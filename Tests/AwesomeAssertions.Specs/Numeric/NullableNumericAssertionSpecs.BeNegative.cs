using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class BeNegative
    {
        [Fact]
        public void NaN_is_never_a_negative_float()
        {
            float? value = float.NaN;

            Action act = () => value.Should().BeNegative();

            act.Should().Throw<XunitException>().WithMessage("*but found NaN*");
        }

        [Fact]
        public void NaN_is_never_a_negative_double()
        {
            double? value = double.NaN;

            Action act = () => value.Should().BeNegative();

            act.Should().Throw<XunitException>().WithMessage("*but found NaN*");
        }
    }
}
