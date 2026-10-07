using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NumericAssertionSpecs
{
    public class BeGreaterThan
    {
        [Fact]
        public void When_a_value_is_greater_than_smaller_value_it_should_not_throw()
        {
            int value = 2;
            int smallerValue = 1;

            value.Should().BeGreaterThan(smallerValue);
        }

        [Fact]
        public void When_a_value_is_greater_than_greater_value_it_should_throw()
        {
            int value = 2;
            int greaterValue = 3;

            Action act = () => value.Should().BeGreaterThan(greaterValue);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_value_is_greater_than_same_value_it_should_throw()
        {
            int value = 2;
            int sameValue = 2;

            Action act = () => value.Should().BeGreaterThan(sameValue);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_a_value_is_greater_than_greater_value_it_should_throw_with_descriptive_message()
        {
            int value = 2;
            int greaterValue = 3;

            Action act = () =>
                value.Should().BeGreaterThan(greaterValue, "we want to test the {0} message", "failure");

            act
                .Should().Throw<XunitException>()
                .WithMessage("Expected value to be greater than 3 because we want to test the failure message, but found 2.");
        }

        [Fact]
        public void NaN_is_never_greater_than_another_float()
        {
            Action act = () => float.NaN.Should().BeGreaterThan(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_float_cannot_be_greater_than_NaN()
        {
            Action act = () => 3.4F.Should().BeGreaterThan(float.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void NaN_is_never_greater_than_another_double()
        {
            Action act = () => double.NaN.Should().BeGreaterThan(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void A_double_can_never_be_greater_than_NaN()
        {
            Action act = () => 3.4D.Should().BeGreaterThan(double.NaN);

            act
                .Should().Throw<ArgumentException>()
                .WithMessage("*NaN*");
        }

        [Fact]
        public void When_a_nullable_numeric_null_value_is_not_greater_than_it_should_throw()
        {
            int? value = null;

            Action act = () => value.Should().BeGreaterThan(0);

            act
                .Should().Throw<XunitException>()
                .WithMessage("*null*");
        }

        [Fact]
        public void To_test_the_null_path_for_difference_on_byte()
        {
            var value = (byte)1;

            Action act = () => value.Should().BeGreaterThan(1);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Fact]
        public void To_test_the_non_null_path_for_difference_on_byte()
        {
            var value = (byte)1;

            Action act = () => value.Should().BeGreaterThan(2);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(1, 10)]
        [InlineData(0, 5)]
        [InlineData(0, 0)]
        [InlineData(-1, 5)]
        [InlineData(-1, -1)]
        [InlineData(10, 10)]
        public void To_test_the_null_path_for_difference_on_int(int subject, int expectation)
        {
            Action act = () => subject.Should().BeGreaterThan(expectation);

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
        public void To_test_the_null_path_for_difference_on_long(long subject, long expectation)
        {
            Action act = () => subject.Should().BeGreaterThan(expectation);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 10)]
        [InlineData(10, 11)]
        public void To_test_the_null_path_for_difference_on_ushort(ushort subject, ushort expectation)
        {
            Action act = () => subject.Should().BeGreaterThan(expectation);

            act
                .Should().Throw<XunitException>()
                .Which.Message.Should().NotMatch("*(difference of 0)*");
        }
    }
}
