#nullable enable
#pragma warning disable

using System;
using System.Diagnostics;
using AwesomeAssertions.Extensions;

namespace AwesomeAssertions.Builders;

public readonly struct DateTimeBuilder(DateTime value)
    : IEquatable<DateTimeBuilder>
    , IEquatable<DateTime>
    , IEquatable<DateTimeOffset>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly DateTime Value = value;

    public DateTimeBuilder Local => new(new(Value.Ticks, DateTimeKind.Local));

    public DateTimeBuilder Utc => new (new(Value.Ticks, DateTimeKind.Utc));

    public DateTimeBuilder Unspecified => new(new(Value.Ticks, DateTimeKind.Unspecified));

    public DateTimeOffset WithOffset(int hours, int minutes = 0)
        => WithOffset(new TimeSpan(hours, minutes, 00));

    public DateTimeOffset WithOffset(TimeSpan offset)
        => offset == TimeSpan.Zero
        ? new(new DateTime(Value.Ticks, DateTimeKind.Utc), TimeSpan.Zero)
        : new(new DateTime(Value.Ticks, DateTimeKind.Unspecified), offset);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj switch
    {
        DateTimeBuilder d => Equals(d),
        DateTime d => Equals(d),
        DateTimeOffset d => Equals(d),
        _ => false,
    };

    /// <inheritdoc />
    public bool Equals(DateTimeBuilder other)
        => Value == other.Value;

    /// <inheritdoc />
    public bool Equals(DateTime other)
        => Value == other;

    /// <inheritdoc />
    public bool Equals(DateTimeOffset other)
        => Value.WithOffset(TimeSpan.Zero) == other;

    public static implicit operator DateTime(DateTimeBuilder date)
        => date.Value;

    public static implicit operator DateTimeOffset(DateTimeBuilder date)
        => date.WithOffset(TimeSpan.Zero);
}
