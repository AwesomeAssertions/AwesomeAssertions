using System;
using AwesomeAssertions;
using NUnit.Framework;

// This is essential to ensure proper operation of NUnit within the Microsoft Testing Platform,
// because with this settings the test adapter loads nunit.framework twice.
[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

namespace NUnit4.Mtp.Specs;

public sealed class FrameworkSpecs
{
    [Test]
    public void Throw_nunit_framework_exception_for_nunit4_tests()
    {
        // Act
        Action act = () => 0.Should().Be(1);

        // Assert
        // Here, it is essential to verify that the correct exception type from NUnit is thrown,
        // because the test adapter loads the nunit.framework assembly twice.
        act.Should().Throw<AssertionException>();
    }
}
