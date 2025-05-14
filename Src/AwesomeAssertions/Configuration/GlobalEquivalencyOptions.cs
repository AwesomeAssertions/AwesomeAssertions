using System;
using AwesomeAssertions.Common;
using AwesomeAssertions.Equivalency;
using JetBrains.Annotations;

namespace AwesomeAssertions.Configuration;

public class GlobalEquivalencyOptions
{
    private EquivalencyOptions defaults = new();

    /// <summary>
    /// Represents a mutable plan consisting of steps that are executed while asserting a (collection of) object(s)
    /// is structurally equivalent to another (collection of) object(s).
    /// </summary>
    /// <remarks>
    /// Members on this property are not thread-safe and should not be invoked from within a unit test.
    /// See the <see href="https://awesomeassertions.org/extensibility/#thread-safety">docs</see> on how to safely use it.
    /// </remarks>
    public EquivalencyPlan Plan { get; } = new();

    /// <summary>
    /// Allows configuring the defaults used during a structural equivalency assertion.
    /// </summary>
    /// <remarks>
    /// This method is not thread-safe and should not be invoked from within a unit test.
    /// See the <see href="https://awesomeassertions.org/extensibility/#thread-safety">docs</see> on how to safely use it.
    /// </remarks>
    /// <param name="configureOptions">
    /// An action that is used to configure the defaults.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="configureOptions"/> is <see langword="null"/>.</exception>
    public void Modify(Func<EquivalencyOptions, EquivalencyOptions> configureOptions)
    {
        Guard.ThrowIfArgumentIsNull(configureOptions);

        defaults = configureOptions(defaults);
    }

    /// <summary>
    /// Creates a clone of the default options and allows the caller to modify them.
    /// </summary>
    /// <remarks>
    /// Can be used by external packages like AwesomeAssertions.DataSets to create a copy of the default equivalency options.
    /// </remarks>
    [PublicAPI]
    public EquivalencyOptions<T> CloneDefaults<T>()
    {
        return new EquivalencyOptions<T>(defaults);
    }
}
