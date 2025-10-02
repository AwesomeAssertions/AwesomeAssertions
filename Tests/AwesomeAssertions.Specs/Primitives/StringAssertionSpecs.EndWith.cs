using System;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

/// <content>
/// The [Not]EndWith specs.
/// </content>
public partial class StringAssertionSpecs
{
    public class EndWith
    {
        [Fact]
        public void When_asserting_string_ends_with_a_suffix_it_should_not_throw()
        {
            // Arrange
            string actual = "ABC";
            string expectedSuffix = "BC";

            // Act / Assert
            actual.Should().EndWith(expectedSuffix);
        }

        [Fact]
        public void When_asserting_string_ends_with_the_same_value_it_should_not_throw()
        {
            // Arrange
            string actual = "ABC";
            string expectedSuffix = "ABC";

            // Act / Assert
            actual.Should().EndWith(expectedSuffix);
        }

        [Fact]
        public void When_string_does_not_end_with_expected_phrase_it_should_throw()
        {
            // Act
            Action act = () =>
            {
                using var a = new AssertionScope();
                "ABC".Should().EndWith("AB", "it should");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                """
                Expected string to end with the same string because it should, but they differ before index 2:
                     ↓ (actual)
                  "ABC"
                   "AB"
                     ↑ (expected).
                """
            );
        }

        [Fact]
        public void When_string_ending_is_compared_with_null_it_should_throw()
        {
            // Act
            Action act = () => "ABC".Should().EndWith(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot compare string end with <null>.*");
        }

        [Fact]
        public void When_string_ending_is_compared_with_empty_string_it_should_not_throw()
        {
            // Act / Assert
            "ABC".Should().EndWith("");
        }

        [Fact]
        public void When_string_ending_is_compared_with_string_that_is_longer_it_should_throw()
        {
            // Act
            Action act = () => "ABC".Should().EndWith("00ABC");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 0*
                        ↓ (actual)
                        "ABC"
                      "00ABC"
                        ↑ (expected).
                    """);
        }

        [Fact]
        public void Correctly_stop_further_execution_when_inside_assertion_scope()
        {
            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                "ABC".Should().EndWith("00ABC").And.EndWith("CBA00");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "*\"00ABC\"*");
        }

        [Fact]
        public void When_string_ending_is_compared_and_actual_value_is_null_then_it_should_throw()
        {
            // Arrange
            string someString = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                someString.Should().EndWith("ABC");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected someString to end with \"ABC\", but found <null>.");
        }

        [Fact]
        public void When_short_strings_have_no_common_chars_arrows_are_aligned()
        {
            // Act
            Action act = () => "A".Should().EndWith("B");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 0*
                       ↓ (actual)
                      "A"
                      "B"
                       ↑ (expected).
                    """);
        }

        [Fact]
        public void When_one_string_is_long_and_the_other_is_short_arrows_are_aligned()
        {
            // Arrange
            const string subject = "this is a long text that has more than 60 characters so it requires ellipsis";
            const string expected = "requires an ellipsis";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 66*
                                     ↓ (actual)
                      "…so it requires ellipsis"
                          "requires an ellipsis"
                                     ↑ (expected).
                    """);
        }

        [Fact]
        public void When_expected_is_short_strings_are_right_aligned()
        {
            // Act
            Action act = () => "ABCDEFGHI".Should().EndWith("H");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 8*
                               ↓ (actual)
                      "ABCDEFGHI"
                              "H"
                               ↑ (expected).
                    """);
        }

