using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

/// <content>
/// The [Not]BeNullOrEmpty specs.
/// </content>
public partial class StringAssertionSpecs
{
    public class BeNullOrEmpty
    {
        [Fact]
        public void When_a_null_string_is_expected_to_be_null_or_empty_it_should_not_throw()
        {
            // Arrange
            string str = null;

            // Act / Assert
            str.Should().BeNullOrEmpty();
        }

        [Fact]
        public void When_an_empty_string_is_expected_to_be_null_or_empty_it_should_not_throw()
        {
            // Arrange
            string str = "";

            // Act / Assert
            str.Should().BeNullOrEmpty();
        }

        [Fact]
        public void When_a_valid_string_is_expected_to_be_null_or_empty_it_should_throw()
        {
            // Arrange
            string str = "hello";

            // Act
            Action act = () => str.Should().BeNullOrEmpty("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected str to be <null> or empty because we want to test the failure message, but found \"hello\".");
        }
    }

    public class NotBeNullOrEmpty
    {
        [Fact]
        public void When_a_valid_string_is_expected_to_be_not_null_or_empty_it_should_not_throw()
        {
            // Arrange
            string str = "Hello World";

            // Act / Assert
            str.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void When_an_empty_string_is_not_expected_to_be_null_or_empty_it_should_throw()
        {
            // Arrange
            string str = "";

            // Act
            Action act = () => str.Should().NotBeNullOrEmpty("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected str not to be <null> or empty because we want to test the failure message, but found \"\".");
        }

        [Fact]
        public void When_a_null_string_is_not_expected_to_be_null_or_empty_it_should_throw()
        {
            // Arrange
            string str = null;

            // Act
            Action act = () => str.Should().NotBeNullOrEmpty("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected str not to be <null> or empty because we want to test the failure message, but found <null>.");
        }
    }
}
