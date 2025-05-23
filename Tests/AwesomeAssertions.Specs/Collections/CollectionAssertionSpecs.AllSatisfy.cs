using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

/// <content>
/// The AllSatisfy specs.
/// </content>
public partial class CollectionAssertionSpecs
{
    public class AllSatisfy
    {
        [Fact]
        public void A_null_inspector_should_throw()
        {
            // Arrange
            IEnumerable<int> collection = [1, 2];

            // Act
            Action act = () => collection.Should().AllSatisfy(null);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Cannot verify against a <null> inspector*");
        }

        [Fact]
        public void A_null_collection_should_throw()
        {
            // Arrange
            IEnumerable<int> collection = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                collection.Should().AllSatisfy(x => x.Should().Be(1), "because we want to test the failure {0}", "message");
            };

            // Assert
            act.Should()
                .Throw<XunitException>()
                .WithMessage(
                    "Expected collection to contain only items satisfying the inspector because we want to test the failure message, but collection is <null>.");
        }

        [Fact]
        public void An_empty_collection_should_succeed()
        {
            // Arrange
            IEnumerable<int> collection = [];

            // Act / Assert
            collection.Should().AllSatisfy(x => x.Should().Be(1));
        }

        [Fact]
        public void All_items_satisfying_inspector_should_succeed()
        {
            // Arrange
            Customer[] collection = [new Customer { Age = 21, Name = "John" }, new Customer { Age = 21, Name = "Jane" }];

            // Act / Assert
            collection.Should().AllSatisfy(x => x.Age.Should().Be(21));
        }

        [Fact]
        public void Any_items_not_satisfying_inspector_should_throw()
        {
            // Arrange
            CustomerWithItems[] customers =
            [
                new CustomerWithItems { Age = 21, Items = [1, 2] },
                new CustomerWithItems { Age = 22, Items = [3] }
            ];

            // Act
            Action act = () => customers.Should()
                .AllSatisfy(
                    customer =>
                    {
                        customer.Age.Should().BeLessThan(21);

                        customer.Items.Should()
                            .AllSatisfy(item => item.Should().Be(3));
                    },
                    "because we want to test {0}",
                    "nested assertions");

            // Assert
            act.Should()
                .Throw<XunitException>()
                .WithMessage("""
                    Expected customers to contain only items satisfying the inspector because we want to test nested assertions:
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
            // Arrange
            byte[][] subject = [[1]];

            // Act
            Action act = () => subject.Should().AllSatisfy(e => e.Should().BeEquivalentTo(new byte[] { 2, 3, 4 }));

            // Assert
            act.Should().NotThrow<FormatException>();
        }
    }
}
