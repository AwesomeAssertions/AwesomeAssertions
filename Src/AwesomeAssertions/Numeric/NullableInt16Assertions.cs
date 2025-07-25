using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="short"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
internal class NullableInt16Assertions : NullableNumericAssertions<short>
{
    internal NullableInt16Assertions(short? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    private protected override string CalculateDifferenceForFailureMessage(short subject, short expected)
    {
        if (subject < 10 && expected < 10)
        {
            return null;
        }

        int difference = subject - expected;
        return difference != 0 ? difference.ToString(CultureInfo.InvariantCulture) : null;
    }
}
