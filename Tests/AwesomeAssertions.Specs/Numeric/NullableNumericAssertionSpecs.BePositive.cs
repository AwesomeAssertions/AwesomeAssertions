using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class BePositive
    {
        [Fact]
        public void NaN_is_never_a_positive_float()
        {
            float? value = float.NaN;

            Action act = () => value.Should().BePositive();

            act.Should().Throw<XunitException>().WithMessage("*but found NaN*");
        }

        [Fact]
        public void NaN_is_never_a_positive_double()
        {
            double? value = double.NaN;

            Action act = () => value.Should().BePositive();

            act.Should().Throw<XunitException>().WithMessage("*but found NaN*");
        }
    }
}
