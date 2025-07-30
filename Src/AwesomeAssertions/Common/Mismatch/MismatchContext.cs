using System.Collections.Generic;
using JetBrains.Annotations;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchContext
{
    public required string Subject { get; init; }

    public required string Expected { get; init; }

    public required int IndexOfMismatch { get; init; }

    public bool IsStringEndStrategy { get; init; }

    public required IEqualityComparer<string> Comparer { get; init; }

    public required string ExpectationDescription { get; init; }

    [CanBeNull]
    private MismatchSpan subjectVisibleSpan;

    public MismatchSpan SubjectVisibleSpan => subjectVisibleSpan ??= CreateSpan(Subject, Expected);

    [CanBeNull]
    private MismatchSpan expectedVisibleSpan;

    public MismatchSpan ExpectedVisibleSpan => expectedVisibleSpan ??= CreateSpan(Expected, Subject);

    private MismatchSpan CreateSpan(string self, string other)
    {
        var mismatchIndex = IsStringEndStrategy
            ? self.IndexOfLastMismatch(other, Comparer)
            : self.IndexOfFirstMismatchBidirectional(other, Comparer);
        return CreateSpan(self, mismatchIndex);
    }

    private MismatchSpan CreateSpan(string text, int mismatchPoint)
    {
        var span = MismatchSpan.Create(text, mismatchPoint);

        if (IsStringEndStrategy)
        {
            span.TruncateReverse();
        }
        else
        {
            span.Truncate();
        }

        span.EscapeNewLines();
        return span;
    }
}
