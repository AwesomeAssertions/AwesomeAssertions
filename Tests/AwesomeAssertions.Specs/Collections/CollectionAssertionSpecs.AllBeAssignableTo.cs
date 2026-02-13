using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class AllBeAssignableTo
    {
        [Fact]
        public void When_the_types_in_a_collection_is_matched_against_a_null_type_it_should_throw()
        {
            int[] collection = [];

            Action act = () => collection.Should().AllBeAssignableTo(null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("expectedType");
        }

        [Fact]
        public void All_items_in_an_empty_collection_are_assignable_to_a_generic_type()
        {
            int[] collection = [];

            collection.Should().AllBeAssignableTo<int>();
        }

        [Fact]
        public void When_collection_is_null_then_all_be_assignable_to_should_fail()
        {
            IEnumerable<object> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().AllBeAssignableTo(typeof(object), "we want to test the failure {0}", "message");
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected type to be object *failure message*, but found collection is <null>.");
        }

        [Fact]
        public void All_items_in_an_empty_collection_are_assignable_to_a_type()
        {
            int[] collection = [];

            collection.Should().AllBeAssignableTo(typeof(int));
        }

        [Fact]
        public void When_all_of_the_types_in_a_collection_match_expected_type_it_should_succeed()
        {
            int[] collection = [1, 2, 3];

            collection.Should().AllBeAssignableTo(typeof(int));
        }

        [Fact]
        public void When_all_of_the_types_in_a_collection_match_expected_generic_type_it_should_succeed()
        {
            int[] collection = [1, 2, 3];

            collection.Should().AllBeAssignableTo<int>();
        }

        [Fact]
        public void When_matching_a_collection_against_a_type_it_should_return_the_casted_items()
        {
            int[] collection = [1, 2, 3];

            collection.Should().AllBeAssignableTo<int>().Which.Should().Equal(1, 2, 3);
        }

        [Fact]
        public void When_all_of_the_types_in_a_collection_match_the_type_or_subtype_it_should_succeed()
        {
            var collection = new object[] { new Exception(), new ArgumentException("foo") };

            collection.Should().AllBeAssignableTo<Exception>();
        }

        [Fact]
        public void When_one_of_the_types_does_not_match_it_should_throw_with_a_clear_explanation()
        {
            var collection = new object[] { 1, "2", 3 };

            Action act = () => collection.Should().AllBeAssignableTo(typeof(int), "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be int because we want to test the failure message, but found {int, string, int}.");
        }

        [Fact]
        public void When_one_of_the_types_does_not_match_the_generic_type_it_should_throw_with_a_clear_explanation()
        {
            var collection = new object[] { 1, "2", 3 };

            Action act = () => collection.Should().AllBeAssignableTo<int>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be int because we want to test the failure message, but found {int, string, int}.");
        }

        [Fact]
        public void When_one_of_the_elements_is_null_it_should_throw_with_a_clear_explanation()
        {
            var collection = new object[] { 1, null, 3 };

            Action act = () => collection.Should().AllBeAssignableTo<int>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be int because we want to test the failure message, but found a null element.");
        }

        [Fact]
        public void When_collection_is_of_matching_types_it_should_succeed()
        {
            Type[] collection = [typeof(Exception), typeof(ArgumentException)];

            collection.Should().AllBeAssignableTo<Exception>();
        }

        [Fact]
        public void When_collection_of_types_contains_one_type_that_does_not_match_it_should_throw_with_a_clear_explanation()
        {
            Type[] collection = [typeof(int), typeof(string), typeof(int)];

            Action act = () => collection.Should().AllBeAssignableTo<int>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be int because we want to test the failure message, but found {int, string, int}.");
        }

        [Fact]
        public void When_collection_of_types_and_objects_are_all_of_matching_types_it_should_succeed()
        {
            var collection = new object[] { typeof(int), 2, typeof(int) };

            collection.Should().AllBeAssignableTo<int>();
        }

        [Fact]
        public void When_collection_of_different_types_and_objects_are_all_assignable_to_type_it_should_succeed()
        {
            var collection = new object[] { typeof(Exception), new ArgumentException("foo") };

            collection.Should().AllBeAssignableTo<Exception>();
        }

        [Fact]
        public void When_collection_is_null_then_all_be_assignable_toOfT_should_fail()
        {
            IEnumerable<object> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().AllBeAssignableTo<object>("we want to test the failure {0}", "message");
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected type to be object *failure message*, but found collection is <null>.");
        }
    }
}
