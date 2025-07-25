using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that a <see cref="ushort"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
internal class UInt16Assertions : NumericAssertions<ushort>
{
    internal UInt16Assertions(ushort value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    private protected override string CalculateDifferenceForFailureMessage(ushort subject, ushort expected)
    {
        if (subject < 10 && expected < 10)
        {
            return null;
        }

        int difference = subject - expected;
        return difference != 0 ? difference.ToString(CultureInfo.InvariantCulture) : null;
    }
}
