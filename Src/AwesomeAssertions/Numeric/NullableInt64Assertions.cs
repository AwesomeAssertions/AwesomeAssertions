using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that a nullable <see cref="long"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
internal class NullableInt64Assertions : NullableNumericAssertions<long>
{
    internal NullableInt64Assertions(long? value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    private protected override string CalculateDifferenceForFailureMessage(long subject, long expected)
    {
        if (subject is > 0 and < 10 && expected is > 0 and < 10)
        {
            return null;
        }

        decimal difference = (decimal)subject - expected;
        return difference != 0 ? difference.ToString(CultureInfo.InvariantCulture) : null;
    }
}
