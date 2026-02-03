using System;
using System.Collections.Immutable;
using System.Linq;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class BeEquivalentTo
    {
        [Fact]
        public void When_two_collections_contain_the_same_elements_it_should_treat_them_as_equivalent()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [3, 1, 2];

            collection1.Should().BeEquivalentTo(collection2);
        }

        [Fact]
        public void When_a_collection_contains_same_elements_it_should_treat_it_as_equivalent()
        {
            int[] collection = [1, 2, 3];

            collection.Should().BeEquivalentTo([3, 1, 2]);
        }

        [Fact]
        public void When_character_collections_are_equivalent_it_should_not_throw()
        {
            char[] list1 = "abc123ab".ToCharArray();
            char[] list2 = "abc123ab".ToCharArray();

            list1.Should().BeEquivalentTo(list2);
        }

        [Fact]
        public void When_collections_are_not_equivalent_it_should_throw()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [1, 2];

            Action act = () => collection1.Should().BeEquivalentTo(collection2, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected*collection*2 item(s) because*failure message, but *1 item(s) more than*");
        }

        [Fact]
        public void When_collections_with_duplicates_are_not_equivalent_it_should_throw()
        {
            int[] collection1 = [1, 2, 3, 1];
            int[] collection2 = [1, 2, 3, 3];

            Action act = () => collection1.Should().BeEquivalentTo(collection2);

            act.Should().Throw<XunitException>().WithMessage("Expected collection1[3]*to be 3, but found 1*");
        }

        [Fact]
        public void When_testing_for_equivalence_against_empty_collection_it_should_throw()
        {
            int[] subject = [1, 2, 3];
            int[] otherCollection = [];

            Action act = () => subject.Should().BeEquivalentTo(otherCollection);

            act.Should().Throw<XunitException>().WithMessage(
                "Expected subject to be a collection with 0 item(s), but*contains 3 item(s)*");
        }

        [Fact]
        public void When_two_collections_are_both_empty_it_should_treat_them_as_equivalent()
        {
            int[] subject = [];
            int[] otherCollection = [];

            subject.Should().BeEquivalentTo(otherCollection);
        }

        [Fact]
        public void When_testing_for_equivalence_against_null_collection_it_should_throw()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = null;

            Action act = () => collection1.Should().BeEquivalentTo(collection2, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("Expected*<null>*failure message*but found {1, 2, 3}*");
        }

        [Fact]
        public void When_asserting_collections_to_be_equivalent_but_subject_collection_is_null_it_should_throw()
        {
            int[] collection = null;
            int[] collection1 = [1, 2, 3];

            Action act = () =>
                collection.Should().BeEquivalentTo(collection1, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("Expected collection not to be <null> because*failure message*");
        }

        [Fact]
        public void Default_immutable_arrays_should_be_equivalent()
        {
            ImmutableArray<string> collection = default;
            ImmutableArray<string> collection1 = default;

            collection.Should().BeEquivalentTo(collection1);
        }

        [Fact]
        public void Default_immutable_lists_should_be_equivalent()
        {
            ImmutableList<string> collection = null;
            ImmutableList<string> collection1 = null;

            collection.Should().BeEquivalentTo(collection1);
        }

        [Fact]
        public void Can_ignore_casing_while_comparing_collections_of_strings()
        {
            var actual = new[] { "first", "test", "last" };
            var expectation = new[] { "first", "TEST", "last" };

            actual.Should().BeEquivalentTo(expectation, o => o.IgnoringCase());
        }

        [Fact]
        public void Can_ignore_leading_whitespace_while_comparing_collections_of_strings()
        {
            var actual = new[] { "first", "  test", "last" };
            var expectation = new[] { "first", "test", "last" };

            actual.Should().BeEquivalentTo(expectation, o => o.IgnoringLeadingWhitespace());
        }

        [Fact]
        public void Can_ignore_trailing_whitespace_while_comparing_collections_of_strings()
        {
            var actual = new[] { "first", "test  ", "last" };
            var expectation = new[] { "first", "test", "last" };

            actual.Should().BeEquivalentTo(expectation, o => o.IgnoringTrailingWhitespace());
        }

        [Fact]
        public void Can_ignore_newline_style_while_comparing_collections_of_strings()
        {
            var actual = new[] { "first", "A\nB\r\nC", "last" };
            var expectation = new[] { "first", "A\r\nB\nC", "last" };

            actual.Should().BeEquivalentTo(expectation, o => o.IgnoringNewlineStyle());
        }
    }

    public class NotBeEquivalentTo
    {
        [Fact]
        public void When_collection_is_not_equivalent_to_another_smaller_collection_it_should_succeed()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [3, 1];

            collection1.Should().NotBeEquivalentTo(collection2);
        }

        [Fact]
        public void When_large_collection_is_equivalent_to_another_equally_size_collection_it_should_throw()
        {
            var collection1 = Enumerable.Repeat(1, 10000);
            var collection2 = Enumerable.Repeat(1, 10000);

            Action act = () => collection1.Should().NotBeEquivalentTo(collection2);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_collection_is_not_equivalent_to_another_equally_sized_collection_it_should_succeed()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [3, 1, 4];

            collection1.Should().NotBeEquivalentTo(collection2);
        }

        [Fact]
        public void When_collections_are_unexpectedly_equivalent_it_should_throw()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [3, 1, 2];

            Action act = () => collection1.Should().NotBeEquivalentTo(collection2);

            act.Should().Throw<XunitException>().WithMessage("Expected collection1 {1, 2, 3} not*equivalent*{3, 1, 2}.");
        }

        [Fact]
        public void When_asserting_collections_not_to_be_equivalent_but_subject_collection_is_null_it_should_throw()
        {
            int[] actual = null;
            int[] expectation = [1, 2, 3];

            Action act = () =>
            {
                using var _ = new AssertionScope();
                actual.Should().NotBeEquivalentTo(expectation, "we want to test the {0} message", "failure");
            };

            act.Should().Throw<XunitException>().WithMessage("*be equivalent because*failure message, but found <null>*");
        }

        [Fact]
        public void When_non_empty_collection_is_not_expected_to_be_equivalent_to_an_empty_collection_it_should_succeed()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = [];

            collection1.Should().NotBeEquivalentTo(collection2);
        }

        [Fact]
        public void When_testing_collections_not_to_be_equivalent_against_null_collection_it_should_throw()
        {
            int[] collection1 = [1, 2, 3];
            int[] collection2 = null;

            Action act = () => collection1.Should().NotBeEquivalentTo(collection2);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot verify inequivalence against a <null> collection.*")
                .WithParameterName("unexpected");
        }

        [Fact]
        public void When_testing_collections_not_to_be_equivalent_against_same_collection_it_should_throw()
        {
            int[] collection = [1, 2, 3];
            var collection1 = collection;

            Action act = () => collection.Should().NotBeEquivalentTo(collection1, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "*not to be equivalent*because*failure message, but they both reference the same object.");
        }

        [Fact]
        public void When_a_collections_is_equivalent_to_an_approximate_copy_it_should_throw()
        {
            double[] collection = [1.0, 2.0, 3.0];
            double[] collection1 = [1.5, 2.5, 3.5];

            Action act = () => collection.Should().NotBeEquivalentTo(collection1, opt => opt
                    .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.5))
                    .WhenTypeIs<double>(),
                "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("*not to be equivalent*because*failure message*");
        }

        [Fact]
        public void When_asserting_collections_not_to_be_equivalent_with_options_but_subject_collection_is_null_it_should_throw()
        {
            int[] actual = null;
            int[] expectation = [1, 2, 3];

            Action act = () =>
            {
                using var _ = new AssertionScope();
                actual.Should().NotBeEquivalentTo(expectation, opt => opt, "we want to test the {0} message", "failure");
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected actual not to be equivalent because*failure message*, but found <null>.*");
        }

        [Fact]
        public void Default_immutable_array_should_not_be_equivalent_to_initialized_immutable_array()
        {
            ImmutableArray<string> subject = default;
            ImmutableArray<string> expectation = ImmutableArray.Create("a", "b", "c");

            subject.Should().NotBeEquivalentTo(expectation);
        }

        [Fact]
        public void Immutable_array_should_not_be_equivalent_to_default_immutable_array()
        {
            ImmutableArray<string> collection = ImmutableArray.Create("a", "b", "c");
            ImmutableArray<string> collection1 = default;

            collection.Should().NotBeEquivalentTo(collection1);
        }
    }
}
