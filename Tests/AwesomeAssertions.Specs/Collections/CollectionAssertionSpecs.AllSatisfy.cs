using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class AllSatisfy
    {
        [Fact]
        public void A_null_inspector_should_throw()
        {
            IEnumerable<int> collection = [1, 2];

            Action act = () => collection.Should().AllSatisfy(null);

            act.Should().Throw<ArgumentException>().WithMessage("Cannot verify against a <null> inspector*");
        }

        [Fact]
        public void A_null_collection_should_throw()
        {
            IEnumerable<int> collection = null;

            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().AllSatisfy(x => x.Should().Be(1), "we want to test the {0} message", "failure");
            };

            act.Should()
                .Throw<XunitException>()
                .WithMessage(
                    "Expected collection to contain only items satisfying the inspector"
                    + " because we want to test the failure message, but collection is <null>.");
        }

        [Fact]
        public void An_empty_collection_should_succeed()
        {
            IEnumerable<int> collection = [];

            collection.Should().AllSatisfy(x => x.Should().Be(1));
        }

        [Fact]
        public void All_items_satisfying_inspector_should_succeed()
        {
            Customer[] collection = [new() { Age = 21, Name = "John" }, new() { Age = 21, Name = "Jane" }];

            collection.Should().AllSatisfy(x => x.Age.Should().Be(21));
        }

        [Fact]
        public void Any_items_not_satisfying_inspector_should_throw()
        {
            CustomerWithItems[] customers =
            [
                new() { Age = 21, Items = [1, 2] },
                new() { Age = 22, Items = [3] }
            ];

            Action act = () => customers.Should()
                .AllSatisfy(
                    customer =>
                    {
                        customer.Age.Should().BeLessThan(21);

                        customer.Items.Should()
                            .AllSatisfy(item => item.Should().Be(3));
                    },
                    "we want to test the {0} message",
                    "failure");

            act.Should()
                .Throw<XunitException>()
                .WithMessage("""
                             Expected customers to contain only items satisfying the inspector because we want to test the failure message:
                             *At index 0:
                             *Expected customer.Age to be less than 21, but found 21
                             *Expected customer.Items to contain only items satisfying the inspector:
                             *At index 0:
                             *Expected item to be 3, but found 1
                             *At index 1:
                             *Expected item to be 3, but found 2
                             *At index 1:
                             *Expected customer.Age to be less than 21, but found 22 (difference of 1)
                             """);
        }

        [Fact]
        public void Inspector_message_that_is_not_reformatable_should_not_throw()
        {
            byte[][] subject = [[1]];

            Action act = () => subject.Should().AllSatisfy(e => e.Should().BeEquivalentTo(new byte[] { 2, 3, 4 }));

            act.Should().NotThrow<FormatException>();
        }
    }
}
