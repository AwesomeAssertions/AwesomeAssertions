using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="ushort"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
internal class NullableUInt16Assertions : NullableNumericAssertions<ushort>
{
    internal NullableUInt16Assertions(ushort? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    protected override NumericAssertions<ushort> AsTAssertions() => new UInt16Assertions(Subject.GetValueOrDefault(), CurrentAssertionChain);

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
