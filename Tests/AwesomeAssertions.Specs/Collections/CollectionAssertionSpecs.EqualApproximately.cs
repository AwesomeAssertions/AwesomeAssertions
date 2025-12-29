#if NET8_0_OR_GREATER
using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The EqualApproximately specs.
/// </content>
public sealed partial class CollectionAssertionSpecs
{
    public sealed class EqualApproximately
    {
        [Fact]
        public void Two_null_collections_are_equal()
        {
            // Arrange
            float[] nullColl = null;

            // Act / Assert
            nullColl.Should().EqualApproximately(expectation: null, 0.1f);
        }

        [Fact]
        public void Subject_collection_with_more_items_than_expectation_is_not_equal()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Expected collection1 to approximate {1F, 2F} by ± 0.1F because we want to test the failure message, but {1F, 2F, 3F} contains 1 item(s) too many.");
        }

        [Fact]
        public void Subject_collection_with_less_items_than_expectation_is_not_equal()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2, 3, 4];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Expected collection1 to approximate {1F, 2F, 3F, 4F} by ± 0.1F because we want to test the failure message, but {1F, 2F, 3F} contains 1 item(s) less.");
        }

        [Fact]
        public void Subject_collection_cannot_be_null_if_expectation_is_not_null()
        {
            // Arrange
            float[] collection = null;
            float[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
                collection.Should().EqualApproximately(collection1, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Expected collection to approximate {1F, 2F, 3F} by ± 0.1F because we want to test the failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collections_to_be_equal_but_expected_collection_is_null_it_should_throw()
        {
            // Arrange
            float[] collection = [1, 2, 3];
            float[] collection1 = null;

            // Act
            Action act = () =>
                collection.Should().EqualApproximately(collection1, 0.1f, "because we want to test the failure {0}", "message");

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
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Expected collection1 to approximate {1F, 2F, 3F} by ± 0.1F because we want to test the failure message, but found empty collection.");
        }

        [Fact]
        public void When_a_non_empty_collection_is_compared_for_equality_to_an_empty_collection_it_should_throw()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Expected collection1 to approximate {empty} by ± 0.1F because we want to test the failure message, but found {1F, 2F, 3F}.");
        }

        [Fact]
        public void Float_collection_is_equal_approximately_to_another_collection_with_same_special_values()
        {
            // Arrange
            float[] collection1 = [-1, 0, 1, float.PositiveInfinity, float.NegativeInfinity, float.NaN, float.MinValue, float.MaxValue];
            float[] collection2 = [-1, 0, 1, float.PositiveInfinity, float.NegativeInfinity, float.NaN, float.MinValue, float.MaxValue];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.0f);
        }

        [Fact]
        public void Float_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            // Arrange
            float[] collection1 = [-1.001f, 0, 1.01f];
            float[] collection2 = [-1, 0, 1];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.01f);
        }

        [Fact]
        public void Float_collection_is_not_equal_approximately_to_another_collection_for_one_item_not_within_precision()
        {
            // Arrange
            float[] collection1 = [1, 2, 3.01f];
            float[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.001f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected * to approximate {1F, 2F, 3F} by ± 0.001F *failure message, but {1F, 2F, 3.01F} differs by 0.0099*F at index 2.");
        }

        [Fact]
        public void Double_collection_is_equal_approximately_to_another_collection_with_same_special_values()
        {
            // Arrange
            double[] collection1 = [-1, 0, 1, double.PositiveInfinity, double.NegativeInfinity, double.NaN, double.MinValue, double.MaxValue];
            double[] collection2 = [-1, 0, 1, double.PositiveInfinity, double.NegativeInfinity, double.NaN, double.MinValue, double.MaxValue];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.1);
        }

        [Fact]
        public void Double_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            // Arrange
            double[] collection1 = [-1.001, 0, 1.0099];
            double[] collection2 = [-1, 0, 1];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 0.01);
        }

        [Fact]
        public void Double_collection_is_not_equal_approximately_to_another_collection_for_one_item_not_within_precision()
        {
            // Arrange
            double[] collection1 = [1, 2, 3.01];
            double[] collection2 = [1, 2, 3];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 0.001, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected * to approximate {1.0, 2.0, 3.0} by ± 0.001 * failure message, but {1.0, 2.0, 3.01} differs by 0.0099* at index 2.");
        }

        [Fact]
        public void Integer_collection_is_equal_approximately_to_another_collection_with_same_special_values()
        {
            // Arrange
            int[] collection1 = [-1, 0, 1, int.MinValue, int.MaxValue];
            int[] collection2 = [-1, 0, 1, int.MinValue, int.MaxValue];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 1);
        }

        [Fact]
        public void Integer_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            // Arrange
            int[] collection1 = [-100, 0, 100];
            int[] collection2 = [-101, 0, 100];

            // Act / Assert
            collection1.Should().EqualApproximately(collection2, 1);
        }

        [Fact]
        public void Integer_collection_is_not_equal_approximately_to_another_collection_for_one_item_not_within_precision()
        {
            // Arrange
            int[] collection1 = [-100, 0, 100];
            int[] collection2 = [-102, 0, 100];

            // Act
            Action act = () => collection1.Should().EqualApproximately(collection2, 1, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected * to approximate {-102, 0, 100} by ± 1 because *failure message, but {-100, 0, 100} differs by 2 at index 0.");
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
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Expected collection1 to approximate {2147483647} by ± 1 because we want to test the failure message, but {-2147483648} differs at index 0.");
        }
    }

    public sealed class NotEqualApproximately
    {
        [Fact]
        public void Two_null_collections_cannot_be_compared()
        {
            // Arrange
            float[] nullColl = null;

            // Act
            Action act = () => nullColl.Should().NotEqualApproximately(unexpected: null, 0.1f);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("unexpected");
        }

        [Fact]
        public void Subject_collection_with_more_items_than_expectation_is_not_equal()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.1f);
        }

        [Fact]
        public void Subject_collection_with_less_items_than_expectation_is_not_equal()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [1, 2, 3, 4];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.1f);
        }

        [Fact]
        public void Subject_collection_cannot_be_null_if_expectation_is_not_null()
        {
            // Arrange
            float[] collection = null;
            float[] collection1 = [1, 2, 3];

            // Act
            Action act = () =>
                collection.Should().NotEqualApproximately(collection1, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>().Which.Message.Should().Be(
                "Did not expected collections to be approximately equal within ± 0.1F because we want to test the failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collections_not_to_be_equal_but_expected_collection_is_null_it_should_throw()
        {
            // Arrange
            float[] collection = [1, 2, 3];
            float[] collection1 = null;

            // Act
            Action act = () =>
                collection.Should().NotEqualApproximately(collection1, 0.1f, "because we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("unexpected");
        }

        [Fact]
        public void An_empty_collection_is_not_approximately_equal_to_a_non_empty_collection()
        {
            // Arrange
            float[] collection1 = [];
            float[] collection2 = [1, 2, 3];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.1f);
        }

        [Fact]
        public void An_non_empty_collection_is_not_approximately_equal_to_an_empty_collection()
        {
            // Arrange
            float[] collection1 = [1, 2, 3];
            float[] collection2 = [];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.1f);
        }

        [Theory]
        [InlineData(float.PositiveInfinity, float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity, float.NaN)]
        [InlineData(float.MaxValue, float.MinValue)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void Float_collection_is_equal_approximately_to_another_collection_with_same_special_values(float subject, float expected)
        {
            // Arrange
            float[] collection1 = [subject];
            float[] collection2 = [expected];

            // Act / Assert
            collection1.Should().NotEqualApproximately(collection2, 0.0f);
        }
    }
}
#endif
