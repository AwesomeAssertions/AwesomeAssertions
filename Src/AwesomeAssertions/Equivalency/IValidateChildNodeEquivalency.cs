namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Defines the ability to run a deep recursive equivalency assertion on the nested or child nodes of an object graph.
/// </summary>
public interface IValidateChildNodeEquivalency
{
    /// <summary>
    /// Runs a deep recursive equivalency assertion on the provided <paramref name="comparands"/>.
    /// </summary>
    void AssertEquivalencyOf(Comparands comparands, IEquivalencyValidationContext context);
}
