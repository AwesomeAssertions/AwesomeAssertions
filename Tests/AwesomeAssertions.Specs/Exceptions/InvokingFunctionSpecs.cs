using System;
using Xunit;

namespace AwesomeAssertions.Specs.Exceptions;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
#pragma warning disable CA1806,MA0060 // false-positive, the result is used in the action
public class InvokingFunctionSpecs
{
    [Fact]
    public void Invoking_on_null_is_not_allowed()
    {
        Does someClass = null;

        Action act = () => someClass.Invoking(d => d.Return());

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("subject");
    }

    [Fact]
    public void Invoking_with_null_is_not_allowed()
    {
        Does someClass = Does.NotThrow();

        Action act = () => someClass.Invoking<Does, object>(null);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("action");
    }
}
