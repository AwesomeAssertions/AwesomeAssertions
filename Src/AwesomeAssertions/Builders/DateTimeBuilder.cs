#nullable enable

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace AwesomeAssertions.Builders;

/// <summary>A builder for creating dates.</summary>
/// <param name="value">
/// The <see cref="DateTime"/> value.
/// </param>
public readonly struct DateTimeBuilder(DateTime value)
    : IEquatable<DateTimeBuilder>,
    IEquatable<IEquatable<DateTime>>,
    IEquatable<IEquatable<DateTimeOffset>>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly DateTime date = value;

    /// <summary>
    /// Gets a new <see cref="DateTime"/> with the kind set to local.
    /// </summary>
    public DateTime Local => new(date.Ticks, DateTimeKind.Local);

    /// <summary>
    /// Gets a new <see cref="DateTime"/> with the kind set to UTC.
    /// </summary>
    public DateTime Utc => new(date.Ticks, DateTimeKind.Utc);

    /// <summary>
    /// Gets a new <see cref="DateTime"/> with the kind set to unspecified.
    /// </summary>
    public DateTime Unspecified => new(date.Ticks, DateTimeKind.Unspecified);

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
        ? new(new DateTime(date.Ticks, DateTimeKind.Utc), TimeSpan.Zero)
        : new(new DateTime(date.Ticks, DateTimeKind.Unspecified), offset);

    /// <inheritdoc />
    [Pure]
    public override bool Equals(object? obj) => obj switch
    {
        DateTimeBuilder d => Equals(d),
        IEquatable<DateTime> d => Equals(d),
        IEquatable<DateTimeOffset> d => Equals(d),
        _ => false,
    };

    /// <inheritdoc />
    [Pure]
    public bool Equals(DateTimeBuilder other)
        => date == other.date;

    /// <inheritdoc />
    [Pure]
    public bool Equals(IEquatable<DateTime>? other)
        => other?.Equals(date) is true;

    /// <inheritdoc />
    [Pure]
    public bool Equals(IEquatable<DateTimeOffset>? other)
        => other?.Equals(AsOffset()) is true;

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode()
        => date.GetHashCode();

    /// <summary>
    /// Converts the <see cref="DateTimeBuilder"/> to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="builder">
    /// The builder to convert.
    /// </param>
    public static implicit operator DateTime(DateTimeBuilder builder)
        => builder.date;

    /// <summary>
    /// Converts the <see cref="DateTimeBuilder"/> to a <see cref="DateTimeBuilder"/>.
    /// </summary>
    /// <param name="builder">
    /// The builder to convert.
    /// </param>
    /// <remarks>
    /// Converts with an offset of zero, which is equivalent to UTC.
    /// </remarks>
    public static implicit operator DateTimeOffset(DateTimeBuilder builder)
        => builder.WithOffset(TimeSpan.Zero);

    /// <summary>Returns true if the two date time builders represent the same time.</summary>
    public static bool operator ==(DateTimeBuilder left, DateTimeBuilder right)
        => left.Equals(right);

    /// <summary>Returns true if the two date time builders represent a different time.</summary>
    public static bool operator !=(DateTimeBuilder left, DateTimeBuilder right)
        => !(left == right);
}
