using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The HaveCountGreaterThan specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class HaveCountGreaterThan
    {
        [Fact]
        public void Should_succeed_when_asserting_collection_has_a_count_greater_than_less_the_number_of_items()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act / Assert
            collection.Should().HaveCountGreaterThan(2);
        }

        [Fact]
        public void Should_fail_when_asserting_collection_has_a_count_greater_than_the_number_of_items()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act
            Action act = () => collection.Should().HaveCountGreaterThan(3);

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_collection_has_a_count_greater_than_the_number_of_items_it_should_fail_with_descriptive_message_()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act
            Action action = () =>
                collection.Should().HaveCountGreaterThan(3, "we want to test the {0} message", "failure");

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected collection to contain more than 3 item(s) because*failure message, but found 3: {1, 2, 3}.");
        }

        [Fact]
        public void When_collection_count_is_greater_than_and_collection_is_null_it_should_throw()
        {
            // Arrange
            IEnumerable<string> collection = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().HaveCountGreaterThan(1, "we want to test the {0} message", "failure");
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*more than*1*because*failure message, but found <null>*");
        }
    }
}
