using System;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The HaveCountGreaterThanOrEqualTo specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class HaveCountGreaterThanOrEqualTo
    {
        [Fact]
        public void Should_succeed_when_asserting_collection_has_a_count_greater_than_or_equal_to_less_the_number_of_items()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act / Assert
            collection.Should().HaveCountGreaterThanOrEqualTo(3);
        }

        [Fact]
        public void Should_fail_when_asserting_collection_has_a_count_greater_than_or_equal_to_the_number_of_items()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act
            Action act = () => collection.Should().HaveCountGreaterThanOrEqualTo(4);

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void
            When_collection_has_a_count_greater_than_or_equal_to_the_number_of_items_it_should_fail_with_descriptive_message_()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act
            Action action = () =>
                collection.Should().HaveCountGreaterThanOrEqualTo(4, "we want to test the {0} message", "failure");

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected collection to contain at least 4 item(s) because*failure message, but found 3: {1, 2, 3}.");
        }

        [Fact]
        public void When_collection_count_is_greater_than_or_equal_to_and_collection_is_null_it_should_throw()
        {
            // Arrange
            int[] collection = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().HaveCountGreaterThanOrEqualTo(1, "we want to test the {0} message", "failure");
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*at least*1*because*failure message, but found <null>*");
        }

        [Fact]
        public void Chaining_after_one_assertion()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act / Assert
            collection.Should().HaveCountGreaterThanOrEqualTo(3).And.Contain(1);
        }
    }
}
