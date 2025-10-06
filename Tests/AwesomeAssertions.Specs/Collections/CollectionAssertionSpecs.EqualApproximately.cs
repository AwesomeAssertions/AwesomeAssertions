#if NET8_0_OR_GREATER
using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The EqualApproximately specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class EqualApproximately
    {
        [Fact]
        public void When_both_collections_are_null_it_should_succeed()
        {
            // Arrange
            float[] nullColl = null;

            // Act / Assert
            nullColl.Should().EqualApproximately(expectation: null, 0.1f);
        }

        [Fact]
        public void When_two_collections_are_not_equal_because_the_actual_collection_contains_more_items_it_should_throw_using_the_reason()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {1F, 2F} by ± 0.1F because we want to test the failure message, but {1F, 2F, 3F} contains 1 item(s) too many.");
        }

        [Fact]
        public void
            When_two_collections_are_not_equal_because_the_actual_collection_contains_less_items_it_should_throw_using_the_reason()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2, 3, 4];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {1F, 2F, 3F, 4F} by ± 0.1F because we want to test the failure message, but {1F, 2F, 3F} contains 1 item(s) less.");
        }

        [Fact]
        public void When_asserting_collections_to_be_equal_but_subject_collection_is_null_it_should_throw()
        {
            // Arrange
            float[] collection = null;
            float[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
                collection.Should().EqualApproximately(collection1, 0.1f, "because we want to test the behaviour with a null subject");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection to approximate {1F, 2F, 3F} by ± 0.1F because we want to test the behaviour with a null subject, but found <null>.");
        }

        [Fact]
        public void When_asserting_collections_to_be_equal_but_expected_collection_is_null_it_should_throw()
        {
            // Arrange
            float[] collection = [1, 2, 3];
            float[] collection1 = null;

            // Act
            Action act = () =>
                collection.Should().EqualApproximately(collection1, 0.1f, "because we want to test the behaviour with a null subject");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("expectation");
        }

        [Fact]
        public void When_an_empty_collection_is_compared_for_equality_to_a_non_empty_collection_it_should_throw()
        {
            // Arrange
            float[] collection1 = [];
            float[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {1F, 2F, 3F} by ± 0.1F, but found empty collection.");
        }

        [Fact]
        public void When_a_non_empty_collection_is_compared_for_equality_to_an_empty_collection_it_should_throw()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {empty} by ± 0.1F, but found {1F, 2F, 3F}.");
        }

        [Fact]
        public void Should_succeed_when_asserting_identical_float_collection_is_equal_approximately_to_the_same_collection()
        {
            // Arrange
            float[] collection1 = [-1, 0, 1, float.PositiveInfinity, float.NegativeInfinity, float.NaN, float.MinValue, float.MaxValue];
            float[] collection2 = [-1, 0, 1, float.PositiveInfinity, float.NegativeInfinity, float.NaN, float.MinValue, float.MaxValue];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.1f);
        }

        [Fact]
        public void Should_succeed_when_asserting_float_collection_is_equal_approximately_to_the_collection_within_given_precision()
        {
            // Arrange
            float[] collection1 = [-1.001f, 0, 1.01f];
            float[] collection2 = [-1, 0, 1];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.01f);
        }

        [Fact]
        public void When_two_float_collections_are_not_equal_because_one_item_differs_it_should_throw_using_the_reason()
        {
            // Arrange
            float[] collection1 = [1, 2, 3.01f];
            float[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.001f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {1F, 2F, 3F} by ± 0.001F because we want to test the failure message, but {1F, 2F, 3.01F} differs at index 2.");
        }

        [Fact]
        public void Should_succeed_when_asserting_identical_double_collection_is_equal_approximately_to_the_same_collection()
        {
            // Arrange
            double[] collection1 = [-1, 0, 1, double.PositiveInfinity, double.NegativeInfinity, double.NaN, double.MinValue, double.MaxValue];
            double[] collection2 = [-1, 0, 1, double.PositiveInfinity, double.NegativeInfinity, double.NaN, double.MinValue, double.MaxValue];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.1);
        }

        [Fact]
        public void Should_succeed_when_asserting_double_collection_is_equal_approximately_to_the_collection_within_given_precision()
        {
            // Arrange
            double[] collection1 = [-1.001, 0, 1.0099];
            double[] collection2 = [-1, 0, 1];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.01);
        }

        [Fact]
        public void When_two_double_collections_are_not_equal_because_one_item_differs_it_should_throw_using_the_reason()
        {
            // Arrange
            double[] collection1 = [1, 2, 3.01];
            double[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.001, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {1.0, 2.0, 3.0} by ± 0.001 because we want to test the failure message, but {1.0, 2.0, 3.01} differs at index 2.");
        }

        [Fact]
        public void Should_succeed_when_asserting_identical_integer_collection_is_equal_approximately_to_the_same_collection()
        {
            // Arrange
            int[] collection1 = [-1, 0, 1, int.MinValue, int.MaxValue];
            int[] collection2 = [-1, 0, 1, int.MinValue, int.MaxValue];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 1);
        }

        [Fact]
        public void Should_succeed_when_asserting_integer_collection_is_equal_approximately_to_the_collection_within_given_precision()
        {
            // Arrange
            int[] collection1 = [-100, 0, 100];
            int[] collection2 = [-101, 0, 100];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 1);
        }

        [Fact]
        public void When_two_integer_collections_are_not_equal_because_one_item_differs_it_should_throw_using_the_reason()
        {
            // Arrange
            int[] collection1 = [-100, 0, 100];
            int[] collection2 = [-102, 0, 100];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 1, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection1 to approximate {-102, 0, 100} by ± 1 because we want to test the failure message, but {-100, 0, 100} differs at index 0.");
        }

        [Fact]
        public void When_two_integer_collections_are_not_equal_because_one_item_differs_by_more_than_maximum_possible_value_it_should_throw()
        {
            // Arrange
            int[] collection1 = [int.MinValue];
            int[] collection2 = [int.MaxValue];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 1, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                $"Expected collection1 to approximate {{{int.MaxValue}}} by ± 1 because we want to test the failure message, but {{{int.MinValue}}} differs at index 0.");
        }
    }

    public class NotEqualApproximately
    {
        [Fact]
        public void When_asserting_collections_not_to_equal_approximately_subject_but_collection_is_null_it_should_throw()
        {
            // Arrange
            float[] collection = null;
            float[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
                collection.Should().NotEqualApproximately(collection1, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collections not to be approximately equal within ± 0.1F because we want to test the failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collections_not_to_be_approximately_equal_but_expected_collection_is_null_it_should_throw()
        {
            // Arrange
            float[] collection = [1, 2, 3];
            float[] collection1 = null;

            // Act
            Action act =
                () => collection.Should().NotEqualApproximately(collection1, 0.1F, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("unexpected");
        }

        [Fact]
        public void When_asserting_collections_not_to_be_approximately_equal_but_both_collections_reference_the_same_object_it_should_throw()
        {
            float[] collection1 = [1, 2, 3];
            var collection2 = collection1;

            // Act
            Action act = () =>
                collection1.Should().NotEqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected collections not to be approximately equal within ± 0.1F because we want to test the failure message, but they both reference the same object.");
        }

        [Fact]
        public void When_asserting_two_collections_not_to_be_approximately_equal_because_the_actual_collection_contains_more_items_it_should_succeed()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.1F);
        }

        [Fact]
        public void When_asserting_two_collections_not_to_be_equal_because_the_actual_collection_contains_less_items_it_should_succeed()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2, 3, 4];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.1F);
        }

        [Fact]
        public void Should_succeed_when_asserting_float_collection_is_not_equal_approximately_to_a_collection_which_differs_in_one_item()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2, 3.11F];

            // Act / Assert
            collection1.Should()
                .NotEqualApproximately(collection2, 0.1f);
        }

        [Fact]
        public void When_two_approximately_equal_float_collections_are_not_expected_to_be_equal_it_should_throw()
        {
            // Arrange
            float[] collection1 = [1, 2.01f, 3];
            float[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().NotEqualApproximately(collection2, 0.011f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Did not expect collections {1F, 2F, 3F} and {1F, 2.01F, 3F} to be approximately equal within ± 0.011F because we want to test the failure message.");
        }

        [Fact]
        public void Should_succeed_when_asserting_double_collection_is_not_equal_approximately_to_a_collection_which_differs_in_one_item()
        {
            // Arrange
            double[] collection1 = [1, 2, 3];
            double[] collection2 = [1, 2, 3.11];

            // Act / Assert
            collection1.Should()
                .NotEqualApproximately(collection2, 0.1);
        }

        [Fact]
        public void When_two_approximately_equal_double_collections_are_not_expected_to_be_equal_it_should_throw()
        {
            // Arrange
            double[] collection1 = [1, 2.01, 3];
            double[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().NotEqualApproximately(collection2, 0.011, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Did not expect collections {1.0, 2.0, 3.0} and {1.0, 2.01, 3.0} to be approximately equal within ± 0.011 because we want to test the failure message.");
        }

        [Fact]
        public void Should_succeed_when_asserting_integer_collection_is_not_equal_approximately_to_a_collection_which_differs_in_one_item()
        {
            // Arrange
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [1, 2, 6];

            // Act / Assert
            collection1.Should()
                .NotEqualApproximately(collection2, 2);
        }

        [Fact]
        public void When_two_approximately_equal_integer_collections_are_not_expected_to_be_equal_it_should_throw()
        {
            // Arrange
            int[] collection1 = [1, 5, 3];
            int[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().NotEqualApproximately(collection2, 5, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Did not expect collections {1, 2, 3} and {1, 5, 3} to be approximately equal within ± 5 because we want to test the failure message.");
        }
    }
}
#endif
