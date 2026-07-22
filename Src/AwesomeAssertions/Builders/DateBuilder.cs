#nullable enable
#pragma warning disable

using System;
using System.Runtime.InteropServices;

namespace AwesomeAssertions.Builders;

[StructLayout(LayoutKind.Auto)]
public readonly struct DateBuilder(int year, int month, int day)
    : IEquatable<DateBuilder>
#if NET6_0_OR_GREATER
    , IEquatable<DateOnly>
#endif
    , IEquatable<DateTime>
    , IEquatable<DateTimeOffset>
{
    /// <inheritdoc cref="DateTime.Year" />
    public int Year { get; } = year;

    /// <inheritdoc cref="DateTime.Month" />
    public int Month { get; } = month;

    /// <inheritdoc cref="DateTime.Day" />
    public int Day { get; } = day;

    public DateTimeBuilder At(int hours, int minutes, int seconds = 0)
        => new(new DateTime(Year, Month, Day, hours, minutes, seconds, DateTimeKind.Utc));

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj switch
    {
        DateBuilder d => Equals(d),
        DateTime d => Equals(d),
        DateTimeOffset d => Equals(d),
#if NET6_0_OR_GREATER
        DateOnly d => Equals(d),
#endif
        _ => false,
    };

    /// <inheritdoc />
    public bool Equals(DateBuilder other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

    /// <inheritdoc />
    public bool Equals(DateTime other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

    /// <inheritdoc />
    public bool Equals(DateTimeOffset other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

    public static implicit operator DateTime(DateBuilder date)
        => new(date.Year, date.Month, date.Day, 00, 00, 00, DateTimeKind.Utc);

    public static implicit operator DateTimeOffset(DateBuilder date)
        => new(date.Year, date.Month, date.Day, 00, 00, 00, TimeSpan.Zero);

#if NET6_0_OR_GREATER
    /// <inheritdoc />
    public bool Equals(DateOnly other)
        => Year == other.Year
        && Month == other.Month
        && Day == other.Day;

    public static implicit operator DateOnly(DateBuilder date)
        => new(date.Year, date.Month, date.Day);
#endif
}
