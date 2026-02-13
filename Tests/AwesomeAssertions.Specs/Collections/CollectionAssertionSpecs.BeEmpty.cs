using System;
using System.Collections;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class BeEmpty
    {
        [Fact]
        public void When_collection_is_empty_as_expected_it_should_not_throw()
        {
            int[] collection = [];

            collection.Should().BeEmpty();
        }

        [Fact]
        public void When_collection_is_not_empty_unexpectedly_it_should_throw()
        {
            int[] collection = [1, 2, 3];

            Action act = () => collection.Should().BeEmpty("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("*to be empty because we want to test the failure message, but found at least one item*1*");
        }

        [Fact]
        public void When_asserting_collection_with_items_is_not_empty_it_should_succeed()
        {
            int[] collection = [1, 2, 3];

            collection.Should().NotBeEmpty();
        }

        [Fact]
        public void When_asserting_collection_with_items_is_not_empty_it_should_enumerate_the_collection_only_once()
        {
            var trackingEnumerable = new TrackingTestEnumerable(1, 2, 3);

            trackingEnumerable.Should().NotBeEmpty();

            trackingEnumerable.Enumerator.LoopCount.Should().Be(1);
        }

        [Fact]
        public void When_asserting_collection_without_items_is_not_empty_it_should_fail()
        {
            int[] collection = [];

            Action act = () => collection.Should().NotBeEmpty();

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_asserting_collection_without_items_is_not_empty_it_should_fail_with_descriptive_message_()
        {
            int[] collection = [];

            Action act = () => collection.Should().NotBeEmpty("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection not to be empty because we want to test the failure message.");
        }

        [Fact]
        public void When_asserting_collection_to_be_empty_but_collection_is_null_it_should_throw()
        {
            IEnumerable<object> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().BeEmpty("we want to test the {0} message", "failure");
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection to be empty because we want to test the failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collection_to_be_empty_it_should_enumerate_only_once()
        {
            var collection = new CountingGenericEnumerable<int>([]);

            collection.Should().BeEmpty();

            collection.GetEnumeratorCallCount.Should().Be(1);
        }

        [Fact]
        public void When_asserting_non_empty_collection_is_empty_it_should_enumerate_only_once()
        {
            var collection = new CountingGenericEnumerable<int>([1, 2, 3]);

            Action act = () => collection.Should().BeEmpty();

            act.Should().Throw<XunitException>().WithMessage("*to be empty, but found at least one item {1}.");
            collection.GetEnumeratorCallCount.Should().Be(1);
        }

        [Fact]
        public void When_asserting_collection_to_not_be_empty_but_collection_is_null_it_should_throw()
        {
            IEnumerable<object> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().NotBeEmpty("we want to test the {0} message", "failure");
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection not to be empty because*failure message*, but found <null>.");
        }

        [Fact]
        public void When_asserting_an_infinite_collection_to_be_empty_it_should_throw_correctly()
        {
            var collection = new InfiniteEnumerable();

            Action act = () => collection.Should().BeEmpty();

            act.Should().Throw<XunitException>();
        }
    }

    public class NotBeEmpty
    {
        [Fact]
        public void When_asserting_collection_to_be_not_empty_but_collection_is_null_it_should_throw()
        {
            int[] collection = null;

            Action act = () => collection.Should().NotBeEmpty("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected collection not to be empty because we want to test the failure message, but found <null>.");
        }

        [Fact]
        public void When_asserting_collection_to_be_not_empty_it_should_enumerate_only_once()
        {
            var collection = new CountingGenericEnumerable<int>([42]);

            collection.Should().NotBeEmpty();

            collection.GetEnumeratorCallCount.Should().Be(1);
        }
    }

    private sealed class InfiniteEnumerable : IEnumerable<object>
    {
        public IEnumerator<object> GetEnumerator() => new InfiniteEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class InfiniteEnumerator : IEnumerator<object>
    {
        public bool MoveNext() => true;

        public void Reset() { }

        public object Current => new();

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}
