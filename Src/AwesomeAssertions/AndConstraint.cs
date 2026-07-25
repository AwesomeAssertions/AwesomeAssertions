using System.Diagnostics;

namespace AwesomeAssertions;

/// <summary>
/// Provides access to the parent assertions object, allowing another assertion to be chained onto the same subject
/// through its <see cref="And"/> property.
/// </summary>
/// <typeparam name="TParent">The type of the parent assertions object.</typeparam>
[DebuggerNonUserCode]
public class AndConstraint<TParent>
{
    /// <summary>
    /// Gets the parent assertions object, allowing another assertion to be chained onto the same subject.
    /// </summary>
    public TParent And { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndConstraint{T}"/> class.
    /// </summary>
    public AndConstraint(TParent parent)
    {
        And = parent;
    }
}
