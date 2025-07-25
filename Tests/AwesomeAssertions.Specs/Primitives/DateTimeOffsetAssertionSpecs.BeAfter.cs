using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

public partial class DateTimeOffsetAssertionSpecs
{
    public class BeAfter
    {
        [Fact]
        public void When_asserting_subject_datetimeoffset_is_after_earlier_expected_datetimeoffset_should_succeed()
        {
            // Arrange
            DateTimeOffset subject = new(new DateTime(2016, 06, 04), TimeSpan.Zero);
            DateTimeOffset expectation = new(new DateTime(2016, 06, 03), TimeSpan.Zero);

            // Act / Assert
            subject.Should().BeAfter(expectation);
        }

        [Fact]
        public void When_asserting_subject_datetimeoffset_is_after_later_expected_datetimeoffset_should_throw()
        {
            // Arrange
            DateTimeOffset subject = new(new DateTime(2016, 06, 04), TimeSpan.Zero);
            DateTimeOffset expectation = new(new DateTime(2016, 06, 05), TimeSpan.Zero);

            // Act
            Action act = () => subject.Should().BeAfter(expectation);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected subject to be after <2016-06-05 +0h>, but it was <2016-06-04 +0h>.");
        }

        [Fact]
        public void When_asserting_subject_datetimeoffset_is_after_the_same_expected_datetimeoffset_should_throw()
        {
            // Arrange
            DateTimeOffset subject = new(new DateTime(2016, 06, 04), TimeSpan.Zero);
            DateTimeOffset expectation = new(new DateTime(2016, 06, 04), TimeSpan.Zero);

            // Act
            Action act = () => subject.Should().BeAfter(expectation);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected subject to be after <2016-06-04 +0h>, but it was <2016-06-04 +0h>.");
        }
    }

    public class NotBeAfter
    {
        [Fact]
        public void When_asserting_subject_datetimeoffset_is_not_after_earlier_expected_datetimeoffset_should_throw()
        {
            // Arrange
            DateTimeOffset subject = new(new DateTime(2016, 06, 04), TimeSpan.Zero);
            DateTimeOffset expectation = new(new DateTime(2016, 06, 03), TimeSpan.Zero);

            // Act
            Action act = () => subject.Should().NotBeAfter(expectation);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected subject to be on or before <2016-06-03 +0h>, but it was <2016-06-04 +0h>.");
        }

        [Fact]
        public void When_asserting_subject_datetimeoffset_is_not_after_later_expected_datetimeoffset_should_succeed()
        {
            // Arrange
            DateTimeOffset subject = new(new DateTime(2016, 06, 04), TimeSpan.Zero);
            DateTimeOffset expectation = new(new DateTime(2016, 06, 05), TimeSpan.Zero);

            // Act / Assert
            subject.Should().NotBeAfter(expectation);
        }

        [Fact]
        public void When_asserting_subject_datetimeoffset_is_not_after_the_same_expected_datetimeoffset_should_succeed()
        {
            // Arrange
            DateTimeOffset subject = new(new DateTime(2016, 06, 04), TimeSpan.Zero);
            DateTimeOffset expectation = new(new DateTime(2016, 06, 04), TimeSpan.Zero);

            // Act / Assert
            subject.Should().NotBeAfter(expectation);
        }
    }
}
