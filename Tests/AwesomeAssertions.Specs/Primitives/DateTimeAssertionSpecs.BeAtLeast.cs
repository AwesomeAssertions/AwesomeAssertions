using System;
using AwesomeAssertions.Extensions;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

public partial class DateTimeAssertionSpecs
{
    public class BeAtLeast
    {
        [Fact]
        public void When_date_is_not_at_least_one_day_before_another_it_should_throw()
        {
            // Arrange
            var target = new DateTime(2009, 10, 2);
            DateTime subject = target - 23.Hours();

            // Act
            Action act = () => subject.Should().BeAtLeast(TimeSpan.FromDays(1)).Before(target, "we like {0}", "that");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected subject <2009-10-01 01:00:00> to be at least 1d before <2009-10-02> because we like that, but it is behind by 23h.");
        }

        [Fact]
        public void When_date_is_at_least_one_day_before_another_it_should_not_throw()
        {
            // Arrange
            var target = new DateTime(2009, 10, 2);
            DateTime subject = target - 24.Hours();

            // Act / Assert
            subject.Should().BeAtLeast(TimeSpan.FromDays(1)).Before(target);
        }

        [Theory]
        [InlineData(30, 20)] // edge case
        [InlineData(30, 15)]
        public void When_asserting_subject_be_at_least_10_seconds_after_target_but_subject_is_before_target_it_should_throw(
            int targetSeconds, int subjectSeconds)
        {
            // Arrange
            var expectation = 1.January(0001).At(0, 0, targetSeconds);
            var subject = 1.January(0001).At(0, 0, subjectSeconds);

            // Act
            Action action = () => subject.Should().BeAtLeast(10.Seconds()).After(expectation);

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage(
                    $"Expected subject <00:00:{subjectSeconds}> to be at least 10s after <00:00:30>, but it is behind by {Math.Abs(subjectSeconds - targetSeconds)}s.");
        }

        [Theory]
        [InlineData(30, 40)] // edge case
        [InlineData(30, 45)]
        public void When_asserting_subject_be_at_least_10_seconds_before_target_but_subject_is_after_target_it_should_throw(
            int targetSeconds, int subjectSeconds)
        {
            // Arrange
            var expectation = 1.January(0001).At(0, 0, targetSeconds);
            var subject = 1.January(0001).At(0, 0, subjectSeconds);

            // Act
            Action action = () => subject.Should().BeAtLeast(10.Seconds()).Before(expectation);

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage(
                    $"Expected subject <00:00:{subjectSeconds}> to be at least 10s before <00:00:30>, but it is ahead by {Math.Abs(subjectSeconds - targetSeconds)}s.");
        }
    }
}
