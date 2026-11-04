using System;
using AwesomeAssertions.Builders;
using Xunit;

namespace AwesomeAssertions.Specs.Builders;

public class DateBuilderSpecs
{
    public class Equal_to
    {
        [Theory]
        [InlineData(00, 00, DateTimeKind.Utc)]
        [InlineData(06, 15, DateTimeKind.Local)]
        [InlineData(06, 15, DateTimeKind.Unspecified)]
        [InlineData(06, 15, DateTimeKind.Utc)]
        public void DateTime_ignoring_time(int hours, int minutes, DateTimeKind kind)
        {
            // Arrange
            var model = new Model<DateTime>
            {
                Date = new DateTime(2017, 06, 11, hours, minutes, 00, kind),
            };

            // Act & Assert
            model.Should().BeEquivalentTo(new
            {
                Date = new DateBuilder(2017, 06, 11),
            });
        }

        [Theory]
        [InlineData(+0, 00)]
        [InlineData(+6, 00)]
        [InlineData(-6, 30)]
        public void DateTimeOfset_ignoring_time_and_offset(int hours, int minutes)
        {
            // Arrange
            var model = new Model<DateTimeOffset>
            {
                Date = new DateTimeOffset(new DateTime(2017, 06, 11), new TimeSpan(hours, minutes, 00)),
            };

            // Act & Assert
            model.Should().BeEquivalentTo(new
            {
                Date = new DateBuilder(2017, 06, 11),
            });
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void DateOnly()
        {
            // Arrange
            var model = new Model<DateOnly>
            {
                Date = new DateOnly(2017, 06, 11),
            };

            // Act & Assert
            model.Should().BeEquivalentTo(new
            {
                Date = new DateBuilder(2017, 06, 11),
            });
        }

        [Fact]
        public void IComparable_of_DateOnly()
        {
            // Arrange
            var model = new Model<CustomDate>
            {
                Date = new CustomDate(2017, 06, 11),
            };

            // Act & Assert
            model.Should().BeEquivalentTo(new
            {
                Date = new DateBuilder(2017, 06, 11),
            });
        }
#endif
    }

    private sealed record Model<TDate>
    {
        public required TDate Date { get; init; }
    }

#if NET8_0_OR_GREATER
    private sealed record CustomDate(int Year, int Month, int Day) : IEquatable<DateOnly>
    {
        public bool Equals(DateOnly other)
            => Year == other.Year
            && Month == other.Month
            && Day == other.Day;
    }
#endif
}
