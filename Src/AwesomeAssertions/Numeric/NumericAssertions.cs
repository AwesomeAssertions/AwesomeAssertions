using System;
using System.Diagnostics;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that an <see cref="IComparable{T}"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class NumericAssertions<T> : NumericAssertions<T, NumericAssertions<T>>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericAssertions{T}"/> class.
    /// </summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    public NumericAssertions(T value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }
}

/// <summary>
/// Contains a number of methods to assert that an <see cref="IComparable{T}"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
public class NumericAssertions<T, TAssertions> : NumericAssertionsBase<T, T, TAssertions>
    where T : struct, IComparable<T>
    where TAssertions : NumericAssertions<T, TAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericAssertions{T, TAssertions}"/> class.
    /// </summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    public NumericAssertions(T value, AssertionChain assertionChain)
        : base(assertionChain)
    {
        Subject = value;
    }

    /// <summary>
    /// Gets the object whose value is being asserted.
    /// </summary>
    public override T Subject { get; }
}
