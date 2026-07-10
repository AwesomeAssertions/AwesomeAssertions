namespace AwesomeAssertions.Formatting;

/// <summary>
/// Wrapper to tell the <see cref="Formatter"/> not to apply any value formatters on string <paramref name="value"/>.
/// </summary>
internal class WithoutFormattingWrapper(string value)
{
    public override string ToString() => value;
}
