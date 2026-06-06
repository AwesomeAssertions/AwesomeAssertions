using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Collections;

public partial class GenericCollectionAssertionOfStringSpecs
{
    public class BeNull
    {
        [Fact]
        public void When_collection_is_expected_to_be_null_and_it_is_it_should_not_throw()
        {
            // Arrange
            IEnumerable<string> someCollection = null;

            // Act / Assert
            someCollection.Should().BeNull();
        }

        [Fact]
        public void When_collection_is_expected_to_be_null_and_it_isnt_it_should_throw()
        {
            // Arrange
            IEnumerable<string> someCollection = new string[0];

            // Act
            Action act = () => someCollection.Should().BeNull("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected someCollection to be <null> because*failure message, but found {empty}.");
        }
    }

    public class NotBeNull
    {
        [Fact]
        public void When_collection_is_not_expected_to_be_null_and_it_is_it_should_throw()
        {
            // Arrange
            IEnumerable<string> someCollection = null;

            // Act
            Action act = () => someCollection.Should().NotBeNull("we want to test the {0} message", "failure");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected someCollection not to be <null> because*failure message.");
        }

        [Fact]
        public void When_collection_is_not_expected_to_be_null_and_it_isnt_it_should_not_throw()
        {
            // Arrange
            IEnumerable<string> someCollection = new string[0];

            // Act / Assert
            someCollection.Should().NotBeNull();
        }
    }
}
