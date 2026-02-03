using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class BeInAscendingOrder
    {
        [Fact]
        public void When_asserting_a_null_collection_to_be_in_ascending_order_it_should_throw()
        {
            List<int> result = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                result.Should().BeInAscendingOrder();
            };

            act.Should().Throw<XunitException>().WithMessage("*but found <null>*");
        }

        [Fact]
        public void When_asserting_the_items_in_an_ascendingly_ordered_collection_are_ordered_ascending_it_should_succeed()
        {
            int[] collection = [1, 2, 2, 3];

            collection.Should().BeInAscendingOrder();
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_ascendingly_ordered_collection_are_ordered_ascending_using_the_given_comparer_it_should_succeed()
        {
            int[] collection = [1, 2, 2, 3];

            collection.Should().BeInAscendingOrder(Comparer<int>.Default);
        }

        [Fact]
        public void When_asserting_the_items_in_an_unordered_collection_are_ordered_ascending_it_should_throw()
        {
            int[] collection = [1, 6, 12, 15, 12, 17, 26];

            Action action = () => collection.Should().BeInAscendingOrder("we want to test the {0} message", "failure");

            action.Should().Throw<XunitException>().WithMessage(
                "Expected collection to be in ascending order because*failure message,"
                + " but found {1, 6, 12, 15, 12, 17, 26} where item at index 3 is in wrong order.");
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_unordered_collection_are_ordered_ascending_using_the_given_comparer_it_should_throw()
        {
            int[] collection = [1, 6, 12, 15, 12, 17, 26];

            Action action = () =>
                collection.Should().BeInAscendingOrder(Comparer<int>.Default, "we want to test the {0} message", "failure");

            action.Should().Throw<XunitException>().WithMessage(
                "Expected collection to be in ascending order because*failure message,"
                + " but found {1, 6, 12, 15, 12, 17, 26} where item at index 3 is in wrong order.");
        }

        [Fact]
        public void Items_can_be_ordered_by_the_identity_function()
        {
            int[] collection = [1, 2];

            collection.Should().BeInAscendingOrder(x => x);
        }

        [Fact]
        public void When_asserting_empty_collection_with_no_parameters_ordered_in_ascending_it_should_succeed()
        {
            int[] collection = [];

            collection.Should().BeInAscendingOrder();
        }

        [Fact]
        public void When_asserting_empty_collection_by_property_expression_ordered_in_ascending_it_should_succeed()
        {
            IEnumerable<SomeClass> collection = [];

            collection.Should().BeInAscendingOrder(o => o.Number);
        }

        [Fact]
        public void When_asserting_single_element_collection_with_no_parameters_ordered_in_ascending_it_should_succeed()
        {
            int[] collection = [42];

            collection.Should().BeInAscendingOrder();
        }

        [Fact]
        public void When_asserting_single_element_collection_by_property_expression_ordered_in_ascending_it_should_succeed()
        {
            var collection = new SomeClass[] { new() { Text = "a", Number = 1 } };

            collection.Should().BeInAscendingOrder(o => o.Number);
        }

        [Fact]
        public void Can_use_a_cast_expression_in_the_ordering_expression()
        {
            var collection = new SomeClass[] { new() { Text = "a", Number = 1 } };

            collection.Should().BeInAscendingOrder(o => (float)o.Number);
        }

        [Fact]
        public void Can_use_an_index_into_a_list_in_the_ordering_expression()
        {
            List<SomeClass>[] collection = [[new() { Text = "a", Number = 1 }]];

            collection.Should().BeInAscendingOrder(o => o[0].Number);
        }

        [Fact]
        public void Can_use_an_index_into_an_array_in_the_ordering_expression()
        {
            SomeClass[][] collection = [[new SomeClass { Text = "a", Number = 1 }]];

            collection.Should().BeInAscendingOrder(o => o[0].Number);
        }

        [Fact]
        public void Unsupported_ordering_expressions_are_invalid()
        {
            var collection = new SomeClass[] { new() { Text = "a", Number = 1 } };

            Action act = () => collection.Should().BeInAscendingOrder(o => o.Number > 1);

            act.Should().Throw<ArgumentException>().WithMessage("*Expression <*> cannot be used to select a member.*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_unordered_collection_are_ordered_ascending_using_the_specified_property_it_should_throw()
        {
            var collection = new[]
            {
                new { Text = "b", Numeric = 1 },
                new { Text = "c", Numeric = 2 },
                new { Text = "a", Numeric = 3 }
            };

            Action act = () => collection.Should().BeInAscendingOrder(o => o.Text, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection*b*c*a*ordered*Text*because*failure message*a*b*c*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_unordered_collection_are_ordered_ascending_using_the_specified_property_and_the_given_comparer_it_should_throw()
        {
            var collection = new[]
            {
                new { Text = "b", Numeric = 1 },
                new { Text = "c", Numeric = 2 },
                new { Text = "a", Numeric = 3 }
            };

            Action act = () => collection.Should().BeInAscendingOrder(
                o => o.Text, StringComparer.OrdinalIgnoreCase, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection*b*c*a*ordered*Text*because*failure message*a*b*c*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_ascendingly_ordered_collection_are_ordered_ascending_using_the_specified_property_it_should_succeed()
        {
            var collection = new[]
            {
                new { Text = "b", Numeric = 1 },
                new { Text = "c", Numeric = 2 },
                new { Text = "a", Numeric = 3 }
            };

            collection.Should().BeInAscendingOrder(o => o.Numeric);
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_ascendingly_ordered_collection_are_ordered_ascending_using_the_specified_property_and_the_given_comparer_it_should_succeed()
        {
            var collection = new[]
            {
                new { Text = "b", Numeric = 1 },
                new { Text = "c", Numeric = 2 },
                new { Text = "a", Numeric = 3 }
            };

            collection.Should().BeInAscendingOrder(o => o.Numeric, Comparer<int>.Default);
        }

        [Fact]
        public void When_strings_are_in_ascending_order_it_should_succeed()
        {
            string[] strings = ["alpha", "beta", "theta"];

            strings.Should().BeInAscendingOrder();
        }

        [Fact]
        public void When_strings_are_not_in_ascending_order_it_should_throw()
        {
            string[] strings = ["theta", "alpha", "beta"];

            Action act = () => strings.Should().BeInAscendingOrder("we want to test the {0} message", "failure");

            act.Should()
                .Throw<XunitException>()
                .WithMessage("Expected*ascending*because*failure message*index 0*");
        }

        [Fact]
        public void When_strings_are_in_ascending_order_according_to_a_custom_comparer_it_should_succeed()
        {
            string[] strings = ["alpha", "beta", "theta"];

            strings.Should().BeInAscendingOrder(new ByLastCharacterComparer());
        }

        [Fact]
        public void When_strings_are_not_in_ascending_order_according_to_a_custom_comparer_it_should_throw()
        {
            string[] strings = ["dennis", "roy", "thomas"];

            Action act = () =>
                strings.Should().BeInAscendingOrder(new ByLastCharacterComparer(), "we want to test the {0} message", "failure");

            act.Should()
                .Throw<XunitException>()
                .WithMessage("Expected*ascending*because*failure message*index 1*");
        }

        [Fact]
        public void When_strings_are_in_ascending_order_according_to_a_custom_lambda_it_should_succeed()
        {
            string[] strings = ["alpha", "beta", "theta"];

            strings.Should().BeInAscendingOrder((sut, exp) => sut[^1].CompareTo(exp[^1]));
        }

        [Fact]
        public void When_strings_are_not_in_ascending_order_according_to_a_custom_lambda_it_should_throw()
        {
            string[] strings = ["dennis", "roy", "thomas"];

            Action act = () => strings.Should().BeInAscendingOrder(
                (sut, exp) => sut[^1].CompareTo(exp[^1]), "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("Expected*ascending*because*failure message*index 1*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_null_collection_are_ordered_using_the_specified_property_it_should_throw()
        {
            const IEnumerable<SomeClass> collection = null;

            Action act = () => collection.Should().BeInAscendingOrder(o => o.Text);

            act.Should().Throw<XunitException>().WithMessage("*Text*found*null*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_null_collection_are_ordered_using_the_given_comparer_it_should_throw()
        {
            const IEnumerable<SomeClass> collection = null;

            Action act = () => collection.Should().BeInAscendingOrder(Comparer<SomeClass>.Default);

            act.Should().Throw<XunitException>().WithMessage("Expected*found*null*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_a_null_collection_are_ordered_using_the_specified_property_and_the_given_comparer_it_should_throw()
        {
            const IEnumerable<SomeClass> collection = null;

            Action act = () => collection.Should().BeInAscendingOrder(o => o.Text, StringComparer.OrdinalIgnoreCase);

            act.Should().Throw<XunitException>().WithMessage("Expected*Text*found*null*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_collection_are_ordered_and_the_specified_property_is_null_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().BeInAscendingOrder((Expression<Func<SomeClass, string>>)null);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot assert collection ordering without specifying a property*")
                .WithParameterName("propertyExpression");
        }

        [Fact]
        public void When_asserting_the_items_in_a_collection_are_ordered_and_the_given_comparer_is_null_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().BeInAscendingOrder(comparer: null);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot assert collection ordering without specifying a comparer*")
                .WithParameterName("comparer");
        }

        [Fact]
        public void When_asserting_the_items_in_ay_collection_are_ordered_using_an_invalid_property_expression_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().BeInAscendingOrder(o => o.GetHashCode());

            act.Should().Throw<ArgumentException>().WithMessage("Expression*o.GetHashCode()*cannot be used to select a member*");
        }
    }

    public class NotBeInAscendingOrder
    {
        [Fact]
        public void When_asserting_a_null_collection_to_not_be_in_ascending_order_it_should_throw()
        {
            List<int> result = null;

            Action act = () => result.Should().NotBeInAscendingOrder();

            act.Should().Throw<XunitException>().WithMessage("*but found <null>*");
        }

        [Fact]
        public void When_asserting_the_items_in_an_unordered_collection_are_not_in_ascending_order_it_should_succeed()
        {
            int[] collection = [1, 5, 3];

            collection.Should().NotBeInAscendingOrder();
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_unordered_collection_are_not_in_ascending_order_using_the_given_comparer_it_should_succeed()
        {
            int[] collection = [1, 5, 3];

            collection.Should().NotBeInAscendingOrder(Comparer<int>.Default);
        }

        [Fact]
        public void When_asserting_the_items_in_an_ascendingly_ordered_collection_are_not_in_ascending_order_it_should_throw()
        {
            int[] collection = [1, 2, 2, 3];

            Action action = () => collection.Should().NotBeInAscendingOrder("we want to test the {0} message", "failure");

            action.Should().Throw<XunitException>().WithMessage(
                "Did not expect collection to be in ascending order because*failure message, but found {1, 2, 2, 3}.");
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_ascendingly_ordered_collection_are_not_in_ascending_order_using_the_given_comparer_it_should_throw()
        {
            int[] collection = [1, 2, 2, 3];

            Action action = () =>
                collection.Should().NotBeInAscendingOrder(Comparer<int>.Default, "we want to test the {0} message", "failure");

            action.Should().Throw<XunitException>().WithMessage(
                "Did not expect collection to be in ascending order because*failure message, but found {1, 2, 2, 3}.");
        }

        [Fact]
        public void When_asserting_empty_collection_by_property_expression_to_not_be_ordered_in_ascending_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().NotBeInAscendingOrder(o => o.Number);

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection {empty} to not be ordered \"by Number\" and not result in {empty}.");
        }

        [Fact]
        public void When_asserting_empty_collection_with_no_parameters_not_be_ordered_in_ascending_it_should_throw()
        {
            int[] collection = [];

            Action act = () => collection.Should().NotBeInAscendingOrder("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect collection to be in ascending order because*failure message, but found {empty}.");
        }

        [Fact]
        public void When_asserting_single_element_collection_with_no_parameters_not_be_ordered_in_ascending_it_should_throw()
        {
            int[] collection = [42];

            Action act = () => collection.Should().NotBeInAscendingOrder();

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect collection to be in ascending order, but found {42}.");
        }

        [Fact]
        public void
            When_asserting_single_element_collection_by_property_expression_to_not_be_ordered_in_ascending_it_should_throw()
        {
            var collection = new SomeClass[] { new() { Text = "a", Number = 1 } };

            Action act = () => collection.Should().NotBeInAscendingOrder(o => o.Number);

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void
            When_asserting_the_items_in_a_ascending_ordered_collection_are_not_ordered_ascending_using_the_given_comparer_it_should_throw()
        {
            int[] collection = [1, 2, 3];

            Action act = () =>
                collection.Should().NotBeInAscendingOrder(Comparer<int>.Default, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect collection to be in ascending order*because*failure message*1*2*3*");
        }

        [Fact]
        public void
            When_asserting_the_items_not_in_an_ascendingly_ordered_collection_are_not_ordered_ascending_using_the_given_comparer_it_should_succeed()
        {
            int[] collection = [3, 2, 1];

            collection.Should().NotBeInAscendingOrder(Comparer<int>.Default);
        }

        [Fact]
        public void
            When_asserting_the_items_in_a_ascending_ordered_collection_are_not_ordered_ascending_using_the_specified_property_it_should_throw()
        {
            var collection = new[]
            {
                new { Text = "a", Numeric = 3 },
                new { Text = "b", Numeric = 1 },
                new { Text = "c", Numeric = 2 }
            };

            Action act = () =>
                collection.Should().NotBeInAscendingOrder(o => o.Text, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection*a*b*c*not be ordered*Text*because*failure message*a*b*c*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_an_ordered_collection_are_not_ordered_ascending_using_the_specified_property_and_the_given_comparer_it_should_throw()
        {
            var collection = new[]
            {
                new { Text = "A", Numeric = 1 },
                new { Text = "b", Numeric = 2 },
                new { Text = "C", Numeric = 3 }
            };

            Action act = () => collection.Should().NotBeInAscendingOrder(
                o => o.Text, StringComparer.OrdinalIgnoreCase, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected collection*A*b*C*not be ordered*Text*because*failure message*A*b*C*");
        }

        [Fact]
        public void
            When_asserting_the_items_not_in_an_ascendingly_ordered_collection_are_not_ordered_ascending_using_the_specified_property_it_should_succeed()
        {
            var collection = new[]
            {
                new { Text = "b", Numeric = 3 },
                new { Text = "c", Numeric = 2 },
                new { Text = "a", Numeric = 1 }
            };

            collection.Should().NotBeInAscendingOrder(o => o.Numeric);
        }

        [Fact]
        public void
            When_asserting_the_items_not_in_an_ascendingly_ordered_collection_are_not_ordered_ascending_using_the_specified_property_and_the_given_comparer_it_should_succeed()
        {
            var collection = new[]
            {
                new { Text = "b", Numeric = 3 },
                new { Text = "c", Numeric = 2 },
                new { Text = "a", Numeric = 1 }
            };

            collection.Should().NotBeInAscendingOrder(o => o.Numeric, Comparer<int>.Default);
        }

        [Fact]
        public void When_strings_are_not_in_ascending_order_it_should_succeed()
        {
            string[] strings = ["beta", "alpha", "theta"];

            strings.Should().NotBeInAscendingOrder();
        }

        [Fact]
        public void When_strings_are_in_ascending_order_unexpectedly_it_should_throw()
        {
            string[] strings = ["alpha", "beta", "theta"];

            Action act = () => strings.Should().NotBeInAscendingOrder("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("Did not expect*ascending*because*failure message*but found*");
        }

        [Fact]
        public void When_strings_are_not_in_ascending_order_according_to_a_custom_comparer_it_should_succeed()
        {
            string[] strings = ["dennis", "roy", "barbara"];

            strings.Should().NotBeInAscendingOrder(new ByLastCharacterComparer());
        }

        [Fact]
        public void When_strings_are_unexpectedly_in_ascending_order_according_to_a_custom_comparer_it_should_throw()
        {
            string[] strings = ["dennis", "thomas", "roy"];

            Action act = () => strings.Should().NotBeInAscendingOrder(
                new ByLastCharacterComparer(), "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("Did not expect*ascending*because*failure message*but found*");
        }

        [Fact]
        public void When_strings_are_not_in_ascending_order_according_to_a_custom_lambda_it_should_succeed()
        {
            string[] strings = ["roy", "dennis", "thomas"];

            strings.Should().NotBeInAscendingOrder((sut, exp) => sut[^1].CompareTo(exp[^1]));
        }

        [Fact]
        public void When_strings_are_unexpectedly_in_ascending_order_according_to_a_custom_lambda_it_should_throw()
        {
            string[] strings = ["barbara", "dennis", "roy"];

            Action act = () => strings.Should().NotBeInAscendingOrder(
                (sut, exp) => sut[^1].CompareTo(exp[^1]), "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("Did not expect*ascending*because*failure message*but found*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_null_collection_are_not_ordered_using_the_specified_property_it_should_throw()
        {
            const IEnumerable<SomeClass> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().NotBeInAscendingOrder(o => o.Text);
            };

            act.Should().Throw<XunitException>().WithMessage("*Text*found*null*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_null_collection_are_not_ordered_using_the_given_comparer_it_should_throw()
        {
            const IEnumerable<SomeClass> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().NotBeInAscendingOrder(Comparer<SomeClass>.Default);
            };

            act.Should().Throw<XunitException>().WithMessage("*found*null*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_a_null_collection_are_not_ordered_using_the_specified_property_and_the_given_comparer_it_should_throw()
        {
            const IEnumerable<SomeClass> collection = null;

            Action act = () => collection.Should().NotBeInAscendingOrder(o => o.Text, StringComparer.OrdinalIgnoreCase);

            act.Should().Throw<XunitException>().WithMessage("*Text*found*null*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_collection_are_not_ordered_and_the_specified_property_is_null_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().NotBeInAscendingOrder((Expression<Func<SomeClass, string>>)null);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot assert collection ordering without specifying a property*propertyExpression*");
        }

        [Fact]
        public void When_asserting_the_items_in_a_collection_are_not_ordered_and_the_given_comparer_is_null_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().NotBeInAscendingOrder(comparer: null);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot assert collection ordering without specifying a comparer*comparer*");
        }

        [Fact]
        public void
            When_asserting_the_items_in_ay_collection_are_not_ordered_using_an_invalid_property_expression_it_should_throw()
        {
            IEnumerable<SomeClass> collection = [];

            Action act = () => collection.Should().NotBeInAscendingOrder(o => o.GetHashCode());

            act.Should().Throw<ArgumentException>().WithMessage("Expression*o.GetHashCode()*cannot be used to select a member*");
        }
    }
}
