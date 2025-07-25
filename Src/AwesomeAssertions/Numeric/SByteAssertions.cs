using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that an <see cref="sbyte"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
internal class SByteAssertions : NumericAssertions<sbyte>
{
    internal SByteAssertions(sbyte value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    private protected override string CalculateDifferenceForFailureMessage(sbyte subject, sbyte expected)
    {
        int difference = subject - expected;
        return difference != 0 ? difference.ToString(CultureInfo.InvariantCulture) : null;
    }
}
