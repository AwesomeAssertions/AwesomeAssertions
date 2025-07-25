using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

/// <content>
/// The [Not]ContainEquivalentOf specs.
/// </content>
public partial class StringAssertionSpecs
{
    public class ContainEquivalentOf
    {
        [Fact]
        public void Succeed_for_different_strings_using_custom_matching_comparer()
        {
            // Arrange
            var comparer = new AlwaysMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "XYZ";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expect, o => o.Using(comparer));
        }

        [Fact]
        public void Fail_for_same_strings_using_custom_not_matching_comparer()
        {
            // Arrange
            var comparer = new NeverMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "ABC";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expect, o => o.Using(comparer));

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_casing_while_checking_a_string_to_contain_another()
        {
            // Arrange
            string actual = "this is a string containing test.";
            string expect = "TEST";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expect, o => o.IgnoringCase());
        }

        [Fact]
        public void Can_ignore_leading_whitespace_while_checking_a_string_to_contain_another()
        {
            // Arrange
            string actual = "  this is a string containing test.";
            string expect = "test";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expect, o => o.IgnoringLeadingWhitespace());
        }

        [Fact]
        public void Can_ignore_trailing_whitespace_while_checking_a_string_to_contain_another()
        {
            // Arrange
            string actual = "this is a string containing test.  ";
            string expect = "test";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expect, o => o.IgnoringTrailingWhitespace());
        }

        [Fact]
        public void Can_ignore_newline_style_while_checking_a_string_to_contain_another()
        {
            // Arrange
            string actual = "this is a string containing \rA\nB\r\nC.\n";
            string expect = "A\r\nB\nC";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expect, o => o.IgnoringNewlineStyle());
        }

        [InlineData("aa", "A")]
        [InlineData("aCCa", "acca")]
        [Theory]
        public void Should_pass_when_contains_equivalent_of(string actual, string equivalentSubstring)
        {
            // Assert
            actual.Should().ContainEquivalentOf(equivalentSubstring);
        }

        [Fact]
        public void Should_fail_contain_equivalent_of_when_not_contains()
        {
            // Act
            Action act = () =>
                "a".Should().ContainEquivalentOf("aa");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected string \"a\" to contain the equivalent of \"aa\" at least 1 time, but found it 0 times.");
        }

        [Fact]
        public void Should_throw_when_null_equivalent_is_expected()
        {
            // Act
            Action act = () =>
                "a".Should().ContainEquivalentOf(null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Cannot assert string containment against <null>.*")
                .WithParameterName("expected");
        }

        [Fact]
        public void Should_throw_when_empty_equivalent_is_expected()
        {
            // Act
            Action act = () =>
                "a".Should().ContainEquivalentOf("");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Cannot assert string containment against an empty string.*")
                .WithParameterName("expected");
        }

        public class ContainEquivalentOfExactly
        {
            [Fact]
            public void When_containment_equivalent_of_once_is_asserted_against_null_it_should_throw_earlier()
            {
                // Arrange
                string actual = "a";
                string expectedSubstring = null;

                // Act
                Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, Exactly.Once());

                // Assert
                act
                    .Should().Throw<ArgumentNullException>()
                    .WithMessage("Cannot assert string containment against <null>.*");
            }

            [Fact]
            public void
                When_string_containment_equivalent_of_exactly_once_is_asserted_and_actual_value_is_null_then_it_should_throw_earlier()
            {
                // Arrange
                string actual = null;
                string expectedSubstring = "XyZ";

                // Act
                Action act = () =>
                    actual.Should().ContainEquivalentOf(expectedSubstring, Exactly.Once(), "that is {0}", "required");

                // Assert
                act.Should().Throw<XunitException>()
                    .WithMessage(
                        "Expected * <null> to contain the equivalent of \"XyZ\" exactly 1 time because that is required, but found it 0 times.");
            }

            [Fact]
            public void
                When_string_containment_equivalent_of_exactly_is_asserted_and_actual_value_contains_the_expected_string_exactly_expected_times_it_should_not_throw()
            {
                // Arrange
                string actual = "abCDEBcDF";
                string expectedSubstring = "Bcd";

                // Act / Assert
                actual.Should().ContainEquivalentOf(expectedSubstring, Exactly.Times(2));
            }

            [Fact]
            public void
                When_string_containment_equivalent_of_exactly_is_asserted_and_actual_value_contains_the_expected_string_but_not_exactly_expected_times_it_should_throw()
            {
                // Arrange
                string actual = "abCDEBcDF";
                string expectedSubstring = "Bcd";

                // Act
                Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, Exactly.Times(3));

                // Assert
                act.Should().Throw<XunitException>()
                    .WithMessage(
                        "Expected * \"abCDEBcDF\" to contain the equivalent of \"Bcd\" exactly 3 times, but found it 2 times.");
            }

            [Fact]
            public void
                When_string_containment_equivalent_of_exactly_once_is_asserted_and_actual_value_does_not_contain_the_expected_string_it_should_throw()
            {
                // Arrange
                string actual = "abCDEf";
                string expectedSubstring = "xyS";

                // Act
                Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, Exactly.Once());

                // Assert
                act.Should().Throw<XunitException>()
                    .WithMessage("Expected * \"abCDEf\" to contain the equivalent of \"xyS\" exactly 1 time, but found it 0 times.");
            }

            [Fact]
            public void When_containment_equivalent_of_exactly_once_is_asserted_against_an_empty_string_it_should_throw_earlier()
            {
                // Arrange
                string actual = "a";
                string expectedSubstring = "";

                // Act
                Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, Exactly.Once());

                // Assert
                act
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Cannot assert string containment against an empty string.*");
            }
        }
    }

    public class ContainEquivalentOfAtLeast
    {
        [Fact]
        public void
            When_string_containment_equivalent_of_at_least_is_asserted_and_actual_value_contains_the_expected_string_at_least_expected_times_it_should_not_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, AtLeast.Times(2));
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_at_least_is_asserted_and_actual_value_contains_the_expected_string_but_not_at_least_expected_times_it_should_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, AtLeast.Times(3));

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected * \"abCDEBcDF\" to contain the equivalent of \"Bcd\" at least 3 times, but found it 2 times.");
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_at_least_once_is_asserted_and_actual_value_does_not_contain_the_expected_string_it_should_throw_earlier()
        {
            // Arrange
            string actual = "abCDEf";
            string expectedSubstring = "xyS";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, AtLeast.Once());

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected * \"abCDEf\" to contain the equivalent of \"xyS\" at least 1 time, but found it 0 times.");
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_at_least_once_is_asserted_and_actual_value_is_null_then_it_should_throw_earlier()
        {
            // Arrange
            string actual = null;
            string expectedSubstring = "XyZ";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, AtLeast.Once());

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected * <null> to contain the equivalent of \"XyZ\" at least 1 time, but found it 0 times.");
        }
    }

    public class ContainEquivalentOfMoreThan
    {
        [Fact]
        public void
            When_string_containment_equivalent_of_more_than_is_asserted_and_actual_value_contains_the_expected_string_more_than_expected_times_it_should_not_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, MoreThan.Times(1));
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_more_than_is_asserted_and_actual_value_contains_the_expected_string_but_not_more_than_expected_times_it_should_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, MoreThan.Times(2));

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected * \"abCDEBcDF\" to contain the equivalent of \"Bcd\" more than 2 times, but found it 2 times.");
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_more_than_once_is_asserted_and_actual_value_does_not_contain_the_expected_string_it_should_throw_earlier()
        {
            // Arrange
            string actual = "abCDEf";
            string expectedSubstring = "xyS";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, MoreThan.Once());

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected * \"abCDEf\" to contain the equivalent of \"xyS\" more than 1 time, but found it 0 times.");
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_more_than_once_is_asserted_and_actual_value_is_null_then_it_should_throw_earlier()
        {
            // Arrange
            string actual = null;
            string expectedSubstring = "XyZ";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, MoreThan.Once());

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected * <null> to contain the equivalent of \"XyZ\" more than 1 time, but found it 0 times.");
        }
    }

    public class ContainEquivalentOfAtMost
    {
        [Fact]
        public void
            When_string_containment_equivalent_of_at_most_is_asserted_and_actual_value_contains_the_expected_string_at_most_expected_times_it_should_not_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, AtMost.Times(2));
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_at_most_is_asserted_and_actual_value_contains_the_expected_string_but_not_at_most_expected_times_it_should_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, AtMost.Times(1));

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected * \"abCDEBcDF\" to contain the equivalent of \"Bcd\" at most 1 time, but found it 2 times.");
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_at_most_once_is_asserted_and_actual_value_does_not_contain_the_expected_string_it_should_not_throw()
        {
            // Arrange
            string actual = "abCDEf";
            string expectedSubstring = "xyS";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, AtMost.Once());
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_at_most_once_is_asserted_and_actual_value_is_null_then_it_should_not_throw()
        {
            // Arrange
            string actual = null;
            string expectedSubstring = "XyZ";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, AtMost.Once());
        }
    }

    public class ContainEquivalentOfLessThan
    {
        [Fact]
        public void
            When_string_containment_equivalent_of_less_than_is_asserted_and_actual_value_contains_the_expected_string_less_than_expected_times_it_should_not_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, LessThan.Times(3));
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_less_than_is_asserted_and_actual_value_contains_the_expected_string_but_not_less_than_expected_times_it_should_throw()
        {
            // Arrange
            string actual = "abCDEBcDF";
            string expectedSubstring = "Bcd";

            // Act
            Action act = () => actual.Should().ContainEquivalentOf(expectedSubstring, LessThan.Times(2));

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected * \"abCDEBcDF\" to contain the equivalent of \"Bcd\" less than 2 times, but found it 2 times.");
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_less_than_twice_is_asserted_and_actual_value_does_not_contain_the_expected_string_it_should_throw()
        {
            // Arrange
            string actual = "abCDEf";
            string expectedSubstring = "xyS";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, LessThan.Twice());
        }

        [Fact]
        public void
            When_string_containment_equivalent_of_less_than_twice_is_asserted_and_actual_value_is_null_then_it_should_not_throw()
        {
            // Arrange
            string actual = null;
            string expectedSubstring = "XyZ";

            // Act / Assert
            actual.Should().ContainEquivalentOf(expectedSubstring, LessThan.Twice());
        }
    }

    public class NotContainEquivalentOf
    {
        [Fact]
        public void Succeed_for_same_strings_using_custom_not_matching_comparer()
        {
            // Arrange
            var comparer = new NeverMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "ABC";

            // Act / Assert
            actual.Should().NotContainEquivalentOf(expect, o => o.Using(comparer));
        }

        [Fact]
        public void Fail_for_different_strings_using_custom_matching_comparer()
        {
            // Arrange
            var comparer = new AlwaysMatchingEqualityComparer();
            string actual = "ABC";
            string expect = "XYZ";

            // Act
            Action act = () => actual.Should().NotContainEquivalentOf(expect, o => o.Using(comparer));

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_casing_while_checking_a_string_to_not_contain_another()
        {
            // Arrange
            string actual = "this is a string containing test.";
            string expect = "TEST";

            // Act
            Action act = () => actual.Should().NotContainEquivalentOf(expect, o => o.IgnoringCase());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_leading_whitespace_while_checking_a_string_to_not_contain_another()
        {
            // Arrange
            string actual = "  this is a string containing test.";
            string expect = "test";

            // Act
            Action act = () => actual.Should().NotContainEquivalentOf(expect, o => o.IgnoringLeadingWhitespace());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_trailing_whitespace_while_checking_a_string_to_not_contain_another()
        {
            // Arrange
            string actual = "this is a string containing test.  ";
            string expect = "test";

            // Act
            Action act = () => actual.Should().NotContainEquivalentOf(expect, o => o.IgnoringTrailingWhitespace());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Can_ignore_newline_style_while_checking_a_string_to_not_contain_another()
        {
            // Arrange
            string actual = "this is a string containing \rA\nB\r\nC.\n";
            string expect = "\nA\r\nB\rC";

            // Act
            Action act = () => actual.Should().NotContainEquivalentOf(expect, o => o.IgnoringNewlineStyle());

            // Assert
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void Should_fail_when_asserting_string_does_not_contain_equivalent_of_null()
        {
            // Act
            Action act = () =>
                "a".Should().NotContainEquivalentOf(null);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect string to contain the equivalent of <null>, but found \"a\".");
        }

        [Fact]
        public void Should_fail_when_asserting_string_does_not_contain_equivalent_of_empty()
        {
            // Act
            Action act = () =>
                "a".Should().NotContainEquivalentOf("");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect string to contain the equivalent of \"\", but found \"a\".");
        }

        [Fact]
        public void Should_fail_when_asserting_string_does_not_contain_equivalent_of_another_string_but_it_does()
        {
            // Act
            Action act = () =>
                "Hello, world!".Should().NotContainEquivalentOf(", worLD!");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect string to contain the equivalent of \", worLD!\" but found \"Hello, world!\".");
        }

        [Fact]
        public void Should_succeed_when_asserting_string_does_not_contain_equivalent_of_another_string()
        {
            // Act / Assert
            "aAa".Should().NotContainEquivalentOf("aa ");
        }
    }
}
