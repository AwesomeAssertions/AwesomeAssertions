using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeNaN
    {
        [Fact]
        public void NaN_is_equal_to_NaN_when_its_a_float()
        {
            float actual = float.NaN;

            actual.Should().BeNaN();
        }

        [InlineData(-1f)]
        [InlineData(0f)]
        [InlineData(1f)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        [InlineData(float.Epsilon)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity)]
        [Theory]
        public void Should_fail_when_asserting_normal_float_value_to_be_NaN(float actual)
        {
            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_normal_float_value_to_be_NaN()
        {
            float actual = 1;

            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Expected actual to be NaN, but found 1F.");
        }

        [Fact]
        public void Should_chain_when_asserting_NaN_as_float()
        {
            float actual = float.NaN;

            actual.Should().BeNaN()
                .And.Be(actual);
        }

        [Fact]
        public void NaN_is_equal_to_NaN_when_its_a_double()
        {
            double actual = double.NaN;

            actual.Should().BeNaN();
        }

        [InlineData(-1d)]
        [InlineData(0d)]
        [InlineData(1d)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(double.Epsilon)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        [Theory]
        public void Should_fail_when_asserting_normal_double_value_to_be_NaN(double actual)
        {
            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_normal_double_value_to_be_NaN()
        {
            double actual = 1;

            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Expected actual to be NaN, but found 1.0.");
        }

        [Fact]
        public void Should_chain_when_asserting_NaN_as_double()
        {
            double actual = double.NaN;

            actual.Should().BeNaN()
                .And.Be(actual);
        }

#if NET
        [Fact]
        public void NaN_is_equal_to_NaN_when_its_a_floating_point_number()
        {
            Half actual = Half.NaN;

            actual.Should().BeNaN();
        }

        public static TheoryData<Half> NormalFloatingPointNumbers => new()
        {
            { -Half.One },
            { Half.Zero },
            { Half.One },
            { Half.MinValue },
            { Half.MaxValue },
            { Half.Epsilon },
            { Half.NegativeInfinity },
            { Half.PositiveInfinity }
        };

        [Theory]
        [MemberData(nameof(NormalFloatingPointNumbers))]
        public void Should_fail_when_asserting_normal_floating_point_number_to_be_NaN(Half actual)
        {
            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_normal_floating_point_number_to_be_NaN()
        {
            Half actual = Half.One;

            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Expected actual to be NaN, but found 1.");
        }

        [Fact]
        public void Should_chain_when_asserting_NaN_as_floating_point_number()
        {
            Half actual = Half.NaN;

            actual.Should().BeNaN()
                .And.Be(actual);
        }
#endif

        [Fact]
        public void NaN_is_equal_to_NaN_when_its_a_nullable_float()
        {
            float? actual = float.NaN;

            actual.Should().BeNaN();
        }

        [InlineData(null)]
        [InlineData(-1f)]
        [InlineData(0f)]
        [InlineData(1f)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        [InlineData(float.Epsilon)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity)]
        [Theory]
        public void Should_fail_when_asserting_nullable_normal_float_value_to_be_NaN(float? actual)
        {
            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_nullable_normal_float_value_to_be_NaN()
        {
            float? actual = 1;

            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Expected actual to be NaN, but found 1F.");
        }

        [Fact]
        public void Should_chain_when_asserting_NaN_as_nullable_float()
        {
            float? actual = float.NaN;

            actual.Should().BeNaN()
                .And.Be(actual);
        }

        [Fact]
        public void NaN_is_equal_to_NaN_when_its_a_nullable_double()
        {
            double? actual = double.NaN;

            actual.Should().BeNaN();
        }

        [InlineData(null)]
        [InlineData(-1d)]
        [InlineData(0d)]
        [InlineData(1d)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(double.Epsilon)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        [Theory]
        public void Should_fail_when_asserting_nullable_normal_double_value_to_be_NaN(double? actual)
        {
            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_nullable_normal_double_value_to_be_NaN()
        {
            double? actual = 1;

            Action act = () => actual.Should().BeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Expected actual to be NaN, but found 1.0.");
        }

        [Fact]
        public void Should_chain_when_asserting_NaN_as_nullable_double()
        {
            double? actual = double.NaN;

            actual.Should().BeNaN()
                .And.Be(actual);
        }
    }

    public class NotBeNaN
    {
        [InlineData(-1f)]
        [InlineData(0f)]
        [InlineData(1f)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        [InlineData(float.Epsilon)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity)]
        [Theory]
        public void Normal_float_is_never_equal_to_NaN(float actual)
        {
            actual.Should().NotBeNaN();
        }

        [Fact]
        public void Should_fail_when_asserting_NaN_as_float()
        {
            float actual = float.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_NaN_as_float()
        {
            float actual = float.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Did not expect actual to be NaN.");
        }

        [Fact]
        public void Should_chain_when_asserting_normal_float_value()
        {
            float actual = 1;

            actual.Should().NotBeNaN()
                .And.Be(actual);
        }

        [InlineData(-1d)]
        [InlineData(0d)]
        [InlineData(1d)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(double.Epsilon)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        [Theory]
        public void Normal_double_is_never_equal_to_NaN(double actual)
        {
            actual.Should().NotBeNaN();
        }

        [Fact]
        public void Should_fail_when_asserting_NaN_as_double()
        {
            double actual = double.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_NaN_as_double()
        {
            double actual = double.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Did not expect actual to be NaN.");
        }

        [Fact]
        public void Should_chain_when_asserting_normal_double_value()
        {
            double actual = 1;

            actual.Should().NotBeNaN()
                .And.Be(actual);
        }

#if NET
        public static TheoryData<Half> NormalFloatingPointNumbers => new()
        {
            { -Half.One },
            { Half.Zero },
            { Half.One },
            { Half.MinValue },
            { Half.MaxValue },
            { Half.Epsilon },
            { Half.NegativeInfinity },
            { Half.PositiveInfinity }
        };

        [Theory]
        [MemberData(nameof(NormalFloatingPointNumbers))]
        public void Normal_floating_point_number_is_never_equal_to_NaN(Half actual)
        {
            actual.Should().NotBeNaN();
        }

        [Fact]
        public void Should_fail_when_asserting_NaN_as_floating_point_number()
        {
            Half actual = Half.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_NaN_as_floating_point_number()
        {
            Half actual = Half.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Did not expect actual to be NaN.");
        }

        [Fact]
        public void Should_chain_when_asserting_normal_floating_point_number_value()
        {
            Half actual = Half.One;

            actual.Should().NotBeNaN()
                .And.Be(actual);
        }
#endif

        [InlineData(null)]
        [InlineData(-1f)]
        [InlineData(0f)]
        [InlineData(1f)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        [InlineData(float.Epsilon)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity)]
        [Theory]
        public void Normal_nullable_float_is_never_equal_to_NaN(float? actual)
        {
            actual.Should().NotBeNaN();
        }

        [Fact]
        public void Should_fail_when_asserting_NaN_as_nullable_float()
        {
            float? actual = float.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_NaN_as_nullable_float()
        {
            float? actual = float.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Did not expect actual to be NaN.");
        }

        [Fact]
        public void Should_chain_when_asserting_normal_nullable_float_value()
        {
            float? actual = 1;

            actual.Should().NotBeNaN()
                .And.Be(actual);
        }

        [InlineData(null)]
        [InlineData(-1d)]
        [InlineData(0d)]
        [InlineData(1d)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(double.Epsilon)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        [Theory]
        public void Normal_nullable_double_is_never_equal_to_NaN(double? actual)
        {
            actual.Should().NotBeNaN();
        }

        [Fact]
        public void Should_fail_when_asserting_NaN_as_nullable_double()
        {
            double? actual = double.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_with_a_descriptive_message_when_asserting_NaN_as_nullable_double()
        {
            double? actual = double.NaN;

            Action act = () => actual.Should().NotBeNaN();

            act.Should().Throw<XunitException>()
               .WithMessage("Did not expect actual to be NaN.");
        }

        [Fact]
        public void Should_chain_when_asserting_normal_nullable_double_value()
        {
            double? actual = 1;

            actual.Should().NotBeNaN()
                .And.Be(actual);
        }
    }
}
