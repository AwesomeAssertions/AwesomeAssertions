using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class GenericCollectionAssertionOfStringSpecs
{
    public class OnlyHaveUniqueItems
    {
        [Fact]
        public void Should_succeed_when_asserting_collection_with_unique_items_contains_only_unique_items()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "three", "four"];

            // Act / Assert
            collection.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void When_a_collection_contains_duplicate_items_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "three", "three"];

            // Act
            Action act = () => collection.Should().OnlyHaveUniqueItems("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to only have unique items because*failure message, but item \"three\" is not unique.");
        }

        [Fact]
        public void When_a_collection_contains_multiple_duplicate_items_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "two", "three", "three"];

            // Act
            Action act = () => collection.Should().OnlyHaveUniqueItems("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to only have unique items because*failure message, but items {\"two\", \"three\"} are not unique.");
        }

        [Fact]
        public void When_asserting_collection_to_only_have_unique_items_but_collection_is_null_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = null;

            // Act
            Action act =
                () => collection.Should().OnlyHaveUniqueItems("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to only have unique items because*failure message, but found <null>.");
        }
    }
}
