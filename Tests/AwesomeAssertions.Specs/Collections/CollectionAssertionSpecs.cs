using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <summary>
/// Collection assertion specs.
/// </summary>
public partial class CollectionAssertionSpecs
{
    public class Chaining
    {
        [Fact]
        public void Chaining_something_should_do_something()
        {
            // Arrange
            var languages = new[] { "C#" };

            // Act
            var act = () => languages.Should().ContainSingle()
                .Which.Should().EndWith("script");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("Expected languages[0]*");
        }

        [Fact]
        public void Should_support_chaining_constraints_with_and()
        {
            // Arrange
            int[] collection = [1, 2, 3];

            // Act / Assert
            collection.Should()
                .HaveCount(3)
                .And
                .HaveElementAt(1, 2)
                .And
                .NotContain(4);
        }

        [Fact]
        public void When_the_collection_is_ordered_according_to_the_subsequent_ascending_assertion_it_should_succeed()
        {
            // Arrange
            (int, string)[] collection =
            [
                (1, "a"),
                (2, "b"),
                (2, "c"),
                (3, "a")
            ];

            // Act / Assert
            collection.Should()
                .BeInAscendingOrder(x => x.Item1)
                .And
                .ThenBeInAscendingOrder(x => x.Item2);
        }

        [Fact]
        public void When_the_collection_is_not_ordered_according_to_the_subsequent_ascending_assertion_it_should_fail()
        {
            // Arrange
            (int, string)[] collection =
            [
                (1, "a"),
                (2, "b"),
                (2, "c"),
                (3, "a")
            ];

            // Act
            Action action = () => collection.Should()
                .BeInAscendingOrder(x => x.Item1)
                .And
                .BeInAscendingOrder(x => x.Item2);

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected collection*to be ordered \"by Item2\"*");
        }

        [Fact]
        public void
            When_the_collection_is_ordered_according_to_the_subsequent_ascending_assertion_with_comparer_it_should_succeed()
        {
            // Arrange
            (int, string)[] collection =
            [
                (1, "a"),
                (2, "B"),
                (2, "b"),
                (3, "a")
            ];

            // Act / Assert
            collection.Should()
                .BeInAscendingOrder(x => x.Item1)
                .And
                .ThenBeInAscendingOrder(x => x.Item2, StringComparer.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void When_the_collection_is_ordered_according_to_the_multiple_subsequent_ascending_assertions_it_should_succeed()
        {
            // Arrange
            (int, string, double)[] collection =
            [
                (1, "a", 1.1),
                (2, "b", 1.2),
                (2, "c", 1.3),
                (3, "a", 1.1)
            ];

            // Act / Assert
            collection.Should()
                .BeInAscendingOrder(x => x.Item1)
                .And
                .ThenBeInAscendingOrder(x => x.Item2)
                .And
                .ThenBeInAscendingOrder(x => x.Item3);
        }

        [Fact]
        public void When_the_collection_is_ordered_according_to_the_subsequent_descending_assertion_it_should_succeed()
        {
            // Arrange
            (int, string)[] collection =
            [
                (3, "a"),
                (2, "c"),
                (2, "b"),
                (1, "a")
            ];

            // Act / Assert
            collection.Should()
                .BeInDescendingOrder(x => x.Item1)
                .And
                .ThenBeInDescendingOrder(x => x.Item2);
        }

        [Fact]
        public void When_the_collection_is_not_ordered_according_to_the_subsequent_descending_assertion_it_should_fail()
        {
            // Arrange
            (int, string)[] collection =
            [
                (3, "a"),
                (2, "c"),
                (2, "b"),
                (1, "a")
            ];

            // Act
            Action action = () => collection.Should()
                .BeInDescendingOrder(x => x.Item1)
                .And
                .BeInDescendingOrder(x => x.Item2);

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected collection*to be ordered \"by Item2\"*");
        }

        [Fact]
        public void
            When_the_collection_is_ordered_according_to_the_subsequent_descending_assertion_with_comparer_it_should_succeed()
        {
            // Arrange
            (int, string)[] collection =
            [
                (3, "a"),
                (2, "b"),
                (2, "B"),
                (1, "a")
            ];

            // Act / Assert
            collection.Should()
                .BeInDescendingOrder(x => x.Item1)
                .And
                .ThenBeInDescendingOrder(x => x.Item2, StringComparer.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void When_the_collection_is_ordered_according_to_the_multiple_subsequent_descending_assertions_it_should_succeed()
        {
            // Arrange
            (int, string, double)[] collection =
            [
                (3, "a", 1.1),
                (2, "c", 1.3),
                (2, "b", 1.2),
                (1, "a", 1.1)
            ];

            // Act / Assert
            collection.Should()
                .BeInDescendingOrder(x => x.Item1)
                .And
                .ThenBeInDescendingOrder(x => x.Item2)
                .And
                .ThenBeInDescendingOrder(x => x.Item3);
        }

        [Fact]
        public void When_asserting_ordering_by_property_of_a_null_collection_failed_inside_a_scope_then_a_subsequent_assertion_is_not_evaluated()
        {
            // Arrange
            const IEnumerable<SomeClass> collection = null;

            // Act
            Action act = () =>
            {
                using (new AssertionScope())
                {
                    collection.Should().BeInAscendingOrder(o => o.Text)
                        .And.NotBeEmpty("this won't be asserted");
                }
            };

            // AssertText but found <null>.
            act.Should().Throw<XunitException>()
               .WithMessage("*Text*found*<null>.");
        }

        [Fact]
        public void When_asserting_ordering_with_given_comparer_of_a_null_collection_failed_inside_a_scope_then_a_subsequent_assertion_is_not_evaluated()
        {
            // Arrange
            const IEnumerable<SomeClass> collection = null;

            // Act
            Action act = () =>
            {
                using (new AssertionScope())
                {
                    collection.Should().BeInAscendingOrder(Comparer<SomeClass>.Default)
                        .And.NotBeEmpty("this won't be asserted");
                }
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected*found*<null>.");
        }

        [Fact]
        public void When_asserting_ordering_of_an_unordered_collection_failed_inside_a_scope_then_a_subsequent_assertion_is_not_evaluated()
        {
            // Arrange
            int[] collection = [1, 27, 12];

            // Act
            Action action = () =>
            {
                using (new AssertionScope())
                {
                    collection.Should().BeInAscendingOrder("because numbers are ordered")
                        .And.BeEmpty("this won't be asserted");
                }
            };

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("*item at index 1 is in wrong order.");
        }
    }

    private class ByLastCharacterComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return Nullable.Compare(x?[^1], y?[^1]);
        }
    }
}

internal class CountingGenericEnumerable<TElement> : IEnumerable<TElement>
{
    private readonly IEnumerable<TElement> backingSet;

