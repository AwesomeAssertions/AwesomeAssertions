using AwesomeAssertions.Common;
using AwesomeAssertions.Formatting;

namespace AwesomeAssertions.Configuration;

/// <summary>
/// Provides access to the formatting defaults used by all assertions.
/// </summary>
public class GlobalFormattingOptions : FormattingOptions
{
    private string valueFormatterAssembly;

    /// <summary>
    /// Gets or sets the name of the assembly that is scanned for custom value formatters.
    /// Setting this property changes <see cref="ValueFormatterDetectionMode"/> to <see cref="AwesomeAssertions.Common.ValueFormatterDetectionMode.Specific"/>.
    /// </summary>
    public string ValueFormatterAssembly
    {
        get => valueFormatterAssembly;
        set
        {
            valueFormatterAssembly = value;
            ValueFormatterDetectionMode = ValueFormatterDetectionMode.Specific;
        }
    }

    /// <summary>
    /// Gets or sets the mode that determines how custom value formatters are detected.
    /// </summary>
    public ValueFormatterDetectionMode ValueFormatterDetectionMode { get; set; }

    internal new GlobalFormattingOptions Clone()
    {
        return new GlobalFormattingOptions
        {
            UseLineBreaks = UseLineBreaks,
            MaxDepth = MaxDepth,
            MaxLines = MaxLines,
            MaxItems = MaxItems,
            StringPrintLength = StringPrintLength,
            ScopedFormatters = [.. ScopedFormatters],
            ValueFormatterAssembly = ValueFormatterAssembly,
            ValueFormatterDetectionMode = ValueFormatterDetectionMode
        };
    }
}
