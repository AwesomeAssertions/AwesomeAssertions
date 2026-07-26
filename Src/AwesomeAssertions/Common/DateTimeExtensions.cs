using System;

namespace AwesomeAssertions.Common;

/// <summary>
/// Provides extension methods for converting <see cref="DateTime"/> values.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts an existing <see cref="DateTime"/> to a <see cref="DateTimeOffset"/> but normalizes the <see cref="DateTimeKind"/>
    /// so that comparisons of converted <see cref="DateTime"/> instances retain the UTC/local agnostic behavior.
    /// </summary>
    public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
    {
        return dateTime.ToDateTimeOffset(TimeSpan.Zero);
    }

    /// <summary>
    /// Converts an existing <see cref="DateTime"/> to a <see cref="DateTimeOffset"/> using the specified <paramref name="offset"/>,
    /// while normalizing the <see cref="DateTimeKind"/> so that comparisons of converted <see cref="DateTime"/> instances retain the
    /// UTC/local agnostic behavior.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> to convert.</param>
    /// <param name="offset">The offset from Coordinated Universal Time (UTC) to associate with the result.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the same point in time with the specified <paramref name="offset"/>.</returns>
    public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), offset);
    }
}
