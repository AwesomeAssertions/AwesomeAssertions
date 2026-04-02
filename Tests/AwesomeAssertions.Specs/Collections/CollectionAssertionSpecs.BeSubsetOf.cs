using System;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The [Not]BeSubsetOf specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class BeSubsetOf
    {
        [Fact]
        public void When_collection_is_subset_of_a_specified_collection_it_should_not_throw()
        {
            // Arrange
            int[] subset = [1, 2];
            int[] superset = [1, 2, 3];

            // Act / Assert
            subset.Should().BeSubsetOf(superset);
        }

        [Fact]
        public void When_collection_is_not_a_subset_of_another_it_should_throw_with_the_reason()
        {
            // Arrange
            int[] subset = [1, 2, 3, 6];
            int[] superset = [1, 2, 4, 5];

            // Act
            Action act = () => subset.Should().BeSubsetOf(superset, "we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected subset to be a subset of {1, 2, 4, 5} because*failure message, " +
                "but items {3, 6} are not part of the superset.");
        }

        [Fact]
        public void When_an_empty_collection_is_tested_against_a_superset_it_should_succeed()
        {
            // Arrange
            int[] subset = [];
            int[] superset = [1, 2, 4, 5];

            // Act / Assert
            subset.Should().BeSubsetOf(superset);
        }

        [Fact]
        public void When_a_subset_is_tested_against_a_null_superset_it_should_throw_with_a_clear_explanation()
        {
            // Arrange
            int[] subset = [1, 2, 3];
            int[] superset = null;

            // Act
            Action act = () => subset.Should().BeSubsetOf(superset);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot verify a subset against a <null> collection.*");
        }

        [Fact]
        public void When_a_set_is_expected_to_be_not_a_subset_it_should_succeed()
        {
            // Arrange
            int[] subject = [1, 2, 4];
            int[] otherSet = [1, 2, 3];

            // Act / Assert
            subject.Should().NotBeSubsetOf(otherSet);
        }
    }

    public class NotBeSubsetOf
    {
        [Fact]
        public void When_an_empty_set_is_not_supposed_to_be_a_subset_of_another_set_it_should_throw()
        {
            // Arrange
            int[] subject = [];
            int[] otherSet = [1, 2, 3];

            // Act
            Action act = () => subject.Should().NotBeSubsetOf(otherSet);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect subject {empty} to be a subset of {1, 2, 3}.");
        }

        [Fact]
        public void Should_fail_when_asserting_collection_is_not_subset_of_a_superset_collection()
        {
            // Arrange
            int[] subject = [1, 2];
            int[] otherSet = [1, 2, 3];

            // Act
            Action act = () => subject.Should().NotBeSubsetOf(otherSet, "we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Did not expect subject {1, 2} to be a subset of {1, 2, 3} because*failure message.");
        }

        [Fact]
        public void When_asserting_collection_to_be_subset_against_null_collection_it_should_throw()
        {
            // Arrange
            int[] collection = null;
            int[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().BeSubsetOf(collection1, "we want to test the {0} message", "failure");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to be a subset of {1, 2, 3} because*failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collection_to_not_be_subset_against_same_collection_it_should_throw()
        {
            // Arrange
            int[] collection = [1, 2, 3];
            var collection1 = collection;

            // Act
            Action act = () => collection.Should().NotBeSubsetOf(collection1, "we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Did not expect*to be a subset of*because*failure message, but they both reference the same object.");
        }

        [Fact]
        public void When_asserting_collection_to_not_be_subset_against_null_collection_it_should_throw()
        {
            // Arrange
            int[] collection = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().NotBeSubsetOf([1, 2, 3], "we want to test the {0} message", "failure");
            };

            // TODO should the message contain the because text?
            // Assert
            act.Should().Throw<XunitException>().WithMessage("Cannot assert a <null> collection against a subset.");
        }
    }
}
