using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class BeGreaterThan
    {
        [Fact]
        public void A_float_can_never_be_greater_than_NaN()
        {
            float? value = 3.4F;

            Action act = () => value.Should().BeGreaterThan(float.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_greater_than_another_float()
        {
            float? value = float.NaN;

            Action act = () => value.Should().BeGreaterThan(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_double_can_never_be_greater_than_NaN()
        {
            double? value = 3.4F;

            Action act = () => value.Should().BeGreaterThan(double.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_greater_than_another_double()
        {
            double? value = double.NaN;

            Action act = () => value.Should().BeGreaterThan(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(1, 10)]
        [InlineData(0, 5)]
        [InlineData(0, 0)]
        [InlineData(-1, 5)]
        [InlineData(-1, -1)]
        [InlineData(10, 10)]
        public void To_test_the_null_path_for_difference_on_nullable_int(int? subject, int expectation)
        {
            Action act = () => subject.Should().BeGreaterThan(expectation);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_null_path_for_difference_on_nullable_byte()
        {
            var value = (byte?)1;

            Action act = () => value.Should().BeGreaterThan(1);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_non_null_path_for_difference_on_nullable_byte()
        {
            var value = (byte?)1;

            Action act = () => value.Should().BeGreaterThan(2);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_null_path_for_difference_on_nullable_decimal()
        {
            var value = (decimal?)11.0;

            Action act = () => value.Should().BeGreaterThan(11M);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_null_path_for_difference_on_short()
        {
            var value = (short)11;

            Action act = () => value.Should().BeGreaterThan(11);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_null_path_for_difference_on_nullable_short()
        {
            var value = (short?)11;

            Action act = () => value.Should().BeGreaterThan(11);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_null_path_for_difference_on_nullable_ushort()
        {
            var value = (ushort?)11;

            Action act = () => value.Should().BeGreaterThan(11);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Theory]
        [InlineData(5L, 5L)]
        [InlineData(1L, 10L)]
        [InlineData(0L, 5L)]
        [InlineData(0L, 0L)]
        [InlineData(-1L, 5L)]
        [InlineData(-1L, -1L)]
        [InlineData(10L, 10L)]
        public void To_test_the_null_path_for_difference_on_nullable_long(long? subject, long expectation)
        {
            Action act = () => subject.Should().BeGreaterThan(expectation);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }
    }
}
