using System;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The [Not]HaveSameCount specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class HaveSameCount
    {
        [Fact]
        public void When_both_collections_have_the_same_number_elements_it_should_succeed()
        {
            // Arrange
            int[] firstCollection = [1, 2, 3];
            int[] secondCollection = [4, 5, 6];

            // Act / Assert
            firstCollection.Should().HaveSameCount(secondCollection);
        }

        [Fact]
        public void When_both_collections_do_not_have_the_same_number_of_elements_it_should_fail()
        {
            // Arrange
            int[] firstCollection = [1, 2, 3];
            int[] secondCollection = [4, 6];

            // Act
            Action act = () => firstCollection.Should().HaveSameCount(secondCollection);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected firstCollection to have 2 item(s), but found 3.");
        }

        [Fact]
        public void When_comparing_item_counts_and_a_reason_is_specified_it_should_it_in_the_exception()
        {
            // Arrange
            int[] firstCollection = [1, 2, 3];
            int[] secondCollection = [4, 6];

            // Act
            Action act = () => firstCollection.Should().HaveSameCount(secondCollection, "we want to test the {0}", "reason");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected firstCollection to have 2 item(s) because we want to test the reason, but found 3.");
        }

        [Fact]
        public void When_asserting_collections_to_have_same_count_against_null_collection_it_should_throw()
        {
            // Arrange
            int[] collection = null;
            int[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().HaveSameCount(collection1, "because we want to test the behaviour with a null subject");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to have the same count as {1, 2, 3} because we want to test the behaviour with a null subject, but found <null>.");
        }

        [Fact]
        public void When_asserting_collections_to_have_same_count_against_an_other_null_collection_it_should_throw()
        {
            // Arrange
            int[] collection = [1, 2, 3];
            int[] otherCollection = null;

            // Act
            Action act = () => collection.Should().HaveSameCount(otherCollection);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot verify count against a <null> collection.*");
        }
    }

    public class NotHaveSameCount
    {
        [Fact]
        public void When_asserting_not_same_count_and_collections_have_different_number_elements_it_should_succeed()
        {
            // Arrange
            int[] firstCollection = [1, 2, 3];
            int[] secondCollection = [4, 6];

            // Act / Assert
            firstCollection.Should().NotHaveSameCount(secondCollection);
        }

        [Fact]
        public void When_asserting_not_same_count_and_both_collections_have_the_same_number_elements_it_should_fail()
        {
            // Arrange
            int[] firstCollection = [1, 2, 3];
            int[] secondCollection = [4, 5, 6];

            // Act
            Action act = () => firstCollection.Should().NotHaveSameCount(secondCollection);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected firstCollection to not have 3 item(s), but found 3.");
        }

        [Fact]
        public void When_comparing_not_same_item_counts_and_a_reason_is_specified_it_should_it_in_the_exception()
        {
            // Arrange
            int[] firstCollection = [1, 2, 3];
            int[] secondCollection = [4, 5, 6];

            // Act
            Action act = () => firstCollection.Should().NotHaveSameCount(secondCollection, "we want to test the {0}", "reason");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected firstCollection to not have 3 item(s) because we want to test the reason, but found 3.");
        }

        [Fact]
        public void When_asserting_collections_to_not_have_same_count_against_null_collection_it_should_throw()
        {
            // Arrange
            int[] collection = null;
            int[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().NotHaveSameCount(collection1, "because we want to test the behaviour with a null subject");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to not have the same count as {1, 2, 3} because we want to test the behaviour with a null subject, but found <null>.");
        }

        [Fact]
        public void When_asserting_collections_to_not_have_same_count_against_an_other_null_collection_it_should_throw()
        {
            // Arrange
            int[] collection = [1, 2, 3];
            int[] otherCollection = null;

            // Act
            Action act = () => collection.Should().NotHaveSameCount(otherCollection);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot verify count against a <null> collection.*");
        }

        [Fact]
        public void
            When_asserting_collections_to_not_have_same_count_but_both_collections_references_the_same_object_it_should_throw()
        {
            // Arrange
            int[] collection = [1, 2, 3];
            var collection1 = collection;

            // Act
            Action act = () => collection.Should().NotHaveSameCount(collection1,
                "because we want to test the behaviour with same objects");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "*not have the same count*because we want to test the behaviour with same objects*but they both reference the same object.");
        }
    }
}
