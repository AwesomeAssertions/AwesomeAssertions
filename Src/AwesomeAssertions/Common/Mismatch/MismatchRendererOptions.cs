using System;

namespace AwesomeAssertions.Common.Mismatch;

/// <summary>
/// Represents configuration options for rendering string comparison mismatches in test assertions.
/// </summary>
internal class MismatchRendererOptions
{
    /// <summary>
    /// The subject string that is being compared.
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// The expected string that the subject is being compared against.
    /// </summary>
    public required string Expected { get; init; }

    /// <summary>
    /// The index position of the first mismatch in the subject string.
    /// </summary>
    public required int SubjectIndexOfMismatch { get; init; }

    /// <summary>
    /// The index position of the first mismatch in the expected string.
    /// </summary>
    public required int ExpectedIndexOfMismatch { get; init; }

    /// <summary>
    /// This flag enables alignment from the right side of the strings being compared. Otherwise, alignment is done from the left side.
    /// </summary>
    public bool AlignRight { get; init; }

    /// <summary>
    /// Gets the description of what was expected in the comparison.
    /// Used to form part of the failure message.
    /// </summary>
    public required string ExpectationDescription { get; init; }

    /// <summary>
    /// Gets or sets the strategy for truncating long strings when displaying mismatches.
    /// Defaults to <see cref="StandardTruncationStrategy"/>.
    /// </summary>
    public ITruncationStrategy TruncationStrategy { get; init; } = new StandardTruncationStrategy();

    /// <summary>
    /// Gets or sets a function that formats index positions into human-readable text.
    /// By default, formats as "at index {index}".
    /// </summary>
    public Func<int, string> IndexFormatter { get; init; } = index => $"at index {index}";
}
