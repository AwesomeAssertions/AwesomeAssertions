using System;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchRendererOptions
{
    public required string Subject { get; init; }

    public required string Expected { get; init; }

    public required int SubjectIndexOfMismatch { get; init; }

    public required int ExpectedIndexOfMismatch { get; init; }

    public bool AlignRight { get; init; }

    public required string ExpectationDescription { get; init; }

    public ITruncationStrategy TruncationStrategy { get; init; } = new StandardTruncationStrategy();

    public Func<int, string> IndexFormatter { get; init; } = index => $"at index {index}";
}
