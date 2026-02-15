using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class GenericCollectionAssertionOfStringSpecs
{
    public class BeEmpty
    {
        [Fact]
        public void Should_fail_when_asserting_collection_with_items_is_empty()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "three"];

            // Act
            Action act = () => collection.Should().BeEmpty();

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_succeed_when_asserting_collection_without_items_is_empty()
        {
            // Arrange
            IEnumerable<string> collection = new string[0];

            // Act / Assert
            collection.Should().BeEmpty();
        }

        [Fact]
        public void When_asserting_collection_to_be_empty_but_collection_is_null_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = null;

            // Act
            Action act = () => collection.Should().BeEmpty("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to be empty because*failure message, but found <null>.");
        }

        [Fact]
        public void When_the_collection_is_not_empty_unexpectedly_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "three"];

            // Act
            Action act = () => collection.Should().BeEmpty("we want to test the {0} message", "failure");

            // Assert
            act
                .Should().Throw<XunitException>()
                .WithMessage(
                    "Expected collection to be empty because*failure message, but found at least one item*one*");
        }
    }

    public class NotBeEmpty
    {
        [Fact]
        public void When_asserting_collection_to_be_not_empty_but_collection_is_null_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = null;

            // Act
            Action act = () => collection.Should().NotBeEmpty("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection not to be empty because*failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collection_with_items_is_not_empty_it_should_succeed()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "three"];

            // Act / Assert
            collection.Should().NotBeEmpty();
        }

        [Fact]
        public void When_asserting_collection_without_items_is_not_empty_it_should_fail()
        {
            // Arrange
            IEnumerable<string> collection = new string[0];

            // Act
            Action act = () => collection.Should().NotBeEmpty();

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_asserting_collection_without_items_is_not_empty_it_should_fail_with_descriptive_message_()
        {
            // Arrange
            IEnumerable<string> collection = new string[0];

            // Act
            Action act = () => collection.Should().NotBeEmpty("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection not to be empty because*failure message.");
        }
    }
}
