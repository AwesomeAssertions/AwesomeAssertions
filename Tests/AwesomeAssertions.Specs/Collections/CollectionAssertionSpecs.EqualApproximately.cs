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
            float[] subject = null;
            float[] expected = null;

            subject.Should().EqualApproximately(expected, 0.1f);
        }

        [Fact]
        public void Subject_collection_with_more_items_than_expectation_is_not_equal()
        {
            float[] subject = [1, 2, 3];
            float[] expected = [1, 2];

            Action act = () => subject.Should().EqualApproximately(expected, 0.1f, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {1F, 2F} ±0.1F because *failure message, but {1F, 2F, 3F} contains 1 item(s) too many.");
        }

        [Fact]
        public void Subject_collection_with_less_items_than_expectation_is_not_equal()
        {
            float[] subject = [1, 2, 3];
            float[] expected = [1, 2, 3, 4];

            Action act = () => subject.Should().EqualApproximately(expected, 0.1f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {1F, 2F, 3F, 4F} ±0.1F because *failure message, but {1F, 2F, 3F} contains 1 item(s) less.");
        }

        [Fact]
        public void Subject_collection_cannot_be_null_if_expectation_is_not_null()
        {
            float[] subject = null;
            float[] expected = [1, 2, 3];

            Action act = () => subject.Should().EqualApproximately(expected, 0.1f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {1F, 2F, 3F} ±0.1F because *failure message, but found <null>.");
        }

        [Fact]
        public void Expectation_collection_cannot_be_null_if_subject_is_not_null()
        {
            float[] subject = [1, 2, 3];
            float[] expected = null;

            Action act = () => subject.Should().EqualApproximately(expected, 0.1f);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("expectation");
        }

        [Fact]
        public void An_empty_collection_is_not_approximately_equal_to_a_non_empty_collection()
        {
            float[] subject = [];
            float[] expected = [1, 2, 3];

            Action act = () => subject.Should().EqualApproximately(expected, 0.1f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {1F, 2F, 3F} ±0.1F because *failure message, but found empty collection.");
        }

        [Fact]
        public void A_non_empty_collection_is_not_approximately_equal_to_an_empty_collection()
        {
            float[] subject = [1, 2, 3];
            float[] expected = [];

            Action act = () => subject.Should().EqualApproximately(expected, 0.1f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {empty} ±0.1F because *failure message, but found {1F, 2F, 3F}.");
        }

        [Theory]
        [InlineData(float.PositiveInfinity)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.NaN)]
        [InlineData(float.MaxValue)]
        [InlineData(float.MinValue)]
        public void Float_collection_is_equal_approximately_to_another_collection_with_same_special_values(float specialValue)
        {
            float[] subject = [-1, 0, 1, specialValue];
            float[] expected = [-1, 0, 1, specialValue];

            subject.Should().EqualApproximately(expected, 0.0f);
        }

        [Fact]
        public void Float_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            float[] subject = [-1.001f, 0, 1.01f];
            float[] expected = [-1, 0, 1];

            subject.Should().EqualApproximately(expected, 0.01f);
        }

        [Fact]
        public void Float_collection_is_not_equal_approximately_to_another_collection_for_one_item_not_within_precision()
        {
            float[] subject = [1, 2, 3.01f];
            float[] expected = [1, 2, 3];

            Action act = () => subject.Should().EqualApproximately(expected, 0.001f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {1F, 2F, 3F} ±0.001F because *failure message, but {1F, 2F, 3.01F} differed by 0.0099*F at index 2.");
        }

        [Theory]
        [InlineData(float.PositiveInfinity)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.NaN)]
        [InlineData(float.MaxValue)]
        [InlineData(float.MinValue)]
        public void Double_collection_is_equal_approximately_to_another_collection_with_same_special_values(double specialValue)
        {
            double[] subject = [-1, 0, 1, specialValue];
            double[] expected = [-1, 0, 1, specialValue];

            subject.Should().EqualApproximately(expected, 0.1);
        }

        [Fact]
        public void Double_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            double[] subject = [-1.001, 0, 1.0099];
            double[] expected = [-1, 0, 1];

            subject.Should().EqualApproximately(expected, 0.01);
        }

        [Fact]
        public void Double_collection_is_not_equal_approximately_to_another_collection_for_one_item_not_within_precision()
        {
            double[] subject = [1, 2, 3.01];
            double[] expected = [1, 2, 3];

            Action act = () => subject.Should().EqualApproximately(expected, 0.001);

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {1.0, 2.0, 3.0} ±0.001, but {1.0, 2.0, 3.01} differed by 0.0099* at index 2.");
        }

        [Fact]
        public void Integer_collection_is_equal_approximately_to_another_collection_with_same_special_values()
        {
            int[] subject = [-1, 0, 1, int.MinValue, int.MaxValue];
            int[] expected = [-1, 0, 1, int.MinValue, int.MaxValue];

            subject.Should().EqualApproximately(expected, 1);
        }

        [Fact]
        public void Integer_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            int[] subject = [-100, 0, 100];
            int[] expected = [-101, 0, 100];

            subject.Should().EqualApproximately(expected, 1);
        }

        [Fact]
        public void Integer_collection_is_not_equal_approximately_to_another_collection_for_one_item_not_within_precision()
        {
            int[] subject = [-100, 0, 100];
            int[] expected = [-102, 0, 100];

            Action act = () => subject.Should().EqualApproximately(expected, 1);

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {-102, 0, 100} ±1, but {-100, 0, 100} differed by 2 at index 0.");
        }

        [Fact]
        public void When_two_integer_collections_are_not_equal_because_one_item_differs_by_more_than_maximum_possible_value_it_should_throw()
        {
            int[] subject = [int.MinValue];
            int[] expected = [int.MaxValue];

            Action act = () => subject.Should().EqualApproximately(expected, 1);

            act.Should().Throw<XunitException>().WithMessage(
                "*to approximate {2147483647} ±1, but {-2147483648} differed at index 0.");
        }
    }

    public sealed class NotEqualApproximately
    {
        [Fact]
        public void Two_null_collections_cannot_be_compared()
        {
            float[] subject = null;
            float[] unexpected = null;

            Action act = () => subject.Should().NotEqualApproximately(unexpected, 0.1f);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("unexpected");
        }

        [Fact]
        public void Subject_collection_with_more_items_than_expectation_is_not_equal()
        {
            float[] subject = [1, 2, 3];
            float[] unexpected = [1, 2];

            subject.Should().NotEqualApproximately(unexpected, 0.1f);
        }

        [Fact]
        public void Subject_collection_is_approximately_equal_to_same_collection()
        {
            float[] subjectAndExpected = [1, 2, 3];

            Action act = () => subjectAndExpected.Should().NotEqualApproximately(subjectAndExpected, 0.1f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*collections to be approximately equal within ±0.1F because *failure message, but they both reference the same object.");
        }

        [Fact]
        public void Subject_collection_with_less_items_than_expectation_is_not_equal()
        {
            float[] subject = [1, 2, 3];
            float[] expected = [1, 2, 3, 4];

            subject.Should().NotEqualApproximately(expected, 0.1f);
        }

        [Fact]
        public void Subject_collection_cannot_be_null_if_expectation_is_not_null()
        {
            float[] subject = null;
            float[] expected = [1, 2, 3];

            Action act = () => subject.Should().NotEqualApproximately(expected, 0.1f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*to be approximately equal within ±0.1F because *failure message, but found <null>.");
        }

        [Fact]
        public void Expectation_collection_cannot_be_null_if_subject_is_not_null()
        {
            float[] subject = [1, 2, 3];
            float[] expected = null;

            Action act = () => subject.Should().NotEqualApproximately(expected, 0.1f);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot compare collection with <null>.*")
                .WithParameterName("unexpected");
        }

        [Fact]
        public void An_empty_collection_is_not_approximately_equal_to_a_non_empty_collection()
        {
            float[] subject = [];
            float[] expected = [1, 2, 3];

            subject.Should().NotEqualApproximately(expected, 0.1f);
        }

        [Fact]
        public void An_non_empty_collection_is_not_approximately_equal_to_an_empty_collection()
        {
            float[] subject = [1, 2, 3];
            float[] expected = [];

            subject.Should().NotEqualApproximately(expected, 0.1f);
        }

        [Theory]
        [InlineData(float.PositiveInfinity, float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity, float.NaN)]
        [InlineData(float.MaxValue, float.MinValue)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void Float_collection_is_not_approximately_equal_to_another_collection_with_different_special_values(float subjectValue, float expectedValue)
        {
            float[] subject = [subjectValue];
            float[] expected = [expectedValue];

            subject.Should().NotEqualApproximately(expected, 0.0f);
        }

        [Fact]
        public void Float_collection_is_equal_approximately_to_another_collection_within_given_precision()
        {
            float[] subject = [-1.001f, 0, 1.01f];
            float[] expected = [-1, 0, 1];

            Action act = () => subject.Should().NotEqualApproximately(expected, 0.01f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "Did not expect collections {-1F, 0F, 1F} and {-1.001F, 0F, 1.01F} to be approximately equal within ±0.01F because *failure message.");
        }

        [Theory]
        [InlineData(float.PositiveInfinity)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.NaN)]
        [InlineData(float.MaxValue)]
        [InlineData(float.MinValue)]
        public void Float_collection_is_approximately_equal_to_another_collection_with_same_special_values(float specialValue)
        {
            float[] subject = [specialValue];
            float[] expected = [specialValue];

            Action act = () => subject.Should().NotEqualApproximately(expected, 0.0f, "we want to test the failure {0}", "message");

            act.Should().Throw<XunitException>().WithMessage(
                "*collections {*} and {*} to be approximately equal within ±0F because *failure message.");
        }
    }
}
#endif
