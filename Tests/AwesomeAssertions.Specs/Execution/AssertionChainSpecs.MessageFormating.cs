using System;
using System.Collections.Generic;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Execution;

/// <content>
/// The message formatting specs.
/// </content>
public partial class AssertionChainSpecs
{
    public class MessageFormatting
    {
        [Fact]
        public void Multiple_assertions_in_an_assertion_scope_are_all_reported()
        {
            // Arrange
            var scope = new AssertionScope();

            AssertionChain.GetOrCreate().FailWith("Failure");
            AssertionChain.GetOrCreate().FailWith("Failure");

            using (new AssertionScope())
            {
                AssertionChain.GetOrCreate().FailWith("Failure");
                AssertionChain.GetOrCreate().FailWith("Failure");
            }

            // Act
            Action act = scope.Dispose;

            // Assert
            act.Should().Throw<XunitException>()
                .Which.Message.Should().Contain("Failure", Exactly.Times(4));
        }

        [InlineData("foo")]
        [InlineData("{}")]
        [Theory]
        public void The_failure_message_uses_the_name_of_the_scope_as_context(string context)
        {
            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope(context);
                new[] { 1, 2, 3 }.Should().Equal(3, 2, 1);
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected {context} to be equal to*");
        }

        [Fact]
        public void The_failure_message_uses_the_lazy_name_of_the_scope_as_context()
        {
            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope(() => "lazy foo");
                new[] { 1, 2, 3 }.Should().Equal(3, 2, 1);
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected lazy foo to be equal to*");
        }

        [Fact]
        public void The_failure_message_includes_all_failures()
        {
            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                var values = new Dictionary<int, int>();
                values.Should().ContainKey(0);
                values.Should().ContainKey(1);
            };

            // Assert
            act.Should().Throw<XunitException>()
                .Which.Message.Should().Match("""
                    Expected * to contain key 0.
                    Expected * to contain key 1.

                    """);
        }

        [Fact]
        public void The_failure_message_includes_all_failures_as_well()
        {
            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                var values = new List<int>();
                values.Should().ContainSingle();
                values.Should().ContainSingle();
            };

