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
        public void A_guid_string_is_parsable_into_a_guid()
        {
            // Arrange
            string sut = "95e117d5-bb0d-4b16-a1c7-a11eea0284af";

            // Act / Assert
            sut.Should().BeParsableInto<Guid>();
        }

        [Fact]
        public void Chaining_after_a_successful_parse_works()
        {
            // Arrange
            string sut = "95e117d5-bb0d-4b16-a1c7-a11eea0284af";

            // Act / Assert
            sut.Should().BeParsableInto<Guid>()
                .Which.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void A_guid_string_is_not_parsable_into_an_int()
        {
            // Arrange
            string sut = "95e117d5-bb0d-4b16-a1c7-a11eea0284af";

            // Act
            Action act = () => sut.Should().BeParsableInto<int>("we want to test the {0}", "failure message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*95e11*int*we want*failure message*could not*");
        }

        [Fact]
        public void Chaining_will_be_ignore_when_parsing_fails()
        {
            // Arrange
            string sut = "95e117d5-bb0d-4b16-a1c7-a11eea0284af";

            // Act
            Action act = () => sut.Should()
                .BeParsableInto<int>("we want to test the {0}", "failure message")
                .Which.Should().Be(12);

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*95e11*int*we want*failure message*could not*");
        }

        [Theory]
        [InlineData("de-AT")]
        [InlineData("en-US")]
        [InlineData("zh-CN")]
        [InlineData("ru-RU")]
        public void A_guid_string_is_parsable_into_a_guid_in_various_cultures(string cultureName)
        {
            // Arrange
            string sut = "95e117d5-bb0d-4b16-a1c7-a11eea0284af";

            // Act / Assert
            sut.Should().BeParsableInto<Guid>(new CultureInfo(cultureName));
        }

        [Fact]
        public void A_guid_string_is_not_parsable_into_an_int_respecting_a_culture_specific_format()
        {
            // Arrange
            string sut = "95e117d5-bb0d-4b16-a1c7-a11eea0284af";

            // Act
            Action act = () =>
                sut.Should().BeParsableInto<int>(new CultureInfo("de-AT"), "we want to test the {0}", "failure message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*95e11*int*we want*failure message*could not*");
        }

        [Fact]
        public void Can_parse_a_floating_point_number_with_correct_culture_specific_separator()
        {
            // Arrange
            var sut = "12,34";

            // Act / Assert
            sut.Should().BeParsableInto<decimal>(new CultureInfo("de-AT"));
        }

        [Fact]
        public void Chaining_after_a_successful_parse_with_culture_specific_format_works()
        {
            // Arrange
            var sut = "12,34";

            // Act / Assert
            sut.Should().BeParsableInto<decimal>(new CultureInfo("de-AT"))
                .Which.Should().Be(12.34M);
        }

        // This test mails on macOS, because of how macOS handles decimal parsing.
        // It seems that macOS handles 12,34 and 12.34 equally even with a german culture.
        // Since we fully take what ever IParsable.Parse gives us, we have to exclude that one on macOS
#if WINDOWS || LINUX
        [Fact]
        public void Cannot_parse_a_floating_point_number_when_separator_does_not_match_the_culture_specific_separator()
        {
            // Arrange
            string sut = "12.34";

            // Act
            Action act = () => sut.Should().BeParsableInto<decimal>(new CultureInfo("de-AT"));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*12.34*decimal*could not*not in a correct format*");
        }
#endif
        [Fact]
        public void Cannot_parse_a_larger_integer_to_a_type_which_can_hold_less_places()
        {
            // Arrange
            string sut = "1234";

            // Act
            Action act = () => sut.Should().BeParsableInto<byte>();

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*1234*byte*too large or too small*byte*");
        }

        [Fact]
        public void Cannot_parse_a_larger_integer_to_a_type_which_can_hold_less_places_with_culture()
        {
            // Arrange
            string sut = "1234";

            // Act
            Action act = () => sut.Should().BeParsableInto<byte>(new CultureInfo("de-AT"));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*1234*byte*too large or too small*byte*");
        }

        [Fact]
        public void A_null_string_throws()
        {
            // Arrange
            string sut = null;

            // Act
            Action act = () => sut.Should().BeParsableInto<Guid>();

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void If_a_format_provider_is_passed_in_it_needs_to_be_non_null()
        {
            // Arrange
            string sut = "null";

            // Act
            Action act = () => sut.Should().BeParsableInto<Guid>((IFormatProvider)null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
#endif
