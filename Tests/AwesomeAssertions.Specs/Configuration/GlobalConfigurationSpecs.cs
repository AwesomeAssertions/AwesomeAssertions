using System;
using System.Threading.Tasks;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Configuration;

[Collection("ConfigurationSpecs")]
public sealed class GlobalConfigurationSpecs : IDisposable
{
    [Fact]
    public void Concurrently_accessing_the_configuration_is_safe()
    {
        // Act
        Action act = () => Parallel.For(
            0,
            10000,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            },
            __ =>
            {
                AssertionConfiguration.Current.Formatting.ValueFormatterAssembly = string.Empty;
                _ = AssertionConfiguration.Current.Formatting.ValueFormatterDetectionMode;
            }
        );

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Tracing_must_be_safe_when_executed_concurrently()
    {
        try
        {
            // Arrange
            AssertionConfiguration.Current.Equivalency.Modify(e => e.WithTracing());

            Parallel.For(1, 10_000, (_, _) =>
            {
                try
                {
                    new { A = "a" }.Should().BeEquivalentTo(new { A = "b" });
                }
                catch (XunitException)
                {
                    // we are only interested in exception not related to the actual assertion.
                }
            });
        }
        finally
        {
            AssertionConfiguration.Current.Equivalency.Modify(_ => new());
        }
    }

    [Fact]
    public void Can_override_the_runtime_test_framework_implementation()
    {
        // Arrange
        AssertionEngine.TestFramework = new NotImplementedTestFramework();

        // Act
        var act = () => 1.Should().Be(2);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Can_override_the_runtime_test_framework()
    {
        // Arrange
        AssertionEngine.Configuration.TestFramework = AwesomeAssertions.Configuration.TestFramework.NUnit;

        // Act
        var act = () => 1.Should().Be(2);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*nunit.framework*");
    }

    private class NotImplementedTestFramework : ITestFramework
    {
        public bool IsAvailable => true;

        public void Throw(string message) => throw new NotImplementedException();
    }

    public void Dispose() => AssertionEngine.ResetToDefaults();
}
