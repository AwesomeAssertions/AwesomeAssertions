using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

/// <content>
/// The [Not]BeEquivalentTo specs.
/// </content>
public partial class StringAssertionSpecs
{
    public class BeEquivalentTo
    {
        [Fact]
        public void Succeed_for_different_strings_using_custom_matching_comparer()
        {
            // Arrange
            var comparer = new AlwaysMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "XYZ";

            // Act / Assert
            actual.Should().BeEquivalentTo(expect, o => o.Using(comparer));
        }

        [Fact]
        public void Fail_for_same_strings_using_custom_not_matching_comparer()
        {
            // Arrange
            var comparer = new NeverMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "ABC";

            // Act
            Action act = () => actual.Should().BeEquivalentTo(expect, o => o.Using(comparer));

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_comparer_does_not_need_equivalent_lengths_strings_are_still_equivalent()
        {
            // Arrange
            var comparer = new ForwardSlashTrimmingEqualityComparer();
            string actual = "ABC";
            string expect = "ABC/";

            // Act / Assert
            actual.Should().BeEquivalentTo(expect, o => o.Using(comparer));
        }

        [Fact]
        public void When_comparer_trims_nonwhitespace_whitespace_still_disqualifies_equivalency_with_logical_error_message()
        {
            // Arrange
            var comparer = new ForwardSlashTrimmingEqualityComparer();

            // Act
            Action act = () => "ab ".Should().BeEquivalentTo("ab/", o => o.Using(comparer));

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string to be equivalent *index 2*\"ab \"*\"ab/\"*.");
        }

        [Fact]
        public void Can_ignore_casing_while_comparing_strings_to_be_equivalent()
        {
            // Arrange
            string actual = "test";
            string expect = "TEST";

            // Act / Assert
            actual.Should().BeEquivalentTo(expect, o => o.IgnoringCase());
        }

        [Fact]
        public void Can_ignore_leading_whitespace_while_comparing_strings_to_be_equivalent()
        {
            // Arrange
            string actual = "  test";
            string expect = "test";

            // Act / Assert
            actual.Should().BeEquivalentTo(expect, o => o.IgnoringLeadingWhitespace());
        }

        [Fact]
        public void Can_ignore_trailing_whitespace_while_comparing_strings_to_be_equivalent()
        {
            // Arrange
            string actual = "test  ";
            string expect = "test";

            // Act / Assert
            actual.Should().BeEquivalentTo(expect, o => o.IgnoringTrailingWhitespace());
        }

        [Fact]
        public void Can_ignore_newline_style_while_comparing_strings_to_be_equivalent()
        {
            // Arrange
            string actual = "A\nB\r\nC";
            string expect = "A\r\nB\nC";

            // Act / Assert
            actual.Should().BeEquivalentTo(expect, o => o.IgnoringNewlineStyle());
        }

        [Fact]
        public void When_strings_are_the_same_while_ignoring_case_it_should_not_throw()
        {
            // Arrange
            string actual = "ABC";
            string expectedEquivalent = "abc";

            // Act / Assert
            actual.Should().BeEquivalentTo(expectedEquivalent);
        }

        [Fact]
        public void When_strings_differ_other_than_by_case_it_should_throw()
        {
            // Act
            Action act = () => "ADC".Should().BeEquivalentTo("abc", "we will test {0} + {1}", 1, 2);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string to be equivalent *because we will test 1 + 2, but*index 1*\"ADC\"*\"abc\"*.");
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_non_null_string_is_expected_to_be_equivalent_to_null_it_should_throw()
        {
            // Act
            Action act = () => "ABCDEF".Should().BeEquivalentTo(null);

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string to be equivalent to <null>, but found \"ABCDEF\".");
        }

