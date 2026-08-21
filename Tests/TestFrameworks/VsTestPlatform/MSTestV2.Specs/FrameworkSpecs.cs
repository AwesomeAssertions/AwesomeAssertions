using System;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTestV2.Specs;

[TestClass]
public class FrameworkSpecs
{
    /// <summary>
    ///     We are testing MSTest here version 2 and 3.
    /// </summary>
    /// <remarks>
    ///     MSTest V2 and V3 are compatible.
    ///     So, testing "as" V2 with reference to V3 is fine.
    /// </remarks>
    [TestMethod]
    public void When_mstestv2_is_used_it_should_throw_mstest_exceptions_for_assertion_failures()
    {
        // Act
        Action act = () => 0.Should().Be(1);

        // Assert
        Exception exception = act.Should().Throw<Exception>().Which;

        // Don't reference the exception type explicitly like this: act.Should().Throw<AssertFailedException>()
        // It could cause this specs project to load the assembly containing the exception (this actually happens for xUnit)
        exception.GetType().FullName.Should().Be("Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException");
    }
}
