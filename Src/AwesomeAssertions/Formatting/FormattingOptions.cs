using System.Collections.Generic;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Formatting;

public class FormattingOptions
{
    internal List<IValueFormatter> ScopedFormatters { get; set; } = [];

    /// <summary>
    /// Indicates whether the formatter should use line breaks when the <see cref="IValueFormatter"/> supports it.
    /// </summary>
    /// <remarks>
    /// This property is not thread-safe and should not be modified through <see cref="AssertionConfiguration"/> from within a unit test.
    /// See the <see href="https://awesomeassertions.org/extensibility/#thread-safety">docs</see> on how to safely use it.
    /// </remarks>
    public bool UseLineBreaks { get; set; }

    /// <summary>
    /// Determines the depth until which the library should try to render an object graph.
    /// </summary>
    /// <remarks>
    /// This property is not thread-safe and should not be modified through <see cref="AssertionConfiguration"/> from within a unit test.
    /// See the <see href="https://awesomeassertions.org/extensibility/#thread-safety">docs</see> on how to safely use it.
    /// </remarks>
    /// <value>
    /// A depth of 1 will only the display the members of the root object.
    /// </value>
    public int MaxDepth { get; set; } = 5;

    /// <summary>
    /// Sets the maximum number of lines of the failure message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Because of technical reasons, the actual output may be one or two lines longer.
    /// </para>
    /// <para>
    /// This property is not thread-safe and should not be modified through <see cref="AssertionConfiguration"/> from within a unit test.
    /// See the <see href="https://awesomeassertions.org/extensibility/#thread-safety">docs</see> on how to safely use it.
    /// </para>
    /// </remarks>
    public int MaxLines { get; set; } = 100;

    /// <summary>
    ///     Gets or sets the maximum number of items to display when the <see cref="IValueFormatter"/> supports it.
    /// </summary>
    /// <remarks>
    ///     The default value here is 0 (zero).
    ///     In this case the default value of the <see cref="IValueFormatter"/> is used.
    /// </remarks>
    public int MaxItems { get; set; }

    /// <summary>
    /// Sets the default number of characters shown when printing the difference of two strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The actual number of shown characters depends on the word-boundary-algorithm.<br />
    /// This algorithm searches for a word boundary (a blank) in the range from 5 characters previous and 10 characters after the
    /// <see cref="StringPrintLength"/>. If found it displays a full word, otherwise it falls back to the <see cref="StringPrintLength"/>.
    /// </para>
    /// <para>
    /// This property is not thread-safe and should not be modified through <see cref="AssertionConfiguration"/> from within a unit test.
    /// See the <see href="https://awesomeassertions.org/extensibility/#thread-safety">docs</see> on how to safely use it.
    /// </para>
    /// </remarks>
    public int StringPrintLength { get; set; } = 50;

    /// <summary>
    /// Removes a scoped formatter that was previously added through <see cref="FormattingOptions.AddFormatter"/>.
    /// </summary>
    /// <param name="formatter">A custom implementation of <see cref="IValueFormatter"/></param>
    public void RemoveFormatter(IValueFormatter formatter)
    {
        ScopedFormatters.Remove(formatter);
    }

    /// <summary>
    /// Ensures a scoped formatter is included in the chain, which is executed before the static custom formatters and the default formatters.
    /// This also lasts only for the current <see cref="AssertionScope"/> until disposal.
    /// </summary>
    /// <param name="formatter">A custom implementation of <see cref="IValueFormatter"/></param>
    public void AddFormatter(IValueFormatter formatter)
    {
        if (!ScopedFormatters.Contains(formatter))
        {
            ScopedFormatters.Insert(0, formatter);
        }
    }

    internal FormattingOptions Clone()
    {
        return new FormattingOptions
        {
            UseLineBreaks = UseLineBreaks,
            MaxDepth = MaxDepth,
            MaxLines = MaxLines,
            StringPrintLength = StringPrintLength,
            ScopedFormatters = [.. ScopedFormatters],
        };
    }
}
