using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Numeric;

/// <summary>
/// Contains a number of methods to assert that a <see cref="float"/> is in the expected state.
/// </summary>
[DebuggerNonUserCode]
internal class SingleAssertions : NumericAssertions<float>
{
    internal SingleAssertions(float value, AssertionChain assertionChain)
        : base(value, assertionChain)
    {
    }

    private protected override bool IsNaN(float value) => float.IsNaN(value);

    private protected override string CalculateDifferenceForFailureMessage(float subject, float expected)
    {
        float difference = subject - expected;
        return difference != 0 ? difference.ToString("R", CultureInfo.InvariantCulture) : null;
    }
}