        [Fact]
        public void When_non_empty_string_is_expected_to_be_equivalent_to_empty_it_should_throw()
        {
            // Act
            Action act = () => "ABC".Should().BeEquivalentTo("");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string to be equivalent * index 0*\"ABC\"*\"\"*.");
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_string_is_equivalent_but_too_short_it_should_throw()
        {
            // Act
            Action act = () => "AB".Should().BeEquivalentTo("ABCD");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string to be equivalent *index 2*\"AB\"*\"ABCD\"*.");
        }

        [Fact]
        public void When_string_equivalence_is_asserted_and_actual_value_is_null_then_it_should_throw()
        {
            // Act
            string someString = null;
            Action act = () => someString.Should().BeEquivalentTo("abc", "we will test {0} + {1}", 1, 2);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected someString to be equivalent to \"abc\" because we will test 1 + 2, but found <null>.");
        }

        [Fact]
        public void
            When_the_expected_string_is_equivalent_to_the_actual_string_but_with_trailing_spaces_it_should_throw_with_clear_error_message()
        {
            // Act
            Action act = () => "ABC".Should().BeEquivalentTo("abc ", "because I say {0}", "so");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *index 3*
                          ↓ (actual)
                      "ABC"
                      "abc "
                          ↑ (expected).
                    """);
        }

        [Fact]
        public void
            When_the_actual_string_equivalent_to_the_expected_but_with_trailing_spaces_it_should_throw_with_clear_error_message()
        {
            // Act
            Action act = () => "ABC ".Should().BeEquivalentTo("abc", "because I say {0}", "so");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage(
                    """
                    *index 3*
                          ↓ (actual)
                      "ABC "
                      "abc"
                          ↑ (expected).
                    """);
        }
    }

    public class NotBeEquivalentTo
    {
        [Fact]
        public void Succeed_for_same_strings_using_custom_not_matching_comparer()
        {
            // Arrange
            var comparer = new NeverMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "ABC";

            // Act / Assert
            actual.Should().NotBeEquivalentTo(expect, o => o.Using(comparer));
        }

        [Fact]
        public void Fail_for_different_strings_using_custom_matching_comparer()
        {
            // Arrange
            var comparer = new AlwaysMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "XYZ";

            // Act
            Action act = () => actual.Should().NotBeEquivalentTo(expect, o => o.Using(comparer));

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_casing_while_comparing_strings_to_not_be_equivalent()
        {
            // Arrange
            string actual = "test";
            string expect = "TEST";

            // Act
            Action act = () => actual.Should().NotBeEquivalentTo(expect, o => o.IgnoringCase());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_leading_whitespace_while_comparing_strings_to_not_be_equivalent()
        {
            // Arrange
            string actual = "  test";
            string expect = "test";

            // Act
            Action act = () => actual.Should().NotBeEquivalentTo(expect, o => o.IgnoringLeadingWhitespace());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_trailing_whitespace_while_comparing_strings_to_not_be_equivalent()
        {
            // Arrange
            string actual = "test  ";
            string expect = "test";

            // Act
            Action act = () => actual.Should().NotBeEquivalentTo(expect, o => o.IgnoringTrailingWhitespace());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_newline_style_while_comparing_strings_to_not_be_equivalent()
        {
            // Arrange
            string actual = "\rA\nB\r\nC\n";
            string expect = "\nA\r\nB\nC\r";

            // Act
            Action act = () => actual.Should().NotBeEquivalentTo(expect, o => o.IgnoringNewlineStyle());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_strings_are_the_same_while_ignoring_case_it_should_throw()
        {
            // Arrange
            string actual = "ABC";
            string unexpected = "abc";

            // Act
            Action action = () => actual.Should().NotBeEquivalentTo(unexpected, "because I say {0}", "so");

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected actual not to be equivalent to \"abc\" because I say so, but they are.");
        }

        [Fact]
        public void When_strings_differ_other_than_by_case_it_should_not_throw()
        {
            // Act / Assert
            "ADC".Should().NotBeEquivalentTo("abc");
        }

        [Fact]
        public void When_non_null_string_is_expected_to_be_equivalent_to_null_it_should_not_throw()
        {
            // Act / Assert
            "ABCDEF".Should().NotBeEquivalentTo(null);
        }

        [Fact]
        public void When_non_empty_string_is_expected_to_be_equivalent_to_empty_it_should_not_throw()
        {
            // Act / Assert
            "ABC".Should().NotBeEquivalentTo("");
        }

        [Fact]
        public void When_string_is_equivalent_but_too_short_it_should_not_throw()
        {
            // Act / Assert
            "AB".Should().NotBeEquivalentTo("ABCD");
        }

        [Fact]
        public void When_string_equivalence_is_asserted_and_actual_value_is_null_then_it_should_not_throw()
        {
            // Arrange
            string someString = null;

            // Act / Assert
            someString.Should().NotBeEquivalentTo("abc");
        }

        [Fact]
        public void When_the_expected_string_is_equivalent_to_the_actual_string_but_with_trailing_spaces_it_should_not_throw()
        {
            // Act / Assert
            "ABC".Should().NotBeEquivalentTo("abc ");
        }

        [Fact]
        public void When_the_actual_string_equivalent_to_the_expected_but_with_trailing_spaces_it_should_not_throw()
        {
            // Act / Assert
            "ABC ".Should().NotBeEquivalentTo("abc");
        }
    }
}
