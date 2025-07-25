using System;
using AwesomeAssertions.Common;
using Xunit;

namespace AwesomeAssertions.Specs.Primitives;

public partial class DateTimeOffsetAssertionSpecs
{
    public class ChainingConstraint
    {
        [Fact]
        public void Should_support_chaining_constraints_with_and()
        {
            // Arrange
            DateTimeOffset yesterday = new DateTime(2016, 06, 03).ToDateTimeOffset();
            DateTimeOffset? nullableDateTime = new DateTime(2016, 06, 04).ToDateTimeOffset();

            // Act / Assert
            nullableDateTime.Should()
                .HaveValue()
                .And
                .BeAfter(yesterday);
        }
    }

    public class Miscellaneous
    {
        [Fact]
        public void Should_throw_a_helpful_error_when_accidentally_using_equals()
        {
            // Arrange
            DateTimeOffset someDateTimeOffset = new(2022, 9, 25, 13, 48, 42, 0, TimeSpan.Zero);

            // Act
            var action = () => someDateTimeOffset.Should().Equals(null);

            // Assert
            action.Should().Throw<NotSupportedException>()
                .WithMessage("Equals is not part of Awesome Assertions. Did you mean Be() instead?");
        }
    }
}
