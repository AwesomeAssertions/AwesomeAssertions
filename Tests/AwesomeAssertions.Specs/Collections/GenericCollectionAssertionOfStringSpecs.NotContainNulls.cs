using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class GenericCollectionAssertionOfStringSpecs
{
    public class NotContainNulls
    {
        [Fact]
        public void When_asserting_collection_to_not_contain_nulls_but_collection_is_null_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = null;

            // Act
            Action act = () => collection.Should().NotContainNulls("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection not to contain <null>s because*failure message, but collection is <null>.");
        }

        [Fact]
        public void When_collection_contains_multiple_nulls_that_are_unexpected_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = ["", null, "", null];

            // Act
            Action act = () => collection.Should().NotContainNulls("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection not to contain <null>s*because*failure message*{1, 3}*");
        }

        [Fact]
        public void When_collection_contains_nulls_that_are_unexpected_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = ["", null];

            // Act
            Action act = () => collection.Should().NotContainNulls("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection not to contain <null>s because*failure message, but found one at index 1.");
        }

        [Fact]
        public void When_collection_does_not_contain_nulls_it_should_not_throw()
        {
            // Arrange
            IEnumerable<string> collection = ["one", "two", "three"];

            // Act / Assert
            collection.Should().NotContainNulls();
        }
    }
}
