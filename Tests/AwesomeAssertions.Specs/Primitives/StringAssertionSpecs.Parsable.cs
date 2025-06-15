#if NET8_0_OR_GREATER
using System;
using System.Globalization;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

public partial class StringAssertionSpecs
{
    public class BeParsableInto
    {
        [Fact]
        public void Anything_which_is_no_int_is_not_parsable_into_an_int()
        {
            // Arrange
            string sut = "aaa";

            // Act
            Action act = () => sut.Should().BeParsableInto<int>("we want to test the {0}", "failure message")
                .Which.Should().Be(0);

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*aaa*int*we want*failure message*could not*");
        }

        [Theory]
        [InlineData("de-AT", "12,34")]
        [InlineData("en-US", "12.34")]
        public void A_floating_point_string_is_parsable_into_a_floating_point_value_in_various_cultures(
            string cultureName,
            string subject)
        {
            // Arrange

            // Act / Assert
            subject.Should().BeParsableInto<decimal>(new CultureInfo(cultureName))
                .Which.Should().Be(12.34M);
        }

        [Fact]
        public void A_string_is_not_parsable_into_an_int_even_with_a_culture_defined()
        {
            // Arrange
            string sut = "aaa";

            // Act
            Action act = () =>
                sut.Should().BeParsableInto<int>(new CultureInfo("de-AT"));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*aaa*int*the string*was not in the correct format*");
        }

        [Theory]
        [InlineData("de-AT", "12.34")]
        [InlineData("en-US", "12,34")]
        public void A_floating_point_string_is_not_parsable_into_a_floating_point_value_in_various_cultures(
            string cultureName,
            string subject)
        {
            // Arrange

            // Act / Assert
            Action act = () => subject.Should().BeParsableInto<decimal>(new CultureInfo(cultureName))
                .Which.Should().Be(12.34M);

            act.Should().Throw<XunitException>();
        }
    }

    public class NotBeParsableInto
    {
        [Fact]
        public void An_invalid_guid_string_is_not_parsable_into_a_guid()
        {
            // Arrange
            string sut = "95e117d5bb0d4b16a1c7a11eea0284a";

            // Act / Assert
            sut.Should().NotBeParsableInto<Guid>();
        }

        [Fact]
        public void An_integer_string_is_parsable_into_an_int_but_throws_when_not_expected_to()
        {
            // Arrange
            string sut = "1";

            // Act
            Action act = () => sut.Should().NotBeParsableInto<int>("we want to test the {0}", "failure message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*1*int*we want*failure message*could*");
        }

        [Fact]
        public void A_string_is_not_parsable_into_an_int_even_with_culture_info_defined()
        {
            // Arrange
            string sut = "aaa";

            // Act / Assert
            sut.Should().NotBeParsableInto<int>(new CultureInfo("de-DE"));
        }

        [Fact]
        public void An_integer_string_is_parsable_into_an_int_but_throws_when_not_expected_to_with_culture_defined()
        {
            // Arrange
            string sut = "1";

            // Act
            Action act = () => sut.Should().NotBeParsableInto<int>(new CultureInfo("de-DE"));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*1*int*but it could*");
        }
    }
}
#endif