        [Fact]
        public void When_long_string_does_not_end_with_long_string_at_middle_strings_are_right_aligned()
        {
            // Act
            Action act = () => "ABCDEFGHI".Should().EndWith("DEXGHI");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 5*
                            ↓ (actual)
                      "ABCDEFGHI"
                         "DEXGHI"
                            ↑ (expected).
                    """);
        }

        [Fact]
        public void When_expected_becomes_longer_than_actual_after_truncation_strings_are_right_aligned()
        {
            // Arrange
            const string subject = "from cancel this waver was coming from this is a long text pat thaT differs in between two words";
            const string expected = "such and when to this the other they should and sad rhino whicH differs in between two words";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                """
                *before index 66*
                                  ↓ (actual)
                    "…text pat thaT differs in between two words"
                  "…sad rhino whicH differs in between two words"
                                  ↑ (expected).
                """);
        }

        [Fact]
        public void When_both_strings_are_truncated_from_left_arrows_are_aligned()
        {
            // Arrange
            var subject = "this is a very long text. it is very long text that12345 differs.";

            var expected = "this is a very long text. it is very long text that1264 differs.";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 55*
                                     ↓ (actual)
                      "…text that12345 differs."
                       "…text that1264 differs."
                                     ↑ (expected).
                    """);
        }

        [Fact]
        public void When_both_strings_are_truncated_from_both_ends_arrows_are_aligned()
        {
            // Arrange
            var subject = "this is a very long text. it is very long text that12345 differs in between two words. it is very lengthy.";

            var expected = "this is a very long text. it is very long text that1264 differs in between two words. it is very lengthy.";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *before index 55*
                                     ↓ (actual)
                      "…text that12345 differs in between two words. it is very…"
                       "…text that1264 differs in between two words. it is very…"
                                     ↑ (expected).
                    """);
        }

        [Fact]
        public void When_both_strings_are_truncated_from_left_with_new_lines_arrows_are_aligned()
        {
            // Arrange
            var subject = """
                          this is a very long text. it is very long text
                          that12345
                          differs.
                          """.RemoveNewlineStyle();

            var expected = """
                           this is a very long text. it is very long text
                           that1264
                           differs.
                           """.RemoveNewlineStyle();

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *line 2*column 9*index 55*
                                      ↓ (actual)
                      "…text\nthat12345\ndiffers."
                       "…text\nthat1264\ndiffers."
                                      ↑ (expected).
                    """);
        }

        [Fact]
        public void When_both_strings_are_truncated_from_right_with_new_lines_arrows_are_aligned()
        {
            // Arrange
            var subject = """
                          this is a very long text
                          that12345
                          differs in between two words. it is very lengthy.
                          """.RemoveNewlineStyle();

            var expected = """
                           this is a very long text
                           that1264
                           differs in between two words. it is very lengthy.
                           """.RemoveNewlineStyle();

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *line 2*column 9*index 33*
                                      ↓ (actual)
                      "…text\nthat12345\ndiffers in between two words. it is very…"
                       "…text\nthat1264\ndiffers in between two words. it is very…"
                                      ↑ (expected).
                    """);
        }

        [Fact]
        public void When_both_strings_are_truncated_from_both_ends_with_new_lines_arrows_are_aligned()
        {
            // Arrange
            var subject = """
                          this is a very long text. it is very long text
                          that12345
                          differs in between two words. it is very lengthy.
                          """.RemoveNewlineStyle();

            var expected = """
                           this is a very long text. it is very long text
                           that1264
                           differs in between two words. it is very lengthy.
                           """.RemoveNewlineStyle();

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                """
                *line 2*column 9*index 55*
                                  ↓ (actual)
                  "…text\nthat12345\ndiffers in between two words. it is very…"
                   "…text\nthat1264\ndiffers in between two words. it is very…"
                                  ↑ (expected).
                """);
        }

        [Fact]
        public void When_subject_shorter_than_expected_arrows_are_aligned()
        {
            // Act
            Action act = () => "H".Should().EndWith("ABCDEFGHI");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                """
                *before index 0*
                           ↓ (actual)
                          "H"
                  "ABCDEFGHI"
                           ↑ (expected).
                """
            );
        }

        [Fact]
        public void When_both_strings_blank_they_are_equal()
        {
            // Assert
            "".Should().EndWith("");
        }
    }

    public class NotEndWith
    {
        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_and_it_does_not_it_should_succeed()
        {
            // Arrange
            string value = "ABC";

            // Act / Assert
            value.Should().NotEndWith("AB");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_but_it_does_it_should_fail_with_a_descriptive_message()
        {
            // Arrange
            string value = "ABC";

            // Act
            Action action = () =>
                value.Should().NotEndWith("BC", "because of some {0}", "reason");

            // Assert
            action.Should().Throw<XunitException>().WithMessage(
                "Expected value not to end with \"BC\" because of some reason, but found \"ABC\".");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_that_is_null_it_should_throw()
        {
            // Arrange
            string value = "ABC";

            // Act
            Action action = () =>
                value.Should().NotEndWith(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot compare end of string with <null>.*");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_that_is_empty_it_should_throw()
        {
            // Arrange
            string value = "ABC";

            // Act
            Action action = () =>
                value.Should().NotEndWith("");

            // Assert
            action.Should().Throw<XunitException>().WithMessage(
                "Expected value not to end with \"\", but found \"ABC\".");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_and_actual_value_is_null_it_should_throw()
        {
            // Arrange
            string someString = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                someString.Should().NotEndWith("ABC", "some {0}", "reason");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected someString not to end with \"ABC\"*some reason*, but found <null>.");
        }
    }
}