            // Assert
            act.Should().Throw<XunitException>()
                .Which.Message.Should().Match("""
                    Expected * to contain a single item, but the collection is empty.
                    Expected * to contain a single item, but the collection is empty.

                    """);
        }

        [Fact]
        public void The_reason_can_contain_parentheses()
        {
            // Act
            Action act = () => 1.Should().Be(2, "can't use these in becauseArgs: {0} {1}", "{", "}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*because can't use these in becauseArgs: { }*");
        }

        [Fact]
        public void Because_reason_should_ignore_undefined_arguments()
        {
            // Act
            object[] becauseArgs = null;
            Action act = () => 1.Should().Be(2, "it should still work", becauseArgs);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*because it should still work*");
        }

        [Fact]
        public void Because_reason_should_threat_parentheses_as_literals_if_no_arguments_are_defined()
        {
            // Act
#pragma warning disable CA2241
            // ReSharper disable once FormatStringProblem
            Action act = () => 1.Should().Be(2, "use of {} is okay if there are no because arguments");
#pragma warning restore CA2241

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*because use of {} is okay if there are no because arguments*");
        }

        [Fact]
        public void Because_reason_should_inform_about_invalid_parentheses_with_a_default_message()
        {
            // Act
#pragma warning disable CA2241
            // ReSharper disable once FormatStringProblem
            Action act = () => 1.Should().Be(2, "use of {} is considered invalid in because parameter with becauseArgs",
                "additional becauseArgs argument");
#pragma warning restore CA2241

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "*because message 'use of {} is considered invalid in because parameter with becauseArgs' could not be formatted with string.Format*");
        }

        [Fact]
        public void Message_should_keep_parentheses_in_literal_values()
        {
            // Act
            Action act = () => "{foo}".Should().Be("{bar}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected string to be the same string*\"{foo}\"*\"{bar}\"*");
        }

        [Fact]
        public void Message_should_contain_literal_value_if_marked_with_double_parentheses()
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate().FailWith("{{empty}}");

            // Assert
            act.Should().ThrowExactly<XunitException>()
                .WithMessage("{empty}*");
        }

        [InlineData("\r")]
        [InlineData("\\r")]
        [InlineData("\\\r")]
        [InlineData("\\\\r")]
        [InlineData("\\\\\r")]
        [InlineData("\n")]
        [InlineData("\\n")]
        [InlineData("\\\n")]
        [InlineData("\\\\n")]
        [InlineData("\\\\\n")]
        [Theory]
        public void Message_should_not_have_modified_carriage_return_or_line_feed_control_characters(string str)
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate().FailWith(str);

            // Assert
            act.Should().ThrowExactly<XunitException>()
                .WithMessage(str);
        }

        [InlineData("\r")]
        [InlineData("\\r")]
        [InlineData("\\\r")]
        [InlineData(@"\\r")]
        [InlineData("\\\\\r")]
        [InlineData("\n")]
        [InlineData("\\n")]
        [InlineData("\\\n")]
        [InlineData(@"\\n")]
        [InlineData("\\\\\n")]
        [Theory]
        public void Message_should_not_have_modified_carriage_return_or_line_feed_control_characters_in_supplied_arguments(
            string str)
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate().FailWith(@"\{0}\A", str);

            // Assert
            act.Should().ThrowExactly<XunitException>()
                .WithMessage("\\\"" + str + "\"\\A*");
        }

        [Fact]
        public void Message_should_not_have_trailing_backslashes_removed_from_subject()
        {
            // Arrange / Act
            Action act = () => "A\\".Should().Be("A");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""*"A\"*"A"*""");
        }

        [Fact]
        public void Message_should_not_have_trailing_backslashes_removed_from_expectation()
        {
            // Arrange / Act
            Action act = () => "A".Should().Be("A\\");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""*"A"*"A\"*""");
        }

        [Fact]
        public void Message_should_have_scope_reportable_values_appended_at_the_end()
        {
            // Arrange
            var scope = new AssertionScope();
            scope.AddReportable("SomeKey", "SomeValue");
            scope.AddReportable("AnotherKey", "AnotherValue");

            AssertionChain.GetOrCreate().FailWith("{SomeKey}{AnotherKey}");

            // Act
            Action act = scope.Dispose;

            // Assert
            act.Should().ThrowExactly<XunitException>()
                .WithMessage("""
                    *With SomeKey:
                    SomeValue
                    With AnotherKey:
                    AnotherValue

                    """);
        }

        [Fact]
        public void Message_should_not_contain_reportable_values_added_to_chain_without_surrounding_scope()
        {
            // Arrange
            var chain = AssertionChain.GetOrCreate();
            chain.AddReportable("SomeKey", "SomeValue");
            chain.AddReportable("AnotherKey", "AnotherValue");

            // Act
            Action act = () => chain.FailWith("{SomeKey}{AnotherKey}");

            // Assert
            act.Should().ThrowExactly<XunitException>()
                .Which.Message.Should().BeEmpty();
        }

        [Fact]
        public void Deferred_reportable_values_should_not_be_calculated_in_absence_of_failures()
        {
            // Arrange
            var scope = new AssertionScope();
            var deferredValueInvoked = false;

            scope.AddReportable("MyKey", () =>
            {
                deferredValueInvoked = true;

                return "MyValue";
            });

            // Act
            scope.Dispose();

            // Assert
            deferredValueInvoked.Should().BeFalse();
        }

        [Fact]
        public void Message_should_start_with_the_defined_expectation()
        {
            // Act
            Action act = () =>
            {
                var assertion = AssertionChain.GetOrCreate();

                assertion
                    .WithExpectation("Expectations are the root ", chain => chain
                        .ForCondition(false)
                        .FailWith("of disappointment"));
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expectations are the root of disappointment");
        }

        [Fact]
        public void Message_should_start_with_the_defined_expectation_and_arguments()
        {
            // Act
            Action act = () =>
            {
                var assertion = AssertionChain.GetOrCreate();

                assertion
                    .WithExpectation("Expectations are the {0} ", "root", chain => chain.ForCondition(false)
                        .FailWith("of disappointment"));
            };

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expectations are the \"root\" of disappointment");
        }

        [Fact]
        public void Message_should_contain_object_as_context_if_identifier_can_not_be_resolved()
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate()
                .ForCondition(false)
                .FailWith("Expected {context}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected object");
        }

        [Fact]
        public void Message_should_contain_the_fallback_value_as_context_if_identifier_can_not_be_resolved()
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate()
                .ForCondition(false)
                .FailWith("Expected {context:fallback}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected fallback");
        }

        [Fact]
        public void Message_should_contain_the_default_identifier_as_context_if_identifier_can_not_be_resolved()
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate()
                .WithDefaultIdentifier("identifier")
                .ForCondition(false)
                .FailWith("Expected {context}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected identifier");
        }

        [Fact]
        public void Message_should_contain_the_reason_as_defined()
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate()
                .BecauseOf("because reasons")
                .FailWith("Expected{reason}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected because reasons");
        }

        [Fact]
        public void Message_should_contain_the_reason_as_defined_with_arguments()
        {
            // Act
            Action act = () => AssertionChain.GetOrCreate()
                .BecauseOf("because {0}", "reasons")
                .FailWith("Expected{reason}");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected because reasons");
        }
    }
}
