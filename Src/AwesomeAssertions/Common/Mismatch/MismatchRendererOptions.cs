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
    /// The direction in which the subject and expected strings are aligned, which are aligned left by default.
    /// </summary>
    public Alignment Alignment { get; init; } = Alignment.Left;

    /// <summary>
    /// Gets the description of what was expected in the comparison.
    /// </summary>
    /// <remarks>
    /// Used to form part of the failure message.
    /// </remarks>
    public required string ExpectationDescription { get; init; }

    /// <summary>
    /// Gets the description of the mismatch location.
    /// </summary>
    /// <remarks>
    /// Used to form part of the failure message.
    /// </remarks>
    public required string MismatchLocationDescription { get; init; }

    /// <summary>
    /// Sets the default number of characters shown when printing the difference of two strings.
    /// </summary>
    public required int StringPrintLength { get; init; }
}
