using Xunit;

namespace AwesomeAssertions.Specs.Collections;

public partial class CollectionAssertionSpecs
{
    public class AllBeEquivalentTo
    {
        [Fact]
        public void Can_ignore_casing_while_comparing_collections_of_strings()
        {
            var actual = new[] { "test", "tEst", "Test", "TEst", "teST" };
            const string expectation = "test";

            actual.Should().AllBeEquivalentTo(expectation, o => o.IgnoringCase());
        }

        [Fact]
        public void Can_ignore_leading_whitespace_while_comparing_collections_of_strings()
        {
            var actual = new[] { " test", "test", "\ttest", "\ntest", "  \t \n test" };
            const string expectation = "test";

            actual.Should().AllBeEquivalentTo(expectation, o => o.IgnoringLeadingWhitespace());
        }

        [Fact]
        public void Can_ignore_trailing_whitespace_while_comparing_collections_of_strings()
        {
            var actual = new[] { "test ", "test", "test\t", "test\n", "test  \t \n " };
            const string expectation = "test";

            actual.Should().AllBeEquivalentTo(expectation, o => o.IgnoringTrailingWhitespace());
        }

        [Fact]
        public void Can_ignore_newline_style_while_comparing_collections_of_strings()
        {
            var actual = new[] { "A\nB\nC", "A\r\nB\r\nC", "A\r\nB\nC", "A\nB\r\nC" };
            const string expectation = "A\nB\nC";

            actual.Should().AllBeEquivalentTo(expectation, o => o.IgnoringNewlineStyle());
        }
    }
}
