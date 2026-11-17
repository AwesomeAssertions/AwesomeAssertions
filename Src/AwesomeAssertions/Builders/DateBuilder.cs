#nullable enable

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using AwesomeAssertions.Extensions;

namespace AwesomeAssertions.Builders;

/// <summary>A builder for creating dates.</summary>
/// <param name="year">
/// The year of the date.
/// </param>
/// <param name="month">
/// The month of the date.
/// </param>
/// <param name="day">
/// The day of the month of the date.
/// </param>
[StructLayout(LayoutKind.Auto)]
public readonly struct DateBuilder(int year, int month, int day)
    : IEquatable<DateBuilder>,
#if NET8_0_OR_GREATER
    IEquatable<IEquatable<DateOnly>>,
#endif
    IEquatable<DateTime>,
    IEquatable<DateTimeOffset>
{
    /// <inheritdoc cref="DateTime.Year" />
    public int Year { get; } = year;

    /// <inheritdoc cref="DateTime.Month" />
    public int Month { get; } = month;

    /// <inheritdoc cref="DateTime.Day" />
    public int Day { get; } = day;

    /// <summary>
    /// Gets a new <see cref="DateTimeBuilder"/> with the kind set to local.
    /// </summary>
    public DateTime Local
        => AsDateTime(DateTimeKind.Local);

    /// <summary>
    /// Gets a new <see cref="DateTimeBuilder"/> with the kind set to UTC.
    /// </summary>
    public DateTime Utc
        => AsDateTime();

    /// <summary>
    /// Gets a new <see cref="DateTimeBuilder"/> with the kind set to unspecified.
    /// </summary>
    public DateTime Unspecified
        => AsDateTime(DateTimeKind.Unspecified);

    /// <summary>Ats a time component.</summary>
    /// <param name="hours">
    /// The hours to add.
    /// </param>
    /// <param name="minutes">
    /// The minutes to add.
    /// </param>
    /// <param name="seconds">
    /// The seconds to add.
    /// </param>
    /// <param name="milliseconds">
    /// The milliseconds to add.
    /// </param>
    /// <param name="microseconds">
    /// The microseconds to add.
    /// </param>
    /// <param name="nanoseconds">
    /// The nanoseconds to add.
    /// </param>
    /// <returns>
    /// A <see cref="DateTimeBuilder"/> with the specified time component.
    /// </returns>
    [Pure]
    public DateTimeBuilder At(int hours, int minutes, int seconds = 0, int milliseconds = 0, int microseconds = 0, int nanoseconds = 0)
    {
        if (hours is < 0 or > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Valid values are between 0 and 23");
        }

        if (minutes is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "Valid values are between 0 and 59");
        }

        if (seconds is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "Valid values are between 0 and 59");
        }

        if (milliseconds is < 0 or > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Valid values are between 0 and 999");
        }

        if (microseconds is < 0 or > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(microseconds), "Valid values are between 0 and 999");
        }

        if (nanoseconds is < 0 or > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(nanoseconds), "Valid values are between 0 and 999");
        }

        var dt = new DateTime(Year, Month, Day, hours, minutes, seconds, milliseconds, DateTimeKind.Utc)
            + microseconds.Microseconds()
            + nanoseconds.Microseconds();

        return new(dt);
    }

    /// <summary>Ats a time component.</summary>    
    /// <param name="timeOfTheDay">
    /// The time of the day.
    /// </param>
    /// <returns>
    /// A <see cref="DateTimeBuilder"/> with the specified time component.
    /// </returns>
    [Pure]
    public DateTimeBuilder At(TimeSpan timeOfTheDay)
        => new(new DateTime(Year, Month, Day, 0, 0, 0, DateTimeKind.Utc) + timeOfTheDay);

    /// <summary>Creates a new <see cref="DateTimeOffset"/> with no offset.</summary>
    [Pure]
    public DateTimeOffset AsOffset()
        => WithOffset(TimeSpan.Zero);

    /// <summary>Creates a new <see cref="DateTimeOffset"/> with the specified offset.</summary>
    /// <param name="hours">
    /// The hours of the offset.
    /// </param>
    /// <param name="minutes">
    /// The minutes of the offset.
    /// </param>
    [Pure]
    public DateTimeOffset WithOffset(int hours, int minutes = 0)
        => WithOffset(new TimeSpan(hours, minutes, 00));

    /// <summary>Creates a new <see cref="DateTimeOffset"/> with the specified offset.</summary>
    /// <param name="offset">
    /// The offset.
    /// </param>
    [Pure]
    public DateTimeOffset WithOffset(TimeSpan offset)
        => offset == TimeSpan.Zero
        ? new(AsDateTime())
        : new(AsDateTime(DateTimeKind.Unspecified), offset);

    /// <inheritdoc />
    [Pure]
    public override bool Equals(object? obj) => obj switch
    {
        DateBuilder d => Equals(d),
        DateTime d => Equals(d),
        DateTimeOffset d => Equals(d),
#if NET6_0_OR_GREATER
        IEquatable<DateOnly> d => Equals(d),
#endif
        _ => false,
    };

    /// <inheritdoc />
    [Pure]
    public bool Equals(DateBuilder other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

    /// <inheritdoc />
    [Pure]
    public bool Equals(DateTime other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

    /// <inheritdoc />
    [Pure]
    public bool Equals(DateTimeOffset other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

#if NET8_0_OR_GREATER
    /// <inheritdoc />
    [Pure]
    public bool Equals(IEquatable<DateOnly>? other)
        => other?.Equals(this) is true;
#endif

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() => (Year * 10000) + (Month * 100) + Day;

    [Pure]
    private DateTime AsDateTime(DateTimeKind kind = DateTimeKind.Utc)
        => new(Year, Month, Day, 00, 00, 00, kind);

    /// <summary>Returns true if the two date builders represent the same time.</summary>
    public static bool operator ==(DateBuilder left, DateBuilder right) => left.Equals(right);

    /// <summary>Returns true if the two date builders represent the same time.</summary>
    public static bool operator !=(DateBuilder left, DateBuilder right) => !(left == right);

    /// <summary>
    /// Implicitly converts a <see cref="DateBuilder"/> to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="builder">
    /// The builder to convert.
    /// </param>
    /// <remarks>
    /// The time is set to 00:00:00 and the kind set to UTC.
    /// </remarks>
    public static implicit operator DateTime(DateBuilder builder)
        => builder.AsDateTime();

    /// <summary>
    /// Implicitly converts a <see cref="DateBuilder"/> to a <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="builder">
    /// The builder to convert.
    /// </param>
    /// <remarks>
    /// The time is set to 00:00:00 and the offset is set to 0:00.
    /// </remarks>
    public static implicit operator DateTimeOffset(DateBuilder builder)
        => builder.AsOffset();

#if NET8_0_OR_GREATER
    /// <summary>
    /// Implicitly converts a <see cref="DateBuilder"/> to a <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="builder">
    /// The builder to convert.
    /// </param>
    public static implicit operator DateOnly(DateBuilder builder)
        => new(builder.Year, builder.Month, builder.Day);
#endif
}