    public CountingGenericEnumerable(IEnumerable<TElement> backingSet)
    {
        this.backingSet = backingSet;
        GetEnumeratorCallCount = 0;
    }

    public int GetEnumeratorCallCount { get; private set; }

    public IEnumerator<TElement> GetEnumerator()
    {
        GetEnumeratorCallCount++;
        return backingSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class CountingGenericCollection<TElement> : ICollection<TElement>
{
    private readonly ICollection<TElement> backingSet;

    public CountingGenericCollection(ICollection<TElement> backingSet)
    {
        this.backingSet = backingSet;
    }

    public int GetEnumeratorCallCount { get; private set; }

    public IEnumerator<TElement> GetEnumerator()
    {
        GetEnumeratorCallCount++;
        return backingSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(TElement item) { throw new NotImplementedException(); }

    public void Clear() { throw new NotImplementedException(); }

    public bool Contains(TElement item) { throw new NotImplementedException(); }

    public void CopyTo(TElement[] array, int arrayIndex) { throw new NotImplementedException(); }

    public bool Remove(TElement item) { throw new NotImplementedException(); }

    public int GetCountCallCount { get; private set; }

    public int Count
    {
        get
        {
            GetCountCallCount++;
            return backingSet.Count;
        }
    }

    public bool IsReadOnly { get; private set; }
}

internal sealed class TrackingTestEnumerable : IEnumerable<int>
{
    public TrackingTestEnumerable(params int[] values)
    {
        Enumerator = new TrackingEnumerator(values);
    }

    public TrackingEnumerator Enumerator { get; }

    public IEnumerator<int> GetEnumerator()
    {
        Enumerator.IncreaseEnumerationCount();
        Enumerator.Reset();
        return Enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class TrackingEnumerator : IEnumerator<int>
{
    private readonly int[] values;
    private int index;

    public TrackingEnumerator(int[] values)
    {
        index = -1;

        this.values = values;
    }

    public int LoopCount { get; private set; }

    public void IncreaseEnumerationCount()
    {
        LoopCount++;
    }

    public bool MoveNext()
    {
        index++;
        return index < values.Length;
    }

    public void Reset()
    {
        index = -1;
    }

    public void Dispose() { }

    object IEnumerator.Current => Current;

    public int Current => values[index];
}

internal class OneTimeEnumerable<T> : IEnumerable<T>
{
    private readonly T[] items;
    private int enumerations;

    public OneTimeEnumerable(params T[] items) => this.items = items;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        if (enumerations++ > 0)
        {
            throw new InvalidOperationException("OneTimeEnumerable can be enumerated one time only");
        }

        return items.AsEnumerable().GetEnumerator();
    }
}

internal class SomeClass
{
    public string Text { get; set; }

    public int Number { get; set; }
}
