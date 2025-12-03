using System;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs;

public class AssertionFailureSpecs
{
    private static readonly string AssertionsTestSubClassName = typeof(AssertionsTestSubClass).Name;

    [Fact]
    public void When_reason_starts_with_because_it_should_not_do_anything()
    {
        // Arrange
        var assertions = new AssertionsTestSubClass();

        // Act
        Action action = () =>
            assertions.AssertFail("because {0} should always fail.", AssertionsTestSubClassName);

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("Expected it to fail because AssertionsTestSubClass should always fail.");
    }

    [Fact]
    public void When_reason_does_not_start_with_because_it_should_be_added()
    {
        // Arrange
        var assertions = new AssertionsTestSubClass();

        // Act
        Action action = () =>
            assertions.AssertFail("{0} should always fail.", AssertionsTestSubClassName);

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("Expected it to fail because AssertionsTestSubClass should always fail.");
    }

    [Fact]
    public void When_reason_starts_with_because_but_is_prefixed_with_blanks_it_should_not_do_anything()
    {
        // Arrange
        var assertions = new AssertionsTestSubClass();

        // Act
        Action action = () =>
            assertions.AssertFail("\r\nbecause {0} should always fail.", AssertionsTestSubClassName);

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("Expected it to fail\r\nbecause AssertionsTestSubClass should always fail.");
    }

    [Fact]
    public void When_reason_starts_with_because_without_blanks_but_reason_placeholder_has_newlines_it_should_not_do_anything()
    {
        // Act
        Action action = () =>
            AssertFail("""
                Expected it to fail
                {reason}
                """, "because {0} should always fail.", AssertionsTestSubClassName);

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("""
                Expected it to fail
                because AssertionsTestSubClass should always fail.
                """);
    }

    [Fact]
    public void When_reason_does_not_start_with_because_but_is_prefixed_with_blanks_it_should_add_because_after_the_blanks()
    {
        // Arrange
        var assertions = new AssertionsTestSubClass();

        // Act
        Action action = () =>
            assertions.AssertFail("\r\n{0} should always fail.", AssertionsTestSubClassName);

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("Expected it to fail\r\nbecause AssertionsTestSubClass should always fail.");
    }

    [Fact]
    public void When_reason_is_empty_and_starts_on_newline_it_should_remove_following_comma_and_space()
    {
        // Act
        Action action = () =>
            AssertFail($"Expected it to fail{Environment.NewLine}{{reason}}, ");

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be($"Expected it to fail{Environment.NewLine}");
    }

    [Fact]
    public void When_reason_is_not_empty_and_starts_on_newline_it_keeps_following_comma_and_space()
    {
        // Act
        Action action = () =>
            AssertFail($"Expected it to fail{Environment.NewLine}{{reason}}, ", "this is a reason {0}", AssertionsTestSubClassName);

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be($"Expected it to fail{Environment.NewLine}because this is a reason {AssertionsTestSubClassName}, ");
    }

    [Fact]
    public void When_reason_is_empty_and_does_not_start_on_newline_it_should_keep_following_comma_and_space()
    {
        // Act
        Action action = () =>
            AssertFail("Expected it to fail{reason}, ");

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("Expected it to fail, ");
    }

    [Fact]
    public void When_reason_is_empty_and_last_in_message_it_is_removed()
    {
        // Act
        Action action = () =>
            AssertFail("Expected it to fail{reason}");

        // Assert
        action.Should().Throw<XunitException>()
            .Which.Message.Should().Be("Expected it to fail");
    }

    private static void AssertFail(string message, string because = "", params object[] becauseArgs)
    {
        AssertionChain.GetOrCreate()
            .BecauseOf(because, becauseArgs)
            .FailWith(message);
    }

    internal class AssertionsTestSubClass
    {
        private readonly AssertionChain assertionChain = AssertionChain.GetOrCreate();

        public void AssertFail(string because, params object[] becauseArgs)
        {
            assertionChain
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected it to fail{reason}");
        }
    }
}
