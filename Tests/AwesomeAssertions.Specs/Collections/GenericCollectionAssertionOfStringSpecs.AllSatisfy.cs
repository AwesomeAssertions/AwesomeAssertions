using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class GenericCollectionAssertionOfStringSpecs
{
    public class AllSatisfy
    {
        [Fact]
        public void All_items_satisfying_inspector_should_succeed()
        {
            // Arrange
            string[] collection = ["John", "John"];

            // Act / Assert
            collection.Should().AllSatisfy(value => value.Should().Be("John"));
        }

        [Fact]
        public void Any_items_not_satisfying_inspector_should_throw()
        {
            // Arrange
            string[] collection = ["Jack", "Jessica"];

            // Act
            Action act = () => collection.Should().AllSatisfy(
                value => value.Should().Be("John"),
                "we want to test the {0} message", "failure");

            // Assert
            act.Should()
                .Throw<XunitException>()
                .WithMessage(
                    "Expected collection to contain only items satisfying the inspector because*failure message:"
                    + "*Jack*John"
                    + "*Jessica*John*");
        }
    }
}
