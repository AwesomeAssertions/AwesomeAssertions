namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Determines how enums are compared during a structural equivalency assertion.
/// </summary>
public enum EnumEquivalencyHandling
{
    /// <summary>
    /// Enums are considered equivalent when their underlying values are equal.
    /// </summary>
    ByValue,

    /// <summary>
    /// Enums are considered equivalent when their names are equal.
    /// </summary>
    ByName
}
