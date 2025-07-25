using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

public partial class DateTimeAssertionSpecs
{
    public class HaveDay
    {
        [Fact]
        public void When_asserting_subject_datetime_should_have_day_with_the_same_value_it_should_succeed()
        {
            // Arrange
            DateTime subject = new(2009, 12, 31);
            int expectation = 31;

            // Act / Assert
            subject.Should().HaveDay(expectation);
        }

        [Fact]
        public void When_asserting_subject_datetime_should_have_day_with_a_different_value_it_should_throw()
        {
            // Arrange
            DateTime subject = new(2009, 12, 31);
            int expectation = 30;

            // Act
            Action act = () => subject.Should().HaveDay(expectation);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected the day part of subject to be 30, but found 31.");
        }

        [Fact]
        public void When_asserting_subject_null_datetime_should_have_day_should_throw()
        {
            // Arrange
            DateTime? subject = null;
            int expectation = 22;

            // Act
            Action act = () => subject.Should().HaveDay(expectation);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected the day part of subject to be 22, but found a <null> DateTime.");
        }
    }

    public class NotHaveDay
    {
        [Fact]
        public void When_asserting_subject_datetime_should_not_have_day_with_the_same_value_it_should_throw()
        {
            // Arrange
            DateTime subject = new(2009, 12, 31);
            int expectation = 31;

            // Act
            Action act = () => subject.Should().NotHaveDay(expectation);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect the day part of subject to be 31, but it was.");
        }

        [Fact]
        public void When_asserting_subject_datetime_should_not_have_day_with_a_different_value_it_should_succeed()
        {
            // Arrange
            DateTime subject = new(2009, 12, 31);
            int expectation = 30;

            // Act / Assert
            subject.Should().NotHaveDay(expectation);
        }

        [Fact]
        public void When_asserting_subject_null_datetime_should_not_have_day_should_throw()
        {
            // Arrange
            DateTime? subject = null;
            int expectation = 22;

            // Act
            Action act = () => subject.Should().NotHaveDay(expectation);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect the day part of subject to be 22, but found a <null> DateTime.");
        }
    }
}
