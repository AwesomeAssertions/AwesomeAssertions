using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The StartWith specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class StartWith
    {
        [Fact]
        public void When_collection_does_not_start_with_a_specific_element_it_should_throw()
        {
            // Arrange
            string[] collection = ["john", "jane", "mike"];

            // Act
            Action act = () => collection.Should().StartWith("ryan", "of some reason");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected*start*ryan*because of some reason*but*john*");
        }

        [Fact]
        public void When_collection_does_not_start_with_a_null_sequence_it_should_throw()
        {
            // Arrange
            string[] collection = ["john"];

            // Act
            Action act = () => collection.Should().StartWith((IEnumerable<string>)null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("expectation");
        }

        [Fact]
        public void When_collection_does_not_start_with_a_null_sequence_using_a_comparer_it_should_throw()
        {
            // Arrange
            string[] collection = ["john"];

            // Act
            Action act = () => collection.Should().StartWith((IEnumerable<string>)null, (_, _) => true);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("expectation");
        }

        [Fact]
        public void When_collection_does_not_start_with_a_specific_element_in_a_sequence_it_should_throw()
        {
            // Arrange
            string[] collection = ["john", "bill", "jane", "mike"];

            // Act
            Action act = () => collection.Should().StartWith(["john", "ryan", "jane"], "of some reason");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected*start*ryan*because of some reason*but*differs at index 1*");
        }

        [Fact]
        public void
            When_collection_does_not_start_with_a_specific_element_in_a_sequence_using_custom_equality_comparison_it_should_throw()
        {
            // Arrange
            string[] collection = ["john", "bill", "jane", "mike"];

            // Act
            Action act = () => collection.Should().StartWith(["john", "ryan", "jane"],
                (s1, s2) => string.Equals(s1, s2, StringComparison.Ordinal), "of some reason");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected*start*ryan*because of some reason*but*differs at index 1*");
        }

        [Fact]
        public void When_collection_starts_with_the_specific_element_it_should_not_throw()
        {
            // Arrange
            string[] collection = ["john", "jane", "mike"];

            // Act / Assert
            collection.Should().StartWith("john");
        }

        [Fact]
        public void When_collection_starts_with_the_specific_sequence_of_elements_it_should_not_throw()
        {
            // Arrange
            string[] collection = ["john", "bill", "jane", "mike"];

            // Act / Assert
            collection.Should().StartWith(["john", "bill"]);
        }

        [Fact]
        public void
            When_collection_starts_with_the_specific_sequence_of_elements_using_custom_equality_comparison_it_should_not_throw()
        {
            // Arrange
            string[] collection = ["john", "bill", "jane", "mike"];

            // Act / Assert
            collection.Should().StartWith(["JoHn", "bIlL"],
                (s1, s2) => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void When_collection_starts_with_the_specific_null_element_it_should_not_throw()
        {
            // Arrange
            string[] collection = [null, "jane", "mike"];

            // Act / Assert
            collection.Should().StartWith((string)null);
        }

        [Fact]
        public void When_non_empty_collection_starts_with_the_empty_sequence_it_should_not_throw()
        {
            // Arrange
            string[] collection = ["jane", "mike"];

            // Act / Assert
            collection.Should().StartWith(new string[] { });
        }

        [Fact]
        public void When_empty_collection_starts_with_the_empty_sequence_it_should_not_throw()
        {
            // Arrange
            string[] collection = [];

            // Act / Assert
            collection.Should().StartWith(new string[] { });
        }

        [Fact]
        public void When_collection_starts_with_the_specific_sequence_with_null_elements_it_should_not_throw()
        {
            // Arrange
            string[] collection = [null, "john", null, "bill", "jane", "mike"];

            // Act / Assert
            collection.Should().StartWith([null, "john", null, "bill"]);
        }

        [Fact]
        public void
            When_collection_starts_with_the_specific_sequence_with_null_elements_using_custom_equality_comparison_it_should_not_throw()
        {
            // Arrange
            string[] collection = [null, "john", null, "bill", "jane", "mike"];

            // Act / Assert
            collection.Should().StartWith([null, "JoHn", null, "bIlL"],
                (s1, s2) => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void When_collection_starts_with_null_but_that_wasnt_expected_it_should_throw()
        {
            // Arrange
            string[] collection = [null, "jane", "mike"];

            // Act
            Action act = () => collection.Should().StartWith("john");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected*start*john*but*null*");
        }

        [Fact]
        public void When_null_collection_is_expected_to_start_with_an_element_it_should_throw()
        {
            // Arrange
            string[] collection = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().StartWith("john");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected*start*john*but*collection*null*");
        }
    }
}
