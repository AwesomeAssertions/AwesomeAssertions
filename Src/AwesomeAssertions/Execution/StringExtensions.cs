using AwesomeAssertions.Formatting;

namespace AwesomeAssertions.Execution;

internal static class StringExtensions
{
    /// <summary>
    /// Can be used to wrap a string so that it is not formatted.
    /// </summary>
    /// <param name="value">The string to wrap</param>
    /// <returns>Object to pass to the fail message methods</returns>
    public static WithoutFormattingWrapper AsNonFormattable(this string value)
    {
        return new WithoutFormattingWrapper(value);
    }

    /// <summary>
    /// Can be used to wrap an object so that its plain ToString output is preserved during formatting.
    /// </summary>
    /// <param name="value">The object to wrap</param>
    /// <returns>Object to pass to the fail message methods</returns>
    public static WithoutFormattingWrapper AsNonFormattable(this object value)
    {
        return new WithoutFormattingWrapper(value?.ToString());
    }
}
