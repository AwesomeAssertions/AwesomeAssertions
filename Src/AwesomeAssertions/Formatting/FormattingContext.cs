namespace AwesomeAssertions.Formatting;

/// <summary>
/// Provides information about the current formatting action.
/// </summary>
public class FormattingContext
{
    /// <summary>
    /// Indicates whether the formatter should use line breaks when the <see cref="IValueFormatter"/> supports it.
    /// </summary>
    public bool UseLineBreaks { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of items to display when the <see cref="IValueFormatter"/> supports it.
    /// </summary>
    public int MaxItems { get; set; }
}
