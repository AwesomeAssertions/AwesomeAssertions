using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class AllBeOfType
    {
        [Fact]
        public void When_the_types_in_a_collection_is_matched_against_a_null_type_exactly_it_should_throw()
        {
            int[] collection = [];

            Action act = () => collection.Should().AllBeOfType(null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("expectedType");
        }

        [Fact]
        public void All_items_in_an_empty_collection_are_of_a_generic_type()
        {
            int[] collection = [];

            collection.Should().AllBeOfType<int>();
        }

        [Fact]
        public void All_items_in_an_empty_collection_are_of_a_type()
        {
            int[] collection = [];

            collection.Should().AllBeOfType(typeof(int));
        }

        [Fact]
        public void When_collection_is_null_then_all_be_of_type_should_fail()
        {
            IEnumerable<object> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().AllBeOfType(typeof(object), "we want to test the {0} message", "failure");
            };

            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected type to be object because we want to test the failure message, but found collection is <null>.");
        }

        [Fact]
        public void When_all_of_the_types_in_a_collection_match_expected_type_exactly_it_should_succeed()
        {
            int[] collection = [1, 2, 3];

            collection.Should().AllBeOfType(typeof(int));
        }

        [Fact]
        public void When_all_of_the_types_in_a_collection_match_expected_generic_type_exactly_it_should_succeed()
        {
            int[] collection = [1, 2, 3];

            collection.Should().AllBeOfType<int>();
        }

        [Fact]
        public void When_matching_a_collection_against_an_exact_type_it_should_return_the_casted_items()
        {
            int[] collection = [1, 2, 3];

            collection.Should().AllBeOfType<int>()
                .Which.Should().Equal(1, 2, 3);
        }

        [Fact]
        public void When_one_of_the_types_does_not_match_exactly_it_should_throw_with_a_clear_explanation()
        {
            var collection = new object[] { new Exception(), new ArgumentException("foo") };

            Action act = () => collection.Should().AllBeOfType(typeof(Exception), "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be System.Exception because we want to test the failure message,"
                + " but found {System.Exception, System.ArgumentException}.");
        }

        [Fact]
        public void When_one_of_the_types_does_not_match_exactly_the_generic_type_it_should_throw_with_a_clear_explanation()
        {
            var collection = new object[] { new Exception(), new ArgumentException("foo") };

            Action act = () => collection.Should().AllBeOfType<Exception>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be System.Exception because we want to test the failure message,"
                + " but found {System.Exception, System.ArgumentException}.");
        }

        [Fact]
        public void When_one_of_the_elements_is_null_for_an_exact_match_it_should_throw_with_a_clear_explanation()
        {
            var collection = new object[] { 1, null, 3 };

            Action act = () => collection.Should().AllBeOfType<int>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be int because we want to test the failure message, but found a null element.");
        }

        [Fact]
        public void When_collection_of_types_match_expected_type_exactly_it_should_succeed()
        {
            Type[] collection = [typeof(int), typeof(int), typeof(int)];

            collection.Should().AllBeOfType<int>();
        }

        [Fact]
        public void When_collection_of_types_and_objects_match_type_exactly_it_should_succeed()
        {
            var collection = new object[] { typeof(ArgumentException), new ArgumentException("foo") };

            collection.Should().AllBeOfType<ArgumentException>();
        }

        [Fact]
        public void When_collection_of_types_and_objects_do_not_match_type_exactly_it_should_throw()
        {
            var collection = new object[] { typeof(Exception), new ArgumentException("foo") };

            Action act = () => collection.Should().AllBeOfType<ArgumentException>();

            act.Should().Throw<XunitException>().WithMessage(
                "Expected type to be System.ArgumentException, but found {System.Exception, System.ArgumentException}.");
        }

        [Fact]
        public void When_collection_is_null_then_all_be_of_typeOfT_should_fail()
        {
            IEnumerable<object> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().AllBeOfType<object>("we want to test the failure {0}", "message");
            };

            act.Should().Throw<XunitException>()
                .WithMessage("Expected type to be object *failure message*, but found collection is <null>.");
        }
    }
}
